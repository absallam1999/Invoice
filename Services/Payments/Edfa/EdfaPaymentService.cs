//using invoice.Core.DTO;
//using invoice.Core.DTO.Payment;
//using invoice.Core.DTO.PaymentResponse;
//using invoice.Core.Entities;
//using invoice.Core.Enums;
//using invoice.Core.Interfaces.Services;
//using invoice.Repo;
//using Microsoft.Extensions.Options;
//using System.Text;
//using System.Text.Json;

//namespace invoice.Services.Payments.Edfa
//{
//    public class EdfaPaymentService : PaymentGatewayBase, IPaymentGateway
//    {
//        private readonly EdfaOptions _options;
//        private readonly IHttpClientFactory _httpClientFactory;
//        private readonly IRepository<Core.Entities.Invoice> _invoiceRepo;
//        private readonly IRepository<ApplicationUser> _userRepo;
//        private readonly decimal _defaultCommissionRate;

//        public override PaymentType PaymentType => PaymentType.Edfa;

//        public EdfaPaymentService(
//            IConfiguration configuration,
//            IOptions<EdfaOptions> options,
//            IHttpClientFactory httpClientFactory,
//            IRepository<Core.Entities.Invoice> invoiceRepo = null,
//            IRepository<ApplicationUser> userRepo = null)
//            : base(configuration)
//        {
//            _options = options.Value;
//            _httpClientFactory = httpClientFactory;
//            _invoiceRepo = invoiceRepo;
//            _userRepo = userRepo;

//            var cfgRate = Configuration["Payments:DefaultCommissionPercent"];
//            _defaultCommissionRate = decimal.TryParse(cfgRate, out var v) ? v / 100m : 0.1m;
//        }

//        public override async Task<GeneralResponse<PaymentSessionResponse>> CreatePaymentSessionAsync(PaymentCreateDTO dto)
//        {
//            try
//            {
//                var validationResult = ValidatePaymentRequest(dto);
//                if (!validationResult.Success)
//                    return CreateErrorResponse<PaymentSessionResponse>(validationResult.Message);

//                Core.Entities.Invoice? invoice = null;
//                if (!string.IsNullOrEmpty(dto.InvoiceId) && _invoiceRepo != null)
//                {
//                    invoice = await _invoiceRepo.GetByIdAsync(dto.InvoiceId);
//                }

//                var currency = (dto.Currency ?? "SAR").ToUpperInvariant();
//                if (currency.Length != 3)
//                    currency = "SAR";

//                var amount = Math.Round(dto.Cost, 2);
//                if (amount < 1.00m)
//                    return CreateErrorResponse<PaymentSessionResponse>($"Payment amount too small. Minimum amount is 1.00 {currency}");

//                var domain = NormalizeDomain(Domain);

//                var sellerEdfaAccountId = GetSellerEdfaAccountId(dto, invoice);
//                var isP2P = !string.IsNullOrEmpty(sellerEdfaAccountId);

//                var requestData = new
//                {
//                    amount = amount,
//                    currency = currency,
//                    order_id = dto.InvoiceId,
//                    customer_email = dto.ClientEmail,
//                    customer_name = "Customer",
//                    description = TruncateString(dto.Description, 255),
//                    success_url = $"{domain}payments/success?session_id={{order_id}}&invoice={Uri.EscapeDataString(dto.InvoiceId)}",
//                    cancel_url = $"{domain}payments/cancel?invoice={Uri.EscapeDataString(dto.InvoiceId)}",
//                    callback_url = $"{domain}api/webhooks/edfa",
//                };

//                var client = _httpClientFactory.CreateClient("Edfa");
//                var json = JsonSerializer.Serialize(requestData, new JsonSerializerOptions
//                {
//                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase
//                });

//                var request = new HttpRequestMessage(HttpMethod.Post, "v1/payment-links")
//                {
//                    Content = new StringContent(json, Encoding.UTF8, "application/json")
//                };

//                request.Headers.Add("Authorization", $"Bearer {_options.ApiKey}");
//                request.Headers.Add("X-Merchant-Id", _options.MerchantId);

//                // For P2P payments, add seller account header if available
//                if (isP2P && !string.IsNullOrEmpty(sellerEdfaAccountId))
//                {
//                    request.Headers.Add("X-Seller-Account", sellerEdfaAccountId);
//                }

//                var response = await client.SendAsync(request);
//                var responseContent = await response.Content.ReadAsStringAsync();

//                if (!response.IsSuccessStatusCode)
//                {
//                    var error = await ParseErrorResponse(responseContent);
//                    return CreateErrorResponse<PaymentSessionResponse>($"Edfa API error: {error}");
//                }

//                var edfaResponse = JsonSerializer.Deserialize<EdfaPaymentLinkResponse>(responseContent);

//                if (edfaResponse?.Success != true || string.IsNullOrEmpty(edfaResponse.Data?.PaymentUrl))
//                {
//                    return CreateErrorResponse<PaymentSessionResponse>("Failed to create payment link with Edfa");
//                }

//                var responseData = new PaymentSessionResponse
//                {
//                    SessionId = edfaResponse.Data.OrderId ?? dto.InvoiceId,
//                    PaymentUrl = edfaResponse.Data.PaymentUrl,
//                    ExpiresAt = DateTime.UtcNow.AddHours(24),
//                    PaymentType = PaymentType.Edfa,
//                    PaymentStatus = PaymentStatus.Pending,
//                    InvoiceId = dto.InvoiceId,
//                    Amount = dto.Cost,
//                    Currency = currency,
//                    RawResponse = responseContent
//                };

//                return CreateSuccessResponse(
//                    responseData.SessionId,
//                    responseData.PaymentUrl,
//                    dto,
//                    responseData.ExpiresAt,
//                    responseContent
//                );
//            }
//            catch (HttpRequestException ex)
//            {
//                return CreateErrorResponse<PaymentSessionResponse>($"Network error connecting to Edfa: {ex.Message}");
//            }
//            catch (Exception ex)
//            {
//                return CreateErrorResponse<PaymentSessionResponse>($"An unexpected error occurred while creating the payment session: {ex.Message}");
//            }
//        }

//        public override async Task<GeneralResponse<bool>> CancelPaymentAsync(string paymentId)
//        {
//            try
//            {
//                var client = _httpClientFactory.CreateClient("Edfa");
//                var request = new HttpRequestMessage(HttpMethod.Post, $"v1/payments/{paymentId}/cancel");

//                request.Headers.Add("Authorization", $"Bearer {_options.ApiKey}");
//                request.Headers.Add("X-Merchant-Id", _options.MerchantId);

//                var response = await client.SendAsync(request);
//                var responseContent = await response.Content.ReadAsStringAsync();

//                if (!response.IsSuccessStatusCode)
//                {
//                    var error = await ParseErrorResponse(responseContent);
//                    return CreateErrorResponse<bool>($"Failed to cancel payment: {error}");
//                }

//                var cancelResponse = JsonSerializer.Deserialize<EdfaBaseResponse>(responseContent);

//                if (cancelResponse?.Success == true)
//                {
//                    return new GeneralResponse<bool>(true, "Payment cancelled successfully", true);
//                }

//                return CreateErrorResponse<bool>("Failed to cancel payment with Edfa");
//            }
//            catch (Exception ex)
//            {
//                return CreateErrorResponse<bool>($"Failed to cancel payment: {ex.Message}");
//            }
//        }

//        public override async Task<GeneralResponse<PaymentStatusResponse>> GetPaymentStatusAsync(string paymentId)
//        {
//            try
//            {
//                var client = _httpClientFactory.CreateClient("Edfa");
//                var request = new HttpRequestMessage(HttpMethod.Get, $"v1/payments/{paymentId}");

//                request.Headers.Add("Authorization", $"Bearer {_options.ApiKey}");
//                request.Headers.Add("X-Merchant-Id", _options.MerchantId);

//                var response = await client.SendAsync(request);
//                var responseContent = await response.Content.ReadAsStringAsync();

//                if (!response.IsSuccessStatusCode)
//                {
//                    var error = await ParseErrorResponse(responseContent);
//                    return CreateErrorResponse<PaymentStatusResponse>($"Failed to get payment status: {error}");
//                }

//                var statusResponse = JsonSerializer.Deserialize<EdfaPaymentStatusResponse>(responseContent);

//                if (statusResponse?.Success != true || statusResponse.Data == null)
//                {
//                    return CreateErrorResponse<PaymentStatusResponse>("Failed to get payment status from Edfa");
//                }

//                var status = MapEdfaStatus(statusResponse.Data.Status);
//                var amount = statusResponse.Data.Amount ?? 0m;

//                var paymentStatusResponse = new PaymentStatusResponse
//                {
//                    PaymentId = paymentId,
//                    Status = status,
//                    Amount = amount,
//                    Currency = statusResponse.Data.Currency ?? "SAR",
//                    LastUpdated = DateTime.UtcNow,
//                    RawResponse = responseContent,
//                    Metadata = statusResponse.Data.Metadata?.ToDictionary(kvp => kvp.Key, kvp => kvp.Value?.ToString() ?? "")
//                };

//                return new GeneralResponse<PaymentStatusResponse>(true, "Status retrieved successfully", paymentStatusResponse);
//            }
//            catch (Exception ex)
//            {
//                return CreateErrorResponse<PaymentStatusResponse>($"Failed to get payment status: {ex.Message}");
//            }
//        }


//        #region P2P and Commission Methods

//        private string? GetSellerEdfaAccountId(PaymentCreateDTO dto, Core.Entities.Invoice? invoice = null)
//        {
//            if (dto.Metadata != null && dto.Metadata.TryGetValue("seller_edfa_account_id", out var accountId) && !string.IsNullOrWhiteSpace(accountId))
//                return accountId;

//            if (invoice?.User?.EdfaAccountId != null)
//                return invoice?.User?.EdfaAccountId;

//            if (invoice != null && !string.IsNullOrEmpty(invoice.UserId) && _userRepo != null)
//            {
//                var user = _userRepo.GetByIdAsync(invoice.UserId).Result;
//                if (user?.EdfaAccountId != null)
//                    return user?.EdfaAccountId;
//            }

//            return null;
//        }

//        private async Task<string?> GetSellerIdFromInvoice(Core.Entities.Invoice invoice)
//        {
//            if (!string.IsNullOrEmpty(invoice.UserId))
//                return invoice.UserId;

//            if (!string.IsNullOrEmpty(invoice.ClientId))
//                return invoice.ClientId;

//            return null;
//        }


//        #endregion

//        #region Helper Methods

//        private async Task<string> ParseErrorResponse(string responseContent)
//        {
//            try
//            {
//                var errorResponse = JsonSerializer.Deserialize<EdfaErrorResponse>(responseContent);
//                return errorResponse?.Message ?? "Unknown error from Edfa";
//            }
//            catch
//            {
//                return "Unable to parse error response from Edfa";
//            }
//        }

//        private bool VerifyWebhookSignature(string payload, string signature)
//        {
//            if (string.IsNullOrEmpty(_options.WebhookSecret))
//                return true;

//            // Implement HMAC verification based on Edfa's documentation
//            // This is a placeholder - implement actual verification
//            return true;
//        }

//        private PaymentStatus MapEdfaStatus(string edfaStatus)
//        {
//            return edfaStatus?.ToLowerInvariant() switch
//            {
//                "completed" or "succeeded" or "paid" => PaymentStatus.Completed,
//                "pending" or "processing" => PaymentStatus.Pending,
//                "failed" or "declined" => PaymentStatus.Failed,
//                "cancelled" or "expired" => PaymentStatus.Cancelled,
//                "refunded" => PaymentStatus.Refunded,
//                _ => PaymentStatus.Pending
//            };
//        }

//        #endregion
//    }
//}