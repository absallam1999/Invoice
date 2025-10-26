using invoice.Core.DTO;
using invoice.Core.DTO.Payment;
using invoice.Core.DTO.PaymentResponse;
using invoice.Core.Enums;
using invoice.Core.Interfaces.Services;
using invoice.Helpers;
using invoice.Repo;
using Microsoft.Extensions.Options;
using System.Text;
using System.Text.Json;

namespace invoice.Services.Payments.TabPayments
{
    public class TabPaymentsService : PaymentGatewayBase, IPaymentGateway
    {
        private readonly IRepository<Core.Entities.Invoice> _invoiceRepo;
        private readonly IInvoiceService _invoiceService;
        private readonly TabPaymentsOptions _options;
        private readonly HttpClient _httpClient;

        public override PaymentType PaymentType => PaymentType.TabPayments;

        public TabPaymentsService(
            IConfiguration configuration,
            IOptions<TabPaymentsOptions> options,
            IHttpClientFactory httpClientFactory,
            IInvoiceService invoiceService = null,
            IRepository<Core.Entities.Invoice> invoiceRepo = null)
            : base(configuration)
        {
            _options = options.Value;
            _httpClient = httpClientFactory.CreateClient("TabPayments");
            _invoiceRepo = invoiceRepo;
            _invoiceService = invoiceService;

            // Configure HTTP client
            _httpClient.BaseAddress = new Uri(_options.BaseUrl);
            _httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {_options.SecretKey}");
            _httpClient.DefaultRequestHeaders.Add("Accept", "application/json");
        }

        public override async Task<GeneralResponse<PaymentSessionResponse>> CreatePaymentSessionAsync(PaymentCreateDTO dto)
        {
            try
            {
                var validationResult = ValidatePaymentRequest(dto);
                if (!validationResult.Success)
                    return new GeneralResponse<PaymentSessionResponse>(false, validationResult.Message);

                Core.Entities.Invoice? invoice = null;
                if (!string.IsNullOrEmpty(dto.InvoiceId) && _invoiceRepo != null)
                {
                    invoice = await _invoiceRepo.GetByIdAsync(dto.InvoiceId);
                }

                var currency = (dto.Currency ?? "SAR").ToUpper();

                var amountInHalalas = (int)(dto.Cost * 100);
                if (amountInHalalas < 100) 
                    return new GeneralResponse<PaymentSessionResponse>(false, $"Payment amount too small. Minimum amount is 1 {currency}");

                var domain = NormalizeDomain(Domain);

                // For Tab Payments P2P
                var isP2P = !string.IsNullOrEmpty(GetSellerTabAccountId(dto, invoice));

                var request = new
                {
                    amount = amountInHalalas,
                    currency = currency,
                    order_id = dto.InvoiceId,
                    customer = new
                    {
                        email = dto.ClientEmail,
                        name = "Customer",
                        phone = dto.Metadata?.GetValueOrDefault("customer_phone") ?? string.Empty
                    },
                    items = new[]
                    {
                        new
                        {
                            name = TruncateString(dto.Name, 100),
                            description = TruncateString(dto.Description, 500),
                            amount = amountInHalalas,
                            quantity = 1
                        }
                    },
                    return_url = $"{domain}payments/success?invoice={Uri.EscapeDataString(dto.InvoiceId)}",
                    cancel_url = $"{domain}payments/cancel?invoice={Uri.EscapeDataString(dto.InvoiceId)}",
                    callback_url = $"{domain}api/webhooks/tabpayments",
                    metadata = CreateTabMetadata(dto, amountInHalalas, isP2P),
                    expires_in = GetSaudiTime.Now().AddDays(7)
                };

                var content = new StringContent(JsonSerializer.Serialize(request), Encoding.UTF8, "application/json");
                var response = await _httpClient.PostAsync("/v2/checkout/sessions", content);

                if (!response.IsSuccessStatusCode)
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    return new GeneralResponse<PaymentSessionResponse>(false, $"TabPayments API error: {errorContent}");
                }

                var responseContent = await response.Content.ReadAsStringAsync();
                var tabResponse = JsonSerializer.Deserialize<TabPaymentSessionResponse>(responseContent);

                var responseData = new PaymentSessionResponse
                {
                    SessionId = tabResponse.Id,
                    PaymentUrl = tabResponse.CheckoutUrl,
                    ExpiresAt = DateTime.UtcNow.AddHours(24),
                    PaymentType = PaymentType.TabPayments,
                    PaymentStatus = PaymentStatus.Pending,
                    InvoiceId = dto.InvoiceId,
                    Amount = dto.Cost,
                    Currency = currency,
                    RawResponse = responseContent
                };

                return new GeneralResponse<PaymentSessionResponse>(true, "TabPayments session created successfully", responseData);
            }
            catch (Exception ex)
            {
                return new GeneralResponse<PaymentSessionResponse>(false, $"Failed to create TabPayments session: {ex.Message}");
            }
        }

        public override async Task<GeneralResponse<bool>> CancelPaymentAsync(string paymentId)
        {
            try
            {
                var response = await _httpClient.PostAsync($"/v2/payments/{paymentId}/cancel", null);

                if (!response.IsSuccessStatusCode)
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    return new GeneralResponse<bool>(false, $"Failed to cancel payment: {errorContent}");
                }

                return new GeneralResponse<bool>(true, "Payment cancelled successfully", true);
            }
            catch (Exception ex)
            {
                return new GeneralResponse<bool>(false, $"Failed to cancel payment: {ex.Message}");
            }
        }

        public override async Task<GeneralResponse<PaymentStatusResponse>> GetPaymentStatusAsync(string paymentId)
        {
            try
            {
                var response = await _httpClient.GetAsync($"/v2/payments/{paymentId}");

                if (!response.IsSuccessStatusCode)
                {
                    return new GeneralResponse<PaymentStatusResponse>(false, "Failed to retrieve payment status");
                }

                var responseContent = await response.Content.ReadAsStringAsync();
                var payment = JsonSerializer.Deserialize<TabPaymentResponse>(responseContent);

                var status = MapTabStatus(payment.Status);

                var statusResponse = new PaymentStatusResponse
                {
                    PaymentId = paymentId,
                    Status = status,
                    LastUpdated = DateTime.UtcNow,
                    RawResponse = responseContent
                };

                return new GeneralResponse<PaymentStatusResponse>(true, "Status retrieved successfully", statusResponse);
            }
            catch (Exception ex)
            {
                return new GeneralResponse<PaymentStatusResponse>(false, $"Failed to get payment status: {ex.Message}");
            }
        }

        #region Tab Payments Specific Methods

        private Dictionary<string, object> CreateTabMetadata(PaymentCreateDTO dto, int sellerAmount, bool isP2P)
        {
            var metadata = new Dictionary<string, object>
            {
                ["invoice_id"] = dto.InvoiceId,
                ["seller_amount"] = sellerAmount,
                ["is_p2p"] = isP2P
            };

            // additional metadata
            if (dto.Metadata != null)
            {
                foreach (var item in dto.Metadata)
                {
                    metadata[item.Key] = item.Value;
                }
            }

            return metadata;
        }

        private string? GetSellerTabAccountId(PaymentCreateDTO dto, Core.Entities.Invoice? invoice = null)
        {
            // Tab Payments seller account identifiers
            if (dto.Metadata != null && dto.Metadata.TryGetValue("seller_tab_account_id", out var accountId) && !string.IsNullOrWhiteSpace(accountId))
                return accountId;

            if (invoice?.User?.TabAccountId != null)
                return invoice?.User?.TabAccountId;

            return null;
        }

        private PaymentStatus MapTabStatus(string tabStatus)
        {
            return tabStatus?.ToLower() switch
            {
                "captured" or "succeeded" => PaymentStatus.Completed,
                "pending" or "authorized" => PaymentStatus.Pending,
                "failed" or "declined" => PaymentStatus.Failed,
                "cancelled" => PaymentStatus.Cancelled,
                "refunded" => PaymentStatus.Refunded,
                _ => PaymentStatus.Pending
            };
        }

        #endregion
    }
}