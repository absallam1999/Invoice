using invoice.Core.DTO;
using invoice.Core.DTO.Payment;
using invoice.Core.DTO.PaymentResponse;
using invoice.Core.Enums;
using invoice.Core.Interfaces.Services;
using Microsoft.Extensions.Options;
using System.Text;
using System.Text.Json;

namespace invoice.Services.Payments.Paypal
{
    public class PayPalPaymentService : PaymentGatewayBase, IPaymentGateway
    {
        private readonly PayPalOptions _options;
        private readonly HttpClient _httpClient;

        public override PaymentType PaymentType => PaymentType.PayPal;

        public PayPalPaymentService(
            ILogger<PayPalPaymentService> logger,
            IConfiguration configuration,
            IOptions<PayPalOptions> options,
            IHttpClientFactory httpClientFactory)
            : base(configuration)
        {
            _options = options.Value;
            _httpClient = httpClientFactory.CreateClient("PayPal");
        }

        public override async Task<GeneralResponse<PaymentSessionResponse>> CreatePaymentSessionAsync(PaymentCreateDTO dto)
        {
            try
            {
                var validationResult = ValidatePaymentRequest(dto);
                if (!validationResult.Success)
                    return validationResult;

                var accessToken = await GetAccessTokenAsync();
                if (string.IsNullOrEmpty(accessToken))
                {
                    return CreateErrorResponse<PaymentSessionResponse>("Failed to authenticate with PayPal");
                }

                var order = await CreateOrderAsync(dto, accessToken);
                if (order == null)
                {
                    return CreateErrorResponse<PaymentSessionResponse>("Failed to create PayPal order");
                }

                var approvalUrl = order.Links
                    .FirstOrDefault(link => link.Rel == "approve")
                    ?.Href;

                if (string.IsNullOrEmpty(approvalUrl))
                {
                    return CreateErrorResponse<PaymentSessionResponse>("PayPal approval URL not found");
                }

                return CreateSuccessResponse(
                    order.Id,
                    approvalUrl,
                    dto,
                    DateTime.UtcNow.AddHours(24),
                    JsonSerializer.Serialize(order)
                );
            }
            catch (Exception ex)
            {
                return CreateErrorResponse<PaymentSessionResponse>("Failed to create PayPal payment session");
            }
        }

        private async Task<string> GetAccessTokenAsync()
        {
            try
            {
                var authString = Convert.ToBase64String(Encoding.UTF8.GetBytes($"{_options.ClientId}:{_options.ClientSecret}"));

                var request = new HttpRequestMessage(HttpMethod.Post, $"{_options.BaseUrl}/v1/oauth2/token");
                request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Basic", authString);
                request.Content = new FormUrlEncodedContent(new[]
                {
                    new KeyValuePair<string, string>("grant_type", "client_credentials")
                });

                var response = await _httpClient.SendAsync(request);
                response.EnsureSuccessStatusCode();

                var content = await response.Content.ReadAsStringAsync();
                var authResult = JsonSerializer.Deserialize<PayPalAuthResponse>(content);

                return authResult?.AccessToken;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        private async Task<PayPalOrderResponse> CreateOrderAsync(PaymentCreateDTO dto, string accessToken)
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
                                currency_code = dto.Currency.ToUpper(),
                                value = dto.Cost.ToString("F2")
                            },
                            description = TruncateString(dto.Description, 127),
                            custom_id = dto.InvoiceId,
                            invoice_id = dto.InvoiceId
                        }
                    },
                    application_context = new
                    {
                        return_url = $"{domain}payments/paypal/success?invoice={dto.InvoiceId}",
                        cancel_url = $"{domain}payments/paypal/cancel?invoice={dto.InvoiceId}",
                        brand_name = Configuration["AppSettings:CompanyName"] ?? "Your Company"
                    }
                };

                var request = new HttpRequestMessage(HttpMethod.Post, $"{_options.BaseUrl}/v2/checkout/orders");
                request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", accessToken);
                request.Content = new StringContent(
                    JsonSerializer.Serialize(orderData),
                    Encoding.UTF8,
                    "application/json"
                );

                var response = await _httpClient.SendAsync(request);
                response.EnsureSuccessStatusCode();

                var content = await response.Content.ReadAsStringAsync();
                return JsonSerializer.Deserialize<PayPalOrderResponse>(content);
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public override Task<GeneralResponse<bool>> CancelPaymentAsync(string paymentId)
        {
            // PayPal cancellation logic
            return Task.FromResult(new GeneralResponse<bool> { Success = true, Data = true });
        }

        public override Task<GeneralResponse<bool>> RefundPaymentAsync(string paymentId)
        {
            // PayPal refund logic
            return Task.FromResult(new GeneralResponse<bool> { Success = true, Data = true });
        }

        public override Task<GeneralResponse<PaymentStatusResponse>> GetPaymentStatusAsync(string paymentId)
        {
            // PayPal status check logic
            return Task.FromResult(new GeneralResponse<PaymentStatusResponse>
            {
                Success = true,
                Data = new PaymentStatusResponse { PaymentId = paymentId, Status = PaymentStatus.Completed }
            });
        }

        public override Task<GeneralResponse<PaymentWebhookResponse>> ProcessWebhookAsync(string payload, string signature)
        {
            // PayPal webhook processing
            return Task.FromResult(new GeneralResponse<PaymentWebhookResponse>
            {
                Success = true,
                Data = new PaymentWebhookResponse { Processed = true, EventType = "paypal_webhook" }
            });
        }
    }
}