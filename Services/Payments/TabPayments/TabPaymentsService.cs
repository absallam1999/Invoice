using invoice.Core.DTO;
using invoice.Core.DTO.Payment;
using invoice.Core.DTO.PaymentResponse;
using invoice.Core.DTO.PaymentResponse.TapPayments;
using invoice.Core.Enums;
using invoice.Core.Interfaces.Services;
using invoice.Repo;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using RestSharp;
using System.Numerics;



namespace invoice.Services.Payments.TabPayments
{
    public class TabPaymentsService : PaymentGatewayBase, IPaymentGateway
    {
        private readonly IRepository<Core.Entities.Invoice> _invoiceRepo;
        private readonly TabPaymentsOptions _options;
        private readonly HttpClient _httpClient;
        private readonly string _secretKey;

        public override PaymentType PaymentType => PaymentType.TabPayments;

        public TabPaymentsService(
            IConfiguration configuration,
            IOptions<TabPaymentsOptions> options,
            IHttpClientFactory httpClientFactory,
            IRepository<Core.Entities.Invoice> invoiceRepo = null)
            : base(configuration)
        {
            _options = options.Value;
            _invoiceRepo = invoiceRepo;
            _secretKey = configuration["TapSettings:SecretKey"];


            if (string.IsNullOrWhiteSpace(_options.BaseUrl))
                _options.BaseUrl = "https://api.tap.company";

            if (string.IsNullOrWhiteSpace(_options.SecretKey))
                throw new ArgumentException("Tap SecretKey is required");

            _httpClient = httpClientFactory.CreateClient("TabPayments");

            try { _httpClient.BaseAddress = new Uri(_options.BaseUrl); }
            catch { _httpClient.BaseAddress = null; }

            _httpClient.DefaultRequestHeaders.Clear();
            _httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {_options.SecretKey}");
            _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            _httpClient.DefaultRequestHeaders.UserAgent.ParseAdd("InvoiceApp/1.0");
        }

        #region  Tap_Onboardin




        public async Task<string?> CreateMerchantAccountAsync(TapUserDto dto, string userId)
        {
           
            var body = new
            {
                scope = "merchant",
                data = new[] { "operation", "brand", "entity", "merchant" },

                lead = new
                {
                    email = dto.Email,
                    phone = new
                    {
                        country_code = dto.CountryCode,
                        number = dto.PhoneNumber
                    },
                    business_name = dto.BusinessName,
                    country = dto.Country,
                    board = new
                    {
                        redirect = new
                        {
                            success_url = "https://yourapp.com/onboarding/success",
                            failure_url = "https://yourapp.com/onboarding/fail"
                        },
                        post = new
                        {
                            url = $"https://myinvoice.runasp.net/api/payments/onboarding-success?userId={userId}"
                        },
                        webhook = new
                        {
                            url = $"https://myinvoice.runasp.net/api/payments/onboarding-webhook"
                        }
                    },
                    @interface = new
                    {
                        theme = "default",
                        lang = "en",
                        mode = "light"
                    }
                }
            };

            var json = JsonConvert.SerializeObject(body);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync("/v3/connect", content);
            var responseContent = await response.Content.ReadAsStringAsync();
            Console.WriteLine("Tap Response: " + responseContent);

            if (!response.IsSuccessStatusCode)
                throw new Exception($"Error: {response.StatusCode} - {responseContent}");

            var data = JObject.Parse(responseContent);
            return data["connect"]?["url"]?.ToString();
        }

        #endregion

        public override async Task<GeneralResponse<PaymentSessionResponse>> CreatePaymentSessionAsync(PaymentCreateDTO dto)
        {
            if (!string.IsNullOrWhiteSpace(_options.BaseUrl) && await TestDomain(_options.BaseUrl))
            {
                var result = await TryCreateSessionWithDomain(dto, _options.BaseUrl);
                if (result.Success) return result;
            }

            var fallbackDomains = new[] { "https://api.tap.company", "https://api.tap.company/" };
            foreach (var domain in fallbackDomains)
            {
                var res = await TryCreateSessionWithDomain(dto, domain);
                if (res.Success) return res;
            }

            return await HandleTabPaymentsConfigurationError(dto);
        }

        private async Task<bool> TestDomain(string domain)
        {
            try
            {
                using var client = new HttpClient { Timeout = TimeSpan.FromSeconds(Math.Max(5, _options.TimeoutSeconds)) };
                var resp = await client.GetAsync(NormalizeDomain(domain));
                return resp.IsSuccessStatusCode;
            }
            catch { return false; }
        }

        private static string NormalizeDomain(string baseUrl)
        {
            if (string.IsNullOrWhiteSpace(baseUrl)) return string.Empty;
            var u = baseUrl.Trim().TrimEnd('/');
            if (!u.StartsWith("http", StringComparison.OrdinalIgnoreCase))
                u = "https://" + u;
            return u;
        }

        private async Task<GeneralResponse<PaymentSessionResponse>> HandleTabPaymentsConfigurationError(PaymentCreateDTO dto)
        {
            return new GeneralResponse<PaymentSessionResponse>(
                false,
                $"Tap service unavailable. Merchant ID: {_options.MerchantId}, Test Mode: {_options.TestMode}"
            );
        }

        private async Task<GeneralResponse<PaymentSessionResponse>> TryCreateSessionWithDomain(PaymentCreateDTO dto, string baseUrl)
        {
            var jsonOptions = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
            try
            {
                var validation = ValidatePaymentRequest(dto);
                if (!validation.Success) return validation;

                var amountInCents = (long)Math.Round(dto.Cost * 100M);
                if (amountInCents < 1)
                    return new GeneralResponse<PaymentSessionResponse>(false, "Payment amount too small");

                var domain = NormalizeDomain(baseUrl);

                // --- 🟢 Detect P2P mode ---
                Core.Entities.Invoice? invoice = null;
                if (!string.IsNullOrEmpty(dto.InvoiceId))
                    invoice = await _invoiceRepo.GetByIdAsync(dto.InvoiceId);

                var sellerAccountId = GetSellerTabAccountId(dto, invoice);
                var isP2P = !string.IsNullOrEmpty(sellerAccountId);

                var payload = new Dictionary<string, object?>
                {
                    ["amount"] = amountInCents,
                    ["currency"] = (dto.Currency ?? "SAR").ToUpperInvariant(),
                    ["merchant"] = new Dictionary<string, object?> { ["id"] = _options.MerchantId },
                    ["source"] = new Dictionary<string, object?> { ["id"] = "src_all" },
                    ["redirect"] = new Dictionary<string, object?> { ["url"] = $"{domain}/payments/success?invoice={Uri.EscapeDataString(dto.InvoiceId)}" },
                    ["post"] = new Dictionary<string, object?> { ["url"] = $"{domain}/api/webhooks/tap" },
                    ["customer"] = new Dictionary<string, object?>
                    {
                        ["first_name"] = dto.ClientId ?? "Customer",
                        ["email"] = dto.ClientEmail ?? string.Empty
                    },
                    ["metadata"] = CreateTabMetadata(dto, amountInCents, isP2P)
                };

                // --- 🟢 Add P2P destination if seller found ---
                if (isP2P)
                {
                    payload["destination"] = new Dictionary<string, object?>
                    {
                        ["merchant"] = new Dictionary<string, object?> { ["id"] = sellerAccountId },
                        ["amount"] = amountInCents
                    };
                }

                var content = new StringContent(System.Text.Json.JsonSerializer.Serialize(payload, jsonOptions), Encoding.UTF8, "application/json");
                var endpoints = new[] { "/v2/charges/", "/v2/charges" };

                foreach (var ep in endpoints)
                {
                    var fullUrl = domain + ep;
                    using var req = new HttpRequestMessage(HttpMethod.Post, fullUrl) { Content = content };
                    req.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _options.SecretKey);

                    using var resp = await _httpClient.SendAsync(req, HttpCompletionOption.ResponseHeadersRead);
                    var body = await resp.Content.ReadAsStringAsync();

                    if (!resp.IsSuccessStatusCode)
                    {
                        continue;
                    }

                    string? sessionId = null;
                    string? paymentUrl = null;
                    DateTime? expiresAt = null;

                    try
                    {
                        using var doc = JsonDocument.Parse(body);
                        var root = doc.RootElement;
                        sessionId = root.GetProperty("id").GetString();
                        if (root.TryGetProperty("transaction", out var tx))
                        {
                            paymentUrl = tx.GetProperty("url").GetString();
                        }
                        else if (root.TryGetProperty("url", out var urlProp))
                        {
                            paymentUrl = urlProp.GetString();
                        }
                        if (root.TryGetProperty("expires_at", out var expProp))
                        {
                            DateTime.TryParse(expProp.GetString(), out var dt);
                            expiresAt = dt.ToUniversalTime();
                        }
                    }
                    catch { }

                    var responseData = new PaymentSessionResponse
                    {
                        SessionId = sessionId ?? Guid.NewGuid().ToString(),
                        PaymentUrl = paymentUrl ?? string.Empty,
                        ExpiresAt = expiresAt ?? DateTime.UtcNow.AddHours(1),
                        PaymentType = PaymentType.TabPayments,
                        PaymentStatus = PaymentStatus.Pending,
                        InvoiceId = dto.InvoiceId,
                        Amount = dto.Cost,
                        Currency = (dto.Currency ?? "SAR").ToUpperInvariant(),
                        RawResponse = body
                    };

                    var msg = string.IsNullOrEmpty(paymentUrl)
                        ? "Charge created but no redirect URL returned. Check RawResponse."
                        : "Payment session created successfully";

                    return new GeneralResponse<PaymentSessionResponse>(true, msg, responseData);
                }

                return new GeneralResponse<PaymentSessionResponse>(false, "No working Tap endpoint found for this domain");
            }
            catch (Exception ex)
            {
                return new GeneralResponse<PaymentSessionResponse>(false, $"Domain unreachable: {ex.Message}");
            }
        }

        private Dictionary<string, object> CreateTabMetadata(PaymentCreateDTO dto, long sellerAmount, bool isP2P)
        {
            var metadata = new Dictionary<string, object>
            {
                ["invoice_id"] = dto.InvoiceId,
                ["seller_amount"] = sellerAmount,
                ["is_p2p"] = isP2P,
                ["merchant_id"] = _options.MerchantId,
                ["test_mode"] = _options.TestMode
            };

            if (dto.Metadata != null)
                foreach (var item in dto.Metadata)
                    metadata[item.Key] = item.Value ?? string.Empty;

            return metadata;
        }

        private string? GetSellerTabAccountId(PaymentCreateDTO dto, Core.Entities.Invoice? invoice = null)
        {
            if (dto.Metadata != null && dto.Metadata.TryGetValue("seller_tab_account_id", out var accountId) && !string.IsNullOrWhiteSpace(accountId))
                return accountId;

            if (!string.IsNullOrWhiteSpace(invoice?.User?.TabAccountId))
                return invoice.User.TabAccountId;

            return null;
        }

        public override async Task<GeneralResponse<bool>> CancelPaymentAsync(string paymentId)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(paymentId)) return new GeneralResponse<bool>(false, "PaymentId required");

                var baseUrl = NormalizeDomain(_options.BaseUrl);
                var endpoint = $"{baseUrl}/v2/charges/{Uri.EscapeDataString(paymentId)}/refund";
                using var req = new HttpRequestMessage(HttpMethod.Post, endpoint);
                req.Headers.Remove("Authorization");
                req.Headers.Add("Authorization", $"Bearer {_options.SecretKey}");
                req.Content = new StringContent("{}", Encoding.UTF8, "application/json");

                using var resp = await _httpClient.SendAsync(req);
                var body = await resp.Content.ReadAsStringAsync();

                if (!resp.IsSuccessStatusCode)
                {
                    return new GeneralResponse<bool>(false, $"Failed to cancel/refund payment: {body}");
                }

                return new GeneralResponse<bool>(true, "Payment cancelled/refunded successfully", true);
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
                if (string.IsNullOrWhiteSpace(paymentId)) return new GeneralResponse<PaymentStatusResponse>(false, "PaymentId required");

                var baseUrl = NormalizeDomain(_options.BaseUrl);
                var endpoint = $"{baseUrl}/v2/charges/{Uri.EscapeDataString(paymentId)}";
                using var req = new HttpRequestMessage(HttpMethod.Get, endpoint);
                req.Headers.Remove("Authorization");
                req.Headers.Add("Authorization", $"Bearer {_options.SecretKey}");

                using var resp = await _httpClient.SendAsync(req);
                var body = await resp.Content.ReadAsStringAsync();

                if (!resp.IsSuccessStatusCode)
                {
                    return new GeneralResponse<PaymentStatusResponse>(false, $"Failed to retrieve payment status: {body}");
                }

                string providerStatus = string.Empty;
                try
                {
                    using var doc = JsonDocument.Parse(body);
                    var root = doc.RootElement;
                    if (root.TryGetProperty("transaction", out var tx) && tx.TryGetProperty("status", out var st))
                        providerStatus = st.GetString() ?? string.Empty;
                    else if (root.TryGetProperty("status", out var s2))
                        providerStatus = s2.GetString() ?? string.Empty;
                }
                catch { }

                var mapped = MapTabStatus(providerStatus);

                var statusResponse = new PaymentStatusResponse
                {
                    PaymentId = paymentId,
                    Status = mapped,
                    LastUpdated = DateTime.UtcNow,
                    RawResponse = body
                };

                return new GeneralResponse<PaymentStatusResponse>(true, "Status retrieved successfully", statusResponse);
            }
            catch (Exception ex)
            {
                return new GeneralResponse<PaymentStatusResponse>(false, $"Failed to get payment status: {ex.Message}");
            }
        }


        private PaymentStatus MapTabStatus(string tabStatus)
        {
            if (string.IsNullOrWhiteSpace(tabStatus)) return PaymentStatus.Pending;

            var normalized = tabStatus.Trim().ToLowerInvariant();
            return normalized switch
            {
                "captured" or "succeeded" or "successful" or "paid" => PaymentStatus.Completed,
                "pending" or "authorized" or "in_progress" => PaymentStatus.Pending,
                "failed" or "declined" or "error" => PaymentStatus.Failed,
                "cancelled" or "canceled" => PaymentStatus.Cancelled,
                "refunded" or "reversed" => PaymentStatus.Refunded,
                _ => PaymentStatus.Pending
            };
        }
    }
}
