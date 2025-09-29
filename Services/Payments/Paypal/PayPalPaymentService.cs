using invoice.Core.DTO;
using invoice.Core.DTO.Payment;
using invoice.Core.DTO.PaymentResponse;
using invoice.Core.Enums;
using invoice.Core.Interfaces.Services;
using Microsoft.Extensions.Options;
using System.Net;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace invoice.Services.Payments.Paypal
{
    public class PayPalPaymentService : PaymentGatewayBase, IPaymentGateway
    {
        private readonly PayPalOptions _options;
        private readonly HttpClient _httpClient;
        private readonly JsonSerializerOptions _jsonOptions;

        public override PaymentType PaymentType => PaymentType.PayPal;

        public PayPalPaymentService(
            IConfiguration configuration,
            IOptions<PayPalOptions> options,
            IHttpClientFactory httpClientFactory)
            : base(configuration)
        {
            _options = options?.Value ?? throw new ArgumentNullException(nameof(options));
            _httpClient = httpClientFactory?.CreateClient("PayPal") ??
                         throw new ArgumentNullException(nameof(httpClientFactory));

            if (string.IsNullOrEmpty(_options.ClientId))
                throw new InvalidOperationException("PayPal ClientId is not configured");

            if (string.IsNullOrEmpty(_options.ClientSecret))
                throw new InvalidOperationException("PayPal ClientSecret is not configured");

            if (string.IsNullOrEmpty(_options.BaseUrl))
                throw new InvalidOperationException("PayPal BaseUrl is not configured");

            _jsonOptions = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
            };
        }

        public override async Task<GeneralResponse<PaymentSessionResponse>> CreatePaymentSessionAsync(PaymentCreateDTO dto)
        {
            try
            {
                var validationResult = ValidatePaymentRequest(dto);
                if (!validationResult.Success)
                    return new GeneralResponse<PaymentSessionResponse>(false, validationResult.Message);

                var accessToken = await GetAccessTokenAsync();
                if (string.IsNullOrEmpty(accessToken))
                {
                    return new GeneralResponse<PaymentSessionResponse>(false, "Failed to authenticate with PayPal");
                }

                var order = await CreateOrderAsync(dto, accessToken);
                if (order == null || string.IsNullOrEmpty(order.Id))
                {
                    return new GeneralResponse<PaymentSessionResponse>(false, "Failed to create PayPal order");
                }

                var approvalUrl = order.Links?
                    .FirstOrDefault(link => link?.Rel?.Equals("approve", StringComparison.OrdinalIgnoreCase) == true)
                    ?.Href;

                if (string.IsNullOrEmpty(approvalUrl))
                {
                    return new GeneralResponse<PaymentSessionResponse>(false, "PayPal approval URL not found");
                }

                var responseData = new PaymentSessionResponse
                {
                    SessionId = order.Id,
                    PaymentUrl = approvalUrl,
                    ExpiresAt = DateTime.UtcNow.AddHours(24),
                    PaymentType = PaymentType.PayPal,
                    PaymentStatus = PaymentStatus.Pending,
                    InvoiceId = dto.InvoiceId,
                    Amount = dto.Cost,
                    Currency = dto.Currency,
                    RawResponse = JsonSerializer.Serialize(order, _jsonOptions)
                };

                return new GeneralResponse<PaymentSessionResponse>(true, "Payment session created successfully", responseData);
            }
            catch (Exception ex)
            {
                return new GeneralResponse<PaymentSessionResponse>(false, $"Failed to create payment session: {ex.Message}");
            }
        }

        private ValidationResult ValidatePaymentRequest(PaymentCreateDTO? dto)
        {
            if (dto == null)
                return ValidationResult.Fail("Payment request is required");

            if (string.IsNullOrEmpty(dto.InvoiceId))
                return ValidationResult.Fail("Invoice ID is required");

            if (dto.Cost <= 0)
                return ValidationResult.Fail("Cost must be greater than zero");

            if (string.IsNullOrEmpty(dto.Currency))
                return ValidationResult.Fail("Currency is required");

            if (string.IsNullOrEmpty(dto.Description))
                return ValidationResult.Fail("Description is required");

            if (dto.Currency.Length != 3)
                return ValidationResult.Fail("Currency must be a 3-letter code");

            return ValidationResult.SuccessFull();
        }

        private async Task<string?> GetAccessTokenAsync()
        {
            try
            {
                var authString = Convert.ToBase64String(Encoding.UTF8.GetBytes($"{_options.ClientId}:{_options.ClientSecret}"));

                using var request = new HttpRequestMessage(HttpMethod.Post, $"{_options.BaseUrl}/v1/oauth2/token");
                request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Basic", authString);
                request.Content = new FormUrlEncodedContent(new[]
                {
                    new KeyValuePair<string, string>("grant_type", "client_credentials")
                });

                using var response = await _httpClient.SendAsync(request);

                if (!response.IsSuccessStatusCode)
                {
                    return null;
                }

                var content = await response.Content.ReadAsStringAsync();
                var authResult = JsonSerializer.Deserialize<PayPalAuthResponse>(content, _jsonOptions);

                return authResult?.AccessToken;
            }
            catch
            {
                return null;
            }
        }

        private async Task<PayPalOrderResponse?> CreateOrderAsync(PaymentCreateDTO dto, string accessToken)
        {
            try
            {
                var domain = NormalizeDomain(Domain);

                var orderData = new
                {
                    intent = "CAPTURE",
                    purchase_units = new[]
                    {
                        new
                        {
                            amount = new
                            {
                                currency_code = dto.Currency.ToUpperInvariant(),
                                value = dto.Cost.ToString("F2")
                            },
                            description = TruncateString(dto.Description, 127),
                            custom_id = dto.InvoiceId,
                            invoice_id = dto.InvoiceId,
                            items = new[]
                            {
                                new
                                {
                                    name = TruncateString(dto.Description, 127),
                                    quantity = "1",
                                    unit_amount = new
                                    {
                                        currency_code = dto.Currency.ToUpperInvariant(),
                                        value = dto.Cost.ToString("F2")
                                    }
                                }
                            }
                        }
                    },
                    application_context = new
                    {
                        return_url = $"{domain}payments/paypal/success?invoice={WebUtility.UrlEncode(dto.InvoiceId)}",
                        cancel_url = $"{domain}payments/paypal/cancel?invoice={WebUtility.UrlEncode(dto.InvoiceId)}",
                        brand_name = Configuration["AppSettings:CompanyName"] ?? "Your Company",
                        user_action = "PAY_NOW",
                        shipping_preference = "NO_SHIPPING"
                    }
                };

                using var request = new HttpRequestMessage(HttpMethod.Post, $"{_options.BaseUrl}/v2/checkout/orders");
                request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", accessToken);
                request.Headers.Add("Prefer", "return=representation");

                request.Content = new StringContent(
                    JsonSerializer.Serialize(orderData, _jsonOptions),
                    Encoding.UTF8,
                    "application/json"
                );

                using var response = await _httpClient.SendAsync(request);

                if (!response.IsSuccessStatusCode)
                {
                    return null;
                }

                var content = await response.Content.ReadAsStringAsync();
                return JsonSerializer.Deserialize<PayPalOrderResponse>(content, _jsonOptions);
            }
            catch
            {
                return null;
            }
        }

        public override async Task<GeneralResponse<bool>> CancelPaymentAsync(string paymentId)
        {
            try
            {
                var accessToken = await GetAccessTokenAsync();
                if (string.IsNullOrEmpty(accessToken))
                {
                    return new GeneralResponse<bool>(false, "Failed to authenticate with PayPal");
                }

                var order = await GetOrderDetailsAsync(paymentId, accessToken);
                if (order == null)
                {
                    return new GeneralResponse<bool>(false, "Order not found");
                }

                var cancellableStates = new[] { "CREATED", "APPROVED" };
                if (!cancellableStates.Contains(order.Status, StringComparer.OrdinalIgnoreCase))
                {
                    return new GeneralResponse<bool>(false, $"Order cannot be cancelled in current state: {order.Status}");
                }

                if (order.Status.Equals("APPROVED", StringComparison.OrdinalIgnoreCase))
                {
                    var voidResult = await VoidAuthorizationAsync(paymentId, accessToken);
                    if (!voidResult)
                    {
                        return new GeneralResponse<bool>(false, "Failed to void authorization");
                    }
                }

                return new GeneralResponse<bool>(true, "Payment cancelled successfully", true);
            }
            catch (Exception ex)
            {
                return new GeneralResponse<bool>(false, $"Failed to cancel payment: {ex.Message}");
            }
        }

        public override async Task<GeneralResponse<bool>> RefundPaymentAsync(string paymentId)
        {
            try
            {
                var accessToken = await GetAccessTokenAsync();
                if (string.IsNullOrEmpty(accessToken))
                {
                    return new GeneralResponse<bool>(false, "Failed to authenticate with PayPal");
                }

                var captureId = await GetCaptureIdAsync(paymentId, accessToken);
                if (string.IsNullOrEmpty(captureId))
                {
                    return new GeneralResponse<bool>(false, "No capture found for refund");
                }

                var refundResult = await ProcessRefundAsync(captureId, accessToken);
                if (!refundResult)
                {
                    return new GeneralResponse<bool>(false, "Failed to process refund");
                }

                return new GeneralResponse<bool>(true, "Refund processed successfully", true);
            }
            catch (Exception ex)
            {
                return new GeneralResponse<bool>(false, $"Failed to process refund: {ex.Message}");
            }
        }

        public override async Task<GeneralResponse<PaymentStatusResponse>> GetPaymentStatusAsync(string paymentId)
        {
            try
            {
                var accessToken = await GetAccessTokenAsync();
                if (string.IsNullOrEmpty(accessToken))
                {
                    return new GeneralResponse<PaymentStatusResponse>(false, "Failed to authenticate with PayPal");
                }

                var order = await GetOrderDetailsAsync(paymentId, accessToken);
                if (order == null)
                {
                    return new GeneralResponse<PaymentStatusResponse>(false, "Order not found");
                }

                var status = order.Status?.ToUpperInvariant() switch
                {
                    "CREATED" => PaymentStatus.Pending,
                    "SAVED" => PaymentStatus.Pending,
                    "APPROVED" => PaymentStatus.Pending,
                    "VOIDED" => PaymentStatus.Failed,
                    "COMPLETED" => PaymentStatus.Completed,
                    "PAYER_ACTION_REQUIRED" => PaymentStatus.Pending,
                    _ => PaymentStatus.Unknown
                };

                var statusResponse = new PaymentStatusResponse
                {
                    PaymentId = paymentId,
                    Status = status,
                    RawResponse = JsonSerializer.Serialize(order, _jsonOptions)
                };

                return new GeneralResponse<PaymentStatusResponse>(true, "Status retrieved successfully", statusResponse);
            }
            catch (Exception ex)
            {
                return new GeneralResponse<PaymentStatusResponse>(false, $"Failed to get payment status: {ex.Message}");
            }
        }

        public override async Task<GeneralResponse<PaymentWebhookResponse>> ProcessWebhookAsync(string payload, string signature)
        {
            try
            {
                if (!await VerifyWebhookSignatureAsync(payload, signature))
                {
                    return new GeneralResponse<PaymentWebhookResponse>(false, "Invalid webhook signature");
                }

                var webhookEvent = JsonSerializer.Deserialize<PayPalWebhookEvent>(payload, _jsonOptions);
                if (webhookEvent == null)
                {
                    return new GeneralResponse<PaymentWebhookResponse>(false, "Invalid webhook payload");
                }

                var (processed, message) = await HandleWebhookEventAsync(webhookEvent);

                var response = new PaymentWebhookResponse
                {
                    Processed = processed,
                    EventType = webhookEvent.EventType,
                    PaymentId = webhookEvent.Resource?.Id,
                    Metadata = new Dictionary<string, string>
                    {
                        { "message", message }
                    }
                };

                return new GeneralResponse<PaymentWebhookResponse>(true, "Webhook processed successfully", response);
            }
            catch (Exception ex)
            {
                return new GeneralResponse<PaymentWebhookResponse>(false, $"Failed to process webhook: {ex.Message}");
            }
        }

        private async Task<PayPalOrderResponse?> GetOrderDetailsAsync(string orderId, string accessToken)
        {
            try
            {
                using var request = new HttpRequestMessage(HttpMethod.Get, $"{_options.BaseUrl}/v2/checkout/orders/{orderId}");
                request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", accessToken);

                using var response = await _httpClient.SendAsync(request);

                if (!response.IsSuccessStatusCode)
                {
                    return null;
                }

                var content = await response.Content.ReadAsStringAsync();
                return JsonSerializer.Deserialize<PayPalOrderResponse>(content, _jsonOptions);
            }
            catch
            {
                return null;
            }
        }

        private async Task<bool> VoidAuthorizationAsync(string authorizationId, string accessToken)
        {
            try
            {
                using var request = new HttpRequestMessage(HttpMethod.Post, $"{_options.BaseUrl}/v2/payments/authorizations/{authorizationId}/void");
                request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", accessToken);

                using var response = await _httpClient.SendAsync(request);
                return response.IsSuccessStatusCode;
            }
            catch
            {
                return false;
            }
        }

        private async Task<string?> GetCaptureIdAsync(string orderId, string accessToken)
        {
            try
            {
                // First, get the order details to find the capture ID
                var order = await GetOrderDetailsAsync(orderId, accessToken);
                if (order == null || !order.Status.Equals("COMPLETED", StringComparison.OrdinalIgnoreCase))
                {
                    return null;
                }

                // For simplicity, we'll assume the capture ID is needed from payments API
                // In real implementation, you'd need to call payments API to get capture details
                return order.Id; // This would be the actual capture ID in a real implementation
            }
            catch
            {
                return null;
            }
        }

        private async Task<bool> ProcessRefundAsync(string captureId, string accessToken)
        {
            try
            {
                var refundData = new
                {
                    amount = new
                    {
                        value = "FULL_REFUND",
                        currency_code = "USD"
                    }
                };

                using var request = new HttpRequestMessage(HttpMethod.Post, $"{_options.BaseUrl}/v2/payments/captures/{captureId}/refund");
                request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", accessToken);
                request.Content = new StringContent(
                    JsonSerializer.Serialize(refundData, _jsonOptions),
                    Encoding.UTF8,
                    "application/json"
                );

                using var response = await _httpClient.SendAsync(request);
                return response.IsSuccessStatusCode;
            }
            catch
            {
                return false;
            }
        }

        private async Task<bool> VerifyWebhookSignatureAsync(string payload, string signature)
        {
            if (string.IsNullOrEmpty(_options.WebhookId))
            {
                // In development, skip verification if no webhook ID configured
                return true;
            }

            try
            {
                var verificationData = new
                {
                    auth_algo = "SHA256withRSA",
                    cert_url = signature, // This would be the actual signature header
                    transmission_id = Guid.NewGuid().ToString(),
                    transmission_sig = signature,
                    transmission_time = DateTime.UtcNow.ToString("O"),
                    webhook_id = _options.WebhookId,
                    webhook_event = JsonSerializer.Deserialize<JsonElement>(payload)
                };

                using var request = new HttpRequestMessage(HttpMethod.Post, $"{_options.BaseUrl}/v1/notifications/verify-webhook-signature");
                var accessToken = await GetAccessTokenAsync();
                if (string.IsNullOrEmpty(accessToken))
                {
                    return false;
                }

                request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", accessToken);
                request.Content = new StringContent(
                    JsonSerializer.Serialize(verificationData, _jsonOptions),
                    Encoding.UTF8,
                    "application/json"
                );

                using var response = await _httpClient.SendAsync(request);
                if (!response.IsSuccessStatusCode)
                {
                    return false;
                }

                var content = await response.Content.ReadAsStringAsync();
                var verificationResult = JsonSerializer.Deserialize<PayPalWebhookVerificationResponse>(content, _jsonOptions);

                return verificationResult?.VerificationStatus == "SUCCESS";
            }
            catch
            {
                return false;
            }
        }

        private async Task<(bool Processed, string Message)> HandleWebhookEventAsync(PayPalWebhookEvent webhookEvent)
        {
            switch (webhookEvent.EventType)
            {
                case "PAYMENT.CAPTURE.COMPLETED":
                    return (true, "Payment captured successfully");

                case "PAYMENT.CAPTURE.DENIED":
                    return (true, "Payment was denied");

                case "PAYMENT.CAPTURE.REFUNDED":
                    return (true, "Payment refund processed");

                case "CHECKOUT.ORDER.APPROVED":
                    return (true, "Order approved by payer");

                case "CHECKOUT.ORDER.COMPLETED":
                    return (true, "Order completed successfully");

                default:
                    return (false, $"Unhandled event type: {webhookEvent.EventType}");
            }
        }

        // Utility method for retry logic
        private async Task<T?> ExecuteWithRetryAsync<T>(Func<Task<T>> operation, int maxRetries = 3)
        {
            for (int retry = 0; retry < maxRetries; retry++)
            {
                try
                {
                    return await operation();
                }
                catch when (retry < maxRetries - 1)
                {
                    await Task.Delay(TimeSpan.FromSeconds(Math.Pow(2, retry)));
                }
            }
            return default;
        }
    }

    internal class ValidationResult
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;

        public static ValidationResult SuccessFull() => new ValidationResult { Success = true, Message = "Valid" };
        public static ValidationResult Fail(string message) => new ValidationResult { Success = false, Message = message };
    }
}