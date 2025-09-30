using AutoMapper;
using invoice.Core.DTO;
using invoice.Core.DTO.Payment;
using invoice.Core.DTO.PaymentResponse;
using invoice.Core.Entities;
using invoice.Core.Enums;
using invoice.Core.Interfaces.Services;
using invoice.Repo;
using Microsoft.Extensions.Options;
using System.Net;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace invoice.Services.Payments.Paypal
{
    public class PayPalPaymentService : PaymentGatewayBase, IPaymentGateway
    {
        private readonly IRepository<Payment> _paymentRepo;
        private readonly IRepository<Invoice> _invoiceRepo;
        private readonly IRepository<Commission> _commissionRepo;
        private readonly JsonSerializerOptions _jsonOptions;
        private readonly PayPalOptions _options;
        private readonly HttpClient _httpClient;
        private readonly decimal _defaultCommissionRate;

        public override PaymentType PaymentType => PaymentType.PayPal;

        public PayPalPaymentService(
            IConfiguration configuration,
            IRepository<Payment> paymentRepo,
            IRepository<Invoice> invoiceRepo,
            IRepository<Commission> commissionRepo,
            IOptions<PayPalOptions> options,
            IHttpClientFactory httpClientFactory)
            : base(configuration)
        {
            _options = options?.Value ?? throw new ArgumentNullException(nameof(options));
            _paymentRepo = paymentRepo ?? throw new ArgumentNullException(nameof(paymentRepo));
            _invoiceRepo = invoiceRepo ?? throw new ArgumentNullException(nameof(invoiceRepo));
            _commissionRepo = commissionRepo ?? throw new ArgumentNullException(nameof(commissionRepo));
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

            var cfgRate = Configuration["Payments:DefaultCommissionPercent"];
            _defaultCommissionRate = decimal.TryParse(cfgRate, out var v) ? v / 100m : 0.1m;
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
                    return new GeneralResponse<PaymentSessionResponse>(false, "Failed to authenticate with PayPal");

                Invoice? invoice = null;
                if (!string.IsNullOrEmpty(dto.InvoiceId) && _invoiceRepo != null)
                {
                    invoice = await _invoiceRepo.GetByIdAsync(dto.InvoiceId);
                }

                var order = await CreateOrderAsync(dto, invoice, accessToken);
                if (order == null || string.IsNullOrEmpty(order.Id))
                    return new GeneralResponse<PaymentSessionResponse>(false, "Failed to create PayPal order");

                var approvalUrl = order.Links?
                    .FirstOrDefault(l => l?.Rel?.Equals("approve", StringComparison.OrdinalIgnoreCase) == true)
                    ?.Href;

                if (string.IsNullOrEmpty(approvalUrl))
                    return new GeneralResponse<PaymentSessionResponse>(false, "PayPal approval URL not found");

                var (commissionRate, commissionValue, sellerAmount) = await CalculateCommissionAsync(dto, invoice);

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

                if (invoice != null && _commissionRepo != null && invoice.Commission == null)
                {
                    var sellerId = await GetSellerIdFromInvoice(invoice);
                    if (!string.IsNullOrEmpty(sellerId))
                    {
                        var commission = new Commission
                        {
                            InvoiceId = invoice.Id,
                            SellerId = sellerId,
                            Value = commissionValue,
                            CreatedAt = DateTime.UtcNow,
                            Settled = false
                        };
                        await _commissionRepo.AddAsync(commission);
                        await _commissionRepo.SaveChangesAsync();
                    }
                }

                return new GeneralResponse<PaymentSessionResponse>(true, "Payment session created successfully", responseData);
            }
            catch (Exception ex)
            {
                return new GeneralResponse<PaymentSessionResponse>(false, $"Failed to create payment session: {ex.Message}");
            }
        }

        private async Task<(decimal commissionRate, decimal commissionValue, decimal sellerAmount)> CalculateCommissionAsync(PaymentCreateDTO dto, Invoice? invoice = null)
        {
            decimal commissionRate;
            decimal commissionValue;
            decimal sellerAmount;

            if (invoice?.Commission != null && invoice.FinalValue > 0)
            {
                commissionRate = invoice.Commission.Value / invoice.FinalValue;
                commissionValue = invoice.Commission.Value;
                sellerAmount = invoice.FinalValue - commissionValue;
            }
            else if (invoice?.FinalValue > 0)
            {
                commissionRate = await GetCommissionRateAsync(dto, invoice);
                commissionValue = Math.Round(invoice.FinalValue * commissionRate, 2);
                sellerAmount = Math.Round(invoice.FinalValue - commissionValue, 2);
            }
            else
            {
                commissionRate = await GetCommissionRateAsync(dto, invoice);
                commissionValue = Math.Round(dto.Cost * commissionRate, 2);
                sellerAmount = Math.Round(dto.Cost - commissionValue, 2);
            }

            return (commissionRate, commissionValue, sellerAmount);
        }

        private async Task<decimal> GetCommissionRateAsync(PaymentCreateDTO dto, Invoice? invoice = null)
        {
            if (invoice?.Commission != null && invoice.FinalValue > 0)
            {
                return invoice.Commission.Value / invoice.FinalValue;
            }

            if (dto.Metadata != null && dto.Metadata.TryGetValue("commission_percent", out var s) && decimal.TryParse(s, out var parsed))
            {
                return parsed / 100m;
            }

            if (invoice != null)
            {
                if (invoice.InvoiceType == InvoiceType.Online)
                {
                    return 0.05m;
                }
            }

            return _defaultCommissionRate;
        }

        private async Task<string?> GetSellerIdFromInvoice(Invoice invoice)
        {
            if (!string.IsNullOrEmpty(invoice.UserId))
                return invoice.UserId;

            if (!string.IsNullOrEmpty(invoice.ClientId))
                return invoice.ClientId;

            return null;
        }

        private string? GetSellerPaypalEmail(PaymentCreateDTO dto, Invoice? invoice = null)
        {
            if (dto.Metadata != null && dto.Metadata.TryGetValue("seller_paypal_email", out var email) && !string.IsNullOrWhiteSpace(email))
                return email;

            if (invoice?.User?.PaypalEmail != null)
                return invoice.User.PaypalEmail;

            return invoice?.User?.Email;
        }

        private ValidationResult ValidatePaymentRequest(PaymentCreateDTO? dto)
        {
            if (dto == null)
                return ValidationResult.Fail("Payment request is required");

            if (string.IsNullOrEmpty(dto.InvoiceId))
                return ValidationResult.Fail("Invoice ID is required");

            if (dto.Cost <= 0)
                return ValidationResult.Fail("Cost must be greater than zero");

            if (string.IsNullOrEmpty(dto.Currency) || dto.Currency.Length != 3)
                return ValidationResult.Fail("Currency must be a 3-letter code");

            if (string.IsNullOrEmpty(dto.Description))
                return ValidationResult.Fail("Description is required");

            return ValidationResult.SuccessFull();
        }

        private async Task<string?> GetAccessTokenAsync()
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
                var content = await response.Content.ReadAsStringAsync();
                throw new InvalidOperationException($"Failed to fetch PayPal access token: {response.StatusCode} - {content}");
            }

            var json = await response.Content.ReadAsStringAsync();
            var auth = JsonSerializer.Deserialize<PayPalAuthResponse>(json, _jsonOptions);
            return auth?.AccessToken;
        }

        private async Task<PayPalOrderResponse?> CreateOrderAsync(PaymentCreateDTO dto, Invoice? invoice, string accessToken)
        {
            var sellerEmail = GetSellerPaypalEmail(dto, invoice);
            var domain = NormalizeDomain(Domain);

            dto.Metadata ??= new Dictionary<string, string>();

            object purchaseUnit;
            if (!string.IsNullOrWhiteSpace(sellerEmail))
            {
                purchaseUnit = new
                {
                    amount = new
                    {
                        currency_code = (dto.Currency).ToUpperInvariant(),
                        value = dto.Cost.ToString("F2"),
                        breakdown = new
                        {
                            item_total = new
                            {
                                currency_code = (dto.Currency).ToUpperInvariant(),
                                value = dto.Cost.ToString("F2")
                            }
                        }
                    },
                    description = TruncateString(dto.Description, 127),
                    custom_id = dto.InvoiceId,
                    invoice_id = dto.InvoiceId,
                    payee = new { email_address = sellerEmail },
                    items = new[]
                    {
                new {
                    name = TruncateString(dto.Description, 127),
                    description = TruncateString(dto.Description, 127),
                    quantity = "1",
                    unit_amount = new {
                        currency_code = (dto.Currency).ToUpperInvariant(),
                        value = dto.Cost.ToString("F2")
                    }
                }
            }
                };
            }
            else
            {
                purchaseUnit = new
                {
                    amount = new
                    {
                        currency_code = (dto.Currency).ToUpperInvariant(),
                        value = dto.Cost.ToString("F2"),
                        breakdown = new
                        {
                            item_total = new
                            {
                                currency_code = (dto.Currency).ToUpperInvariant(),
                                value = dto.Cost.ToString("F2")
                            }
                        }
                    },
                    description = TruncateString(dto.Description, 127),
                    custom_id = dto.InvoiceId,
                    invoice_id = dto.InvoiceId,
                    items = new[]
                    {
                new {
                    name = TruncateString(dto.Description, 127),
                    description = TruncateString(dto.Description, 127),
                    quantity = "1",
                    unit_amount = new {
                        currency_code = (dto.Currency).ToUpperInvariant(),
                        value = dto.Cost.ToString("F2")
                    }
                }
            }
                };
            }

            var orderData = new
            {
                intent = "CAPTURE",
                purchase_units = new[] { purchaseUnit },
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

            request.Content = new StringContent(JsonSerializer.Serialize(orderData, _jsonOptions), Encoding.UTF8, "application/json");

            using var response = await _httpClient.SendAsync(request);
            if (!response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                throw new InvalidOperationException($"PayPal CreateOrder failed: {response.StatusCode} - {content}");
            }

            var json = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<PayPalOrderResponse>(json, _jsonOptions);
        }

        public override async Task<GeneralResponse<bool>> CancelPaymentAsync(string paymentId)
        {
            try
            {
                var accessToken = await GetAccessTokenAsync();
                if (string.IsNullOrEmpty(accessToken))
                    return new GeneralResponse<bool>(false, "Failed to authenticate with PayPal");

                var order = await GetOrderDetailsAsync(paymentId, accessToken);
                if (order == null)
                    return new GeneralResponse<bool>(false, "Order not found");

                var cancellableStates = new[] { "CREATED", "APPROVED" };
                if (!cancellableStates.Contains(order.Status, StringComparer.OrdinalIgnoreCase))
                    return new GeneralResponse<bool>(false, $"Order cannot be cancelled in current state: {order.Status}");

                if (order.Status.Equals("APPROVED", StringComparison.OrdinalIgnoreCase))
                {
                    var authorizationId = ExtractAuthorizationIdFromOrder(order);
                    if (!string.IsNullOrEmpty(authorizationId))
                    {
                        var voided = await VoidAuthorizationAsync(authorizationId, accessToken);
                        if (!voided) return new GeneralResponse<bool>(false, "Failed to void authorization");

                        await UpdateCommissionOnCancellation(paymentId);

                        return new GeneralResponse<bool>(true, "Authorization voided", true);
                    }
                    return new GeneralResponse<bool>(false, "No authorization id found to void");
                }

                await UpdateCommissionOnCancellation(paymentId);
                return new GeneralResponse<bool>(true, "Order cancelled or not yet captured", true);
            }
            catch (Exception ex)
            {
                return new GeneralResponse<bool>(false, $"Failed to cancel payment: {ex.Message}");
            }
        }

        private async Task UpdateCommissionOnCancellation(string paymentId)
        {
            try
            {
                if (_paymentRepo == null || _commissionRepo == null) return;

                var payment = await _paymentRepo.GetByIdAsync(paymentId);
                if (payment?.InvoiceId != null)
                {
                    var commissions = await _commissionRepo.GetAllAsync();
                    var commission = commissions.FirstOrDefault(c => c.InvoiceId == payment.InvoiceId);
                    if (commission != null)
                    {
                        commission.Settled = false;
                        await _commissionRepo.UpdateAsync(commission);
                        await _commissionRepo.SaveChangesAsync();
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed to update commission on cancellation: {ex.Message}");
            }
        }

        private string? ExtractAuthorizationIdFromOrder(PayPalOrderResponse order)
        {
            try
            {
                return order.PurchaseUnits?
                    .FirstOrDefault()?
                    .Payments?
                    .Authorizations?
                    .FirstOrDefault()?
                    .Id;
            }
            catch
            {
                return null;
            }
        }

        public override async Task<GeneralResponse<bool>> RefundPaymentAsync(string paymentId)
        {
            try
            {
                var accessToken = await GetAccessTokenAsync();
                if (string.IsNullOrEmpty(accessToken))
                    return new GeneralResponse<bool>(false, "Failed to authenticate with PayPal");

                var captureId = await GetCaptureIdAsync(paymentId, accessToken);
                if (string.IsNullOrEmpty(captureId))
                    return new GeneralResponse<bool>(false, "No capture found for refund");

                var payment = await _paymentRepo.GetByIdAsync(paymentId);
                if (payment == null)
                    return new GeneralResponse<bool>(false, "Payment record not found");

                var refundResult = await ProcessRefundAsync(captureId, accessToken, payment.Cost, payment.Currency);
                if (!refundResult)
                    return new GeneralResponse<bool>(false, "Failed to process refund");

                await UpdateCommissionOnRefund(payment);

                return new GeneralResponse<bool>(true, "Refund processed successfully", true);
            }
            catch (Exception ex)
            {
                return new GeneralResponse<bool>(false, $"Failed to process refund: {ex.Message}");
            }
        }

        private async Task UpdateCommissionOnRefund(Payment payment)
        {
            try
            {
                if (_commissionRepo == null || string.IsNullOrEmpty(payment.InvoiceId)) return;

                var commissions = await _commissionRepo.GetAllAsync();
                var commission = commissions.FirstOrDefault(c => c.InvoiceId == payment.InvoiceId);

                if (commission != null)
                {
                    commission.Settled = false;
                    await _commissionRepo.UpdateAsync(commission);
                    await _commissionRepo.SaveChangesAsync();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed to update commission on refund: {ex.Message}");
            }
        }

        public override async Task<GeneralResponse<PaymentStatusResponse>> GetPaymentStatusAsync(string paymentId)
        {
            try
            {
                var accessToken = await GetAccessTokenAsync();
                if (string.IsNullOrEmpty(accessToken))
                    return new GeneralResponse<PaymentStatusResponse>(false, "Failed to authenticate with PayPal");

                var order = await GetOrderDetailsAsync(paymentId, accessToken);
                if (order == null)
                    return new GeneralResponse<PaymentStatusResponse>(false, "Order not found");

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
                    return new GeneralResponse<PaymentWebhookResponse>(false, "Invalid webhook signature");

                var webhookEvent = JsonSerializer.Deserialize<PayPalWebhookEvent>(payload, _jsonOptions);
                if (webhookEvent == null)
                    return new GeneralResponse<PaymentWebhookResponse>(false, "Invalid webhook payload");

                var (processed, message) = await HandleWebhookEventAsync(webhookEvent);

                if (webhookEvent.EventType?.Equals("PAYMENT.CAPTURE.COMPLETED", StringComparison.OrdinalIgnoreCase) == true)
                {
                    await HandleCompletedPaymentAsync(webhookEvent);
                }

                var response = new PaymentWebhookResponse
                {
                    Processed = processed,
                    EventType = webhookEvent.EventType,
                    PaymentId = webhookEvent.Resource.HasValue && webhookEvent.Resource.Value.TryGetProperty("id", out var idProp) ? idProp.GetString() : null,
                    Metadata = new Dictionary<string, string> { { "message", message } }
                };

                return new GeneralResponse<PaymentWebhookResponse>(true, "Webhook processed successfully", response);
            }
            catch (Exception ex)
            {
                return new GeneralResponse<PaymentWebhookResponse>(false, $"Failed to process webhook: {ex.Message}");
            }
        }

        private async Task HandleCompletedPaymentAsync(PayPalWebhookEvent webhookEvent)
        {
            if (!webhookEvent.Resource.HasValue || _invoiceRepo == null || _commissionRepo == null) return;

            try
            {
                var resource = webhookEvent.Resource.Value;
                string? invoiceId = null;

                if (resource.ValueKind == JsonValueKind.Object)
                {
                    if (resource.TryGetProperty("invoice_id", out var invProp) && invProp.ValueKind == JsonValueKind.String)
                        invoiceId = invProp.GetString();
                    else if (resource.TryGetProperty("custom_id", out var customProp) && customProp.ValueKind == JsonValueKind.String)
                        invoiceId = customProp.GetString();
                }

                if (string.IsNullOrEmpty(invoiceId)) return;

                var invoice = await _invoiceRepo.GetByIdAsync(invoiceId);
                if (invoice == null) return;

                var commissions = await _commissionRepo.GetAllAsync(userId: null);
                var commission = commissions.FirstOrDefault(c => c.InvoiceId == invoice.Id);
                if (commission == null)
                {
                    var (commissionRate, commissionValue, sellerAmount) = await CalculateCommissionAsync(
                        new PaymentCreateDTO { Cost = invoice.FinalValue }, invoice);

                    var sellerId = await GetSellerIdFromInvoice(invoice);
                    if (string.IsNullOrEmpty(sellerId)) return;

                    commission = new Commission
                    {
                        InvoiceId = invoice.Id,
                        SellerId = sellerId,
                        Value = commissionValue,
                        CreatedAt = DateTime.UtcNow,
                        Settled = false
                    };
                    await _commissionRepo.AddAsync(commission);
                    await _commissionRepo.SaveChangesAsync();
                }

                var sellerEmail = GetSellerPaypalEmail(new PaymentCreateDTO(), invoice);
                if (!string.IsNullOrEmpty(sellerEmail) && commission.Value < invoice.FinalValue)
                {
                    var sellerAmount = Math.Round(invoice.FinalValue - commission.Value, 2);
                    var accessToken = await GetAccessTokenAsync();
                    if (!string.IsNullOrEmpty(accessToken) && sellerAmount > 0)
                    {
                        var payoutOK = await PayoutToSellerAsync( sellerEmail, sellerAmount, "USD", accessToken);
                        if (payoutOK)
                        {
                            commission.Settled = true;
                            await _commissionRepo.UpdateAsync(commission);
                            await _commissionRepo.SaveChangesAsync();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error handling completed payment: {ex.Message}");
            }
        }

        private async Task<PayPalOrderResponse?> GetOrderDetailsAsync(string orderId, string accessToken)
        {
            using var request = new HttpRequestMessage(HttpMethod.Get, $"{_options.BaseUrl}/v2/checkout/orders/{orderId}");
            request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", accessToken);

            using var response = await _httpClient.SendAsync(request);
            if (!response.IsSuccessStatusCode) return null;

            var content = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<PayPalOrderResponse>(content, _jsonOptions);
        }

        private async Task<bool> VoidAuthorizationAsync(string authorizationId, string accessToken)
        {
            using var request = new HttpRequestMessage(HttpMethod.Post, $"{_options.BaseUrl}/v2/payments/authorizations/{authorizationId}/void");
            request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", accessToken);

            using var response = await _httpClient.SendAsync(request);
            return response.IsSuccessStatusCode;
        }

        private async Task<string?> GetCaptureIdAsync(string orderId, string accessToken)
        {
            var order = await GetOrderDetailsAsync(orderId, accessToken);
            return order?.PurchaseUnits?.FirstOrDefault()?.Payments?.Captures?.FirstOrDefault()?.Id;
        }

        private async Task<bool> ProcessRefundAsync(string captureId, string accessToken, decimal amount, string currency)
        {
            var refundData = new
            {
                amount = new
                {
                    value = amount.ToString("F2"),
                    currency_code = currency.ToUpperInvariant()
                }
            };

            using var request = new HttpRequestMessage(HttpMethod.Post, $"{_options.BaseUrl}/v2/payments/captures/{captureId}/refund");
            request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", accessToken);
            request.Content = new StringContent(JsonSerializer.Serialize(refundData, _jsonOptions), Encoding.UTF8, "application/json");

            using var response = await _httpClient.SendAsync(request);
            return response.IsSuccessStatusCode;
        }

        public async Task<bool> PayoutToSellerAsync(string sellerPaypalEmail, decimal amount, string currency, string accessToken)
        {
            try
            {
                var batchId = Guid.NewGuid().ToString("N");

                var payoutPayload = new
                {
                    sender_batch_header = new
                    {
                        sender_batch_id = batchId,
                        email_subject = Configuration["Payments:PayoutEmailSubject"] ?? "You have a payout",
                        email_message = Configuration["Payments:PayoutEmailMessage"] ?? "You received a payout from our platform"
                    },
                    items = new[]
                    {
                        new
                        {
                            recipient_type = "EMAIL",
                            amount = new { value = amount.ToString("F2"), currency = currency.ToUpperInvariant() },
                            receiver = sellerPaypalEmail,
                            note = "Payout for your sale",
                            sender_item_id = Guid.NewGuid().ToString("N")
                        }
                    }
                };

                using var request = new HttpRequestMessage(HttpMethod.Post, $"{_options.BaseUrl}/v1/payments/payouts");
                request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", accessToken);
                request.Content = new StringContent(JsonSerializer.Serialize(payoutPayload, _jsonOptions), Encoding.UTF8, "application/json");

                using var response = await _httpClient.SendAsync(request);
                return response.IsSuccessStatusCode;
            }
            catch
            {
                return false;
            }
        }

        private async Task<bool> VerifyWebhookSignatureAsync(string payload, string transmissionSig)
        {
            if (string.IsNullOrEmpty(_options.WebhookId))
            {
                // In development skip verification
                return true;
            }

            try
            {
                var verificationData = new
                {
                    transmission_id = Guid.NewGuid().ToString(),
                    transmission_time = DateTime.UtcNow.ToString("O"),
                    cert_url = "",
                    auth_algo = "SHA256withRSA",
                    transmission_sig = transmissionSig,
                    webhook_id = _options.WebhookId,
                    webhook_event = JsonSerializer.Deserialize<JsonElement>(payload)
                };

                using var request = new HttpRequestMessage(HttpMethod.Post, $"{_options.BaseUrl}/v1/notifications/verify-webhook-signature");
                var accessToken = await GetAccessTokenAsync();
                if (string.IsNullOrEmpty(accessToken)) return false;

                request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", accessToken);
                request.Content = new StringContent(JsonSerializer.Serialize(verificationData, _jsonOptions), Encoding.UTF8, "application/json");

                using var response = await _httpClient.SendAsync(request);
                if (!response.IsSuccessStatusCode) return false;

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

        private static string TruncateString(string input, int max)
        {
            if (string.IsNullOrEmpty(input)) return input ?? string.Empty;
            return input.Length <= max ? input : input.Substring(0, max);
        }

        internal class ValidationResult
        {
            public bool Success { get; set; }
            public string Message { get; set; } = string.Empty;

            public static ValidationResult SuccessFull() => new ValidationResult { Success = true, Message = "Valid" };
            public static ValidationResult Fail(string message) => new ValidationResult { Success = false, Message = message };
        }
    }
}