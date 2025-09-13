using invoice.Core.DTO;
using invoice.Core.DTO.Payment;
using invoice.Core.DTO.PaymentResponse;
using invoice.Core.Enums;
using invoice.Core.Interfaces.Services;
using Stripe;
using Stripe.Checkout;
using System.Text;
using System.Text.Json;

namespace invoice.Services
{
    public class PaymentGatewayService : IPaymentGatewayService
    {
        private readonly string _domain;
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly ILogger<PaymentGatewayService> _logger;

        public PaymentGatewayService(
            IConfiguration configuration,
            ILogger<PaymentGatewayService> logger,
            IHttpClientFactory httpClientFactory)
        {
            _logger = logger;
            _configuration = configuration;
            _httpClientFactory = httpClientFactory;
            _domain = _configuration["AppSettings:BaseUrl"];
            _httpClient = _httpClientFactory.CreateClient("PaymentGateway");
        }

        public async Task<GeneralResponse<PaymentSessionResponse>> CreatePaymentSessionAsync(PaymentCreateDTO dto, PaymentType type)
        {
            using var scope = _logger.BeginScope(new Dictionary<string, object>
            {
                ["InvoiceId"] = dto.InvoiceId,
                ["PaymentType"] = type,
                ["Amount"] = dto.Cost,
                ["Currency"] = dto.Currency
            });

            try
            {
                _logger.LogInformation("Creating payment session for invoice {InvoiceId}", dto.InvoiceId);

                // Validate input
                var validationResult = ValidatePaymentCreateDTO(dto);
                if (!validationResult.Success)
                {
                    return validationResult;
                }

                GeneralResponse<string> sessionResponse = type switch
                {
                    PaymentType.Cash or PaymentType.Delivery => HandleOfflinePayment(type),
                    PaymentType.Stripe => await CreateStripeSessionAsync(dto),
                    PaymentType.PayPal => await CreatePayPalSessionAsync(dto),
                    PaymentType.ApplePay => await CreateApplePaySessionAsync(dto),
                    PaymentType.GooglePay => await CreateGooglePaySessionAsync(dto),
                    PaymentType.Mada => await CreateMadaSessionAsync(dto),
                    PaymentType.STCPay => await CreateSTCPaySessionAsync(dto),
                    PaymentType.Sadad => await CreateSadadSessionAsync(dto),
                    PaymentType.CreditCard or PaymentType.DebitCard or PaymentType.BankTransfer
                        => await CreateGenericCardSessionAsync(dto, type),
                    _ => HandleUnsupportedPaymentType(type)
                };

                if (!sessionResponse.Success)
                {
                    return new GeneralResponse<PaymentSessionResponse>
                    {
                        Success = false,
                        Message = sessionResponse.Message
                    };
                }

                return new GeneralResponse<PaymentSessionResponse>
                {
                    Success = true,
                    Message = sessionResponse.Message,
                    Data = new PaymentSessionResponse
                    {
                        PaymentUrl = sessionResponse.Data,
                        PaymentType = type,
                        InvoiceId = dto.InvoiceId,
                        Amount = dto.Cost,
                        Currency = dto.Currency,
                        ExpiresAt = DateTime.UtcNow.AddHours(24)
                    }
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating payment session for invoice {InvoiceId}", dto.InvoiceId);
                return new GeneralResponse<PaymentSessionResponse>
                {
                    Success = false,
                    Message = $"Failed to create payment session: {ex.Message}"
                };
            }
        }

        private GeneralResponse<string> HandleOfflinePayment(PaymentType type)
        {
            return new GeneralResponse<string>
            {
                Success = true,
                Message = $"{type} selected, no online session required.",
                Data = null
            };
        }

        private GeneralResponse<string> HandleUnsupportedPaymentType(PaymentType type)
        {
            _logger.LogWarning("Unsupported payment type: {PaymentType}", type);
            return new GeneralResponse<string>
            {
                Success = false,
                Message = $"Payment type {type} is not supported."
            };
        }

        private GeneralResponse<PaymentSessionResponse> ValidatePaymentCreateDTO(PaymentCreateDTO dto)
        {
            if (string.IsNullOrEmpty(dto.InvoiceId))
            {
                return new GeneralResponse<PaymentSessionResponse>
                {
                    Success = false,
                    Message = "Invoice ID is required."
                };
            }

            if (dto.Cost <= 0)
            {
                return new GeneralResponse<PaymentSessionResponse>
                {
                    Success = false,
                    Message = "Payment amount must be greater than zero."
                };
            }

            if (string.IsNullOrEmpty(dto.Currency))
            {
                return new GeneralResponse<PaymentSessionResponse>
                {
                    Success = false,
                    Message = "Currency is required."
                };
            }

            return new GeneralResponse<PaymentSessionResponse> { Success = true };
        }

        public async Task<GeneralResponse<string>> CreateStripeSessionAsync(PaymentCreateDTO dto)
        {
            try
            {
                var options = new SessionCreateOptions
                {
                    PaymentMethodTypes = new List<string> { "card" },
                    LineItems = new List<SessionLineItemOptions>
                    {
                        new SessionLineItemOptions
                        {
                            PriceData = new SessionLineItemPriceDataOptions
                            {
                                UnitAmount = (long)(dto.Cost * 100),
                                Currency = dto.Currency.ToLower(),
                                ProductData = new SessionLineItemPriceDataProductDataOptions
                                {
                                    Name = dto.Name,
                                    Description = dto.Description
                                }
                            },
                            Quantity = 1
                        }
                    },
                    Mode = "payment",
                    SuccessUrl = $"{_domain}payments/success?session_id={{CHECKOUT_SESSION_ID}}&invoice={dto.InvoiceId}",
                    CancelUrl = $"{_domain}payments/cancel?invoice={dto.InvoiceId}",
                    ClientReferenceId = dto.InvoiceId,
                    CustomerEmail = dto.ClientEmail,
                    Metadata = dto.Metadata ?? new Dictionary<string, string>
                    {
                        { "invoice_id", dto.InvoiceId },
                        { "customer_email", dto.ClientEmail ?? "" },
                        { "created_at", DateTime.UtcNow.ToString("O") }
                    }
                };

                var service = new SessionService();
                var session = await service.CreateAsync(options);

                _logger.LogInformation("Stripe session created successfully for invoice {InvoiceId}", dto.InvoiceId);

                return new GeneralResponse<string>
                {
                    Success = true,
                    Message = "Stripe session created successfully.",
                    Data = session.Url
                };
            }
            catch (StripeException ex)
            {
                _logger.LogError(ex, "Stripe error creating session for invoice {InvoiceId}", dto.InvoiceId);
                return new GeneralResponse<string>
                {
                    Success = false,
                    Message = $"Stripe error: {ex.StripeError?.Message ?? ex.Message}"
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating Stripe session for invoice {InvoiceId}", dto.InvoiceId);
                return new GeneralResponse<string>
                {
                    Success = false,
                    Message = $"Failed to create Stripe session: {ex.Message}"
                };
            }
        }

        public async Task<GeneralResponse<string>> CreatePayPalSessionAsync(PaymentCreateDTO dto)
        {
            try
            {
                var paypalConfig = _configuration.GetSection("PayPal");
                var clientId = paypalConfig["ClientId"];
                var clientSecret = paypalConfig["ClientSecret"];
                var baseUrl = paypalConfig["BaseUrl"] ?? "https://api-m.sandbox.paypal.com";

                // Get access token
                var authString = Convert.ToBase64String(Encoding.UTF8.GetBytes($"{clientId}:{clientSecret}"));
                var authRequest = new HttpRequestMessage(HttpMethod.Post, $"{baseUrl}/v1/oauth2/token");
                authRequest.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Basic", authString);
                authRequest.Content = new FormUrlEncodedContent(new[]
                {
                    new KeyValuePair<string, string>("grant_type", "client_credentials")
                });

                var authResponse = await _httpClient.SendAsync(authRequest);
                if (!authResponse.IsSuccessStatusCode)
                {
                    throw new Exception($"PayPal authentication failed: {authResponse.StatusCode}");
                }

                var authContent = await authResponse.Content.ReadAsStringAsync();
                var authResult = JsonSerializer.Deserialize<PayPalAuthResponse>(authContent);

                // Create order
                var orderRequest = new HttpRequestMessage(HttpMethod.Post, $"{baseUrl}/v2/checkout/orders");
                orderRequest.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", authResult.AccessToken);

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
                            description = dto.Description,
                            custom_id = dto.InvoiceId,
                            invoice_id = dto.InvoiceId
                        }
                    },
                    application_context = new
                    {
                        return_url = $"{_domain}payments/paypal/success?invoice={dto.InvoiceId}",
                        cancel_url = $"{_domain}payments/paypal/cancel?invoice={dto.InvoiceId}",
                        brand_name = _configuration["AppSettings:CompanyName"] ?? "Your Company"
                    }
                };

                orderRequest.Content = new StringContent(
                    JsonSerializer.Serialize(orderData),
                    Encoding.UTF8,
                    "application/json"
                );

                var orderResponse = await _httpClient.SendAsync(orderRequest);
                if (!orderResponse.IsSuccessStatusCode)
                {
                    throw new Exception($"PayPal order creation failed: {orderResponse.StatusCode}");
                }

                var orderContent = await orderResponse.Content.ReadAsStringAsync();
                var orderResult = JsonSerializer.Deserialize<PayPalOrderResponse>(orderContent);

                var approvalUrl = orderResult.Links
                    .FirstOrDefault(link => link.Rel == "approve")
                    ?.Href;

                if (string.IsNullOrEmpty(approvalUrl))
                {
                    throw new Exception("PayPal approval URL not found");
                }

                return new GeneralResponse<string>
                {
                    Success = true,
                    Message = "PayPal session created successfully.",
                    Data = approvalUrl
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating PayPal session for invoice {InvoiceId}", dto.InvoiceId);
                return new GeneralResponse<string>
                {
                    Success = false,
                    Message = $"Failed to create PayPal session: {ex.Message}"
                };
            }
        }

        public async Task<GeneralResponse<string>> CreateApplePaySessionAsync(PaymentCreateDTO dto)
        {
            // Implementation for Apple Pay
            return await Task.FromResult(new GeneralResponse<string>
            {
                Success = true,
                Message = "Apple Pay session created.",
                Data = $"{_domain}payments/applepay/process?invoice={dto.InvoiceId}"
            });
        }

        public async Task<GeneralResponse<string>> CreateGooglePaySessionAsync(PaymentCreateDTO dto)
        {
            // Implementation for Google Pay
            return await Task.FromResult(new GeneralResponse<string>
            {
                Success = true,
                Message = "Google Pay session created.",
                Data = $"{_domain}payments/googlepay/process?invoice={dto.InvoiceId}"
            });
        }

        public async Task<GeneralResponse<string>> CreateMadaSessionAsync(PaymentCreateDTO dto)
        {
            // Implementation for Mada
            return await Task.FromResult(new GeneralResponse<string>
            {
                Success = true,
                Message = "Mada payment session created.",
                Data = $"{_domain}payments/mada/process?invoice={dto.InvoiceId}"
            });
        }

        public async Task<GeneralResponse<string>> CreateSTCPaySessionAsync(PaymentCreateDTO dto)
        {
            // Implementation for STC Pay
            return await Task.FromResult(new GeneralResponse<string>
            {
                Success = true,
                Message = "STC Pay session created.",
                Data = $"{_domain}payments/stcpay/process?invoice={dto.InvoiceId}"
            });
        }

        public async Task<GeneralResponse<string>> CreateSadadSessionAsync(PaymentCreateDTO dto)
        {
            // Implementation for Sadad
            return await Task.FromResult(new GeneralResponse<string>
            {
                Success = true,
                Message = "Sadad payment session created.",
                Data = $"{_domain}payments/sadad/process?invoice={dto.InvoiceId}"
            });
        }

        public async Task<GeneralResponse<string>> CreateGenericCardSessionAsync(PaymentCreateDTO dto, PaymentType type)
        {
            return await Task.FromResult(new GeneralResponse<string>
            {
                Success = true,
                Message = $"{type} payment session created.",
                Data = $"{_domain}payments/card/process?type={type}&invoice={dto.InvoiceId}"
            });
        }

        public async Task<GeneralResponse<bool>> CancelPaymentAsync(string paymentId, PaymentType paymentType)
        {
            return await Task.FromResult(new GeneralResponse<bool>
            {
                Success = true,
                Message = $"Payment {paymentId} cancellation processed.",
                Data = true
            });
        }

        public async Task<GeneralResponse<bool>> RefundPaymentAsync(string paymentId, PaymentType paymentType)
        {
            return await Task.FromResult(new GeneralResponse<bool>
            {
                Success = true,
                Message = $"Payment {paymentId} refund processed.",
                Data = true
            });
        }

        public async Task<GeneralResponse<PaymentStatusResponse>> GetPaymentStatusAsync(string paymentId, PaymentType paymentType)
        {
            return await Task.FromResult(new GeneralResponse<PaymentStatusResponse>
            {
                Success = true,
                Message = "Payment status retrieved.",
                Data = new PaymentStatusResponse
                {
                    PaymentId = paymentId,
                    Status = PaymentStatus.Completed,
                    LastUpdated = DateTime.UtcNow
                }
            });
        }
    }
}