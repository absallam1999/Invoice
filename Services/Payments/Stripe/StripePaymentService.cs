//using invoice.Core.DTO;
//using invoice.Core.DTO.Payment;
//using invoice.Core.DTO.PaymentResponse;
//using invoice.Core.Enums;
//using invoice.Core.Interfaces.Services;
//using invoice.Repo;
//using Microsoft.Extensions.Options;
//using Stripe;
//using Stripe.Checkout;
//using System.Text.Json;

//namespace invoice.Services.Payments.Stripe
//{
//    public class StripePaymentService : PaymentGatewayBase, IPaymentGateway
//    {
//        private readonly StripeOptions _options;
//        private readonly IRepository<Core.Entities.Invoice> _invoiceRepo;
//        private readonly decimal _defaultCommissionRate;

//        public override PaymentType PaymentType => PaymentType.Stripe;

//        public StripePaymentService(
//            IConfiguration configuration,
//            IOptions<StripeOptions> options,
//            IRepository<Core.Entities.Invoice> invoiceRepo = null)
//            : base(configuration)
//        {
//            _options = options.Value;
//            _invoiceRepo = invoiceRepo;
//            StripeConfiguration.ApiKey = _options.SecretKey;

//            var cfgRate = Configuration["Payments:DefaultCommissionPercent"];
//            _defaultCommissionRate = decimal.TryParse(cfgRate, out var v) ? v / 100m : 0.1m;
//        }

//        public override async Task<GeneralResponse<PaymentSessionResponse>> CreatePaymentSessionAsync(PaymentCreateDTO dto)
//        {
//            try
//            {
//                var validationResult = ValidatePaymentRequest(dto);
//                if (!validationResult.Success)
//                    return new GeneralResponse<PaymentSessionResponse>(false, validationResult.Message);

//                Core.Entities.Invoice? invoice = null;
//                if (!string.IsNullOrEmpty(dto.InvoiceId) && _invoiceRepo != null)
//                {
//                    invoice = await _invoiceRepo.GetByIdAsync(dto.InvoiceId);
//                }

//                var currency = (dto.Currency ?? "USD").ToLowerInvariant();
//                if (currency.Length != 3)
//                    currency = "usd";

//                var unitAmount = (long)(dto.Cost * 100);
//                if (unitAmount < 50)
//                    return new GeneralResponse<PaymentSessionResponse>(false, $"Payment amount too small. Minimum amount is {GetMinimumAmount(currency)}");

//                var domain = NormalizeDomain(Domain);

//                var sellerStripeAccountId = await GetSellerIdFromInvoice(invoice);
//                var isP2P = !string.IsNullOrEmpty(sellerStripeAccountId);

//                decimal sellerAmount = dto.Cost;
//                if (isP2P)
//                {
//                    var commission = dto.Cost * _defaultCommissionRate;
//                    sellerAmount = dto.Cost - commission;
//                }


//                var sessionOptions = new SessionCreateOptions
//                {
//                    PaymentMethodTypes = new List<string> { "card", "link" },
//                    LineItems = new List<SessionLineItemOptions>
//            {
//                new SessionLineItemOptions
//                {
//                    PriceData = new SessionLineItemPriceDataOptions
//                    {
//                        UnitAmount = unitAmount,
//                        Currency = currency,
//                        ProductData = new SessionLineItemPriceDataProductDataOptions
//                        {
//                            Name = TruncateString(dto.Name, 500),
//                            Description = TruncateString(dto.Description, 500)
//                        }
//                    },
//                    Quantity = 1
//                }
//            },
//                    Mode = "payment",
//                    SuccessUrl = $"{domain}payments/success?session_id={{CHECKOUT_SESSION_ID}}&invoice={Uri.EscapeDataString(dto.InvoiceId)}",
//                    CancelUrl = $"{domain}payments/cancel?invoice={Uri.EscapeDataString(dto.InvoiceId)}",
//                    ClientReferenceId = dto.InvoiceId,
//                    CustomerEmail = !string.IsNullOrWhiteSpace(dto.ClientEmail) ? dto.ClientEmail : null,
//                    Metadata = CreateMetadata(dto, sellerAmount, isP2P),
//                    ExpiresAt = DateTime.UtcNow.AddHours(24),
//                    AllowPromotionCodes = true,
//                    BillingAddressCollection = "required",
//                    AutomaticTax = new SessionAutomaticTaxOptions { Enabled = false },
//                    TaxIdCollection = new SessionTaxIdCollectionOptions { Enabled = false }
//                };

//                if (isP2P)
//                {
//                    sessionOptions.PaymentIntentData = new SessionPaymentIntentDataOptions
//                    {
//                        Metadata = CreateMetadata(dto, sellerAmount, isP2P),
//                        Description = $"Payment for {TruncateString(dto.Name, 500)}",
//                        TransferData = new SessionPaymentIntentDataTransferDataOptions
//                        {
//                            Destination = sellerStripeAccountId,
//                            Amount = (long)(sellerAmount * 100)
//                        },
//                    };
//                }
//                else
//                {
//                    sessionOptions.PaymentIntentData = new SessionPaymentIntentDataOptions
//                    {
//                        Metadata = CreateMetadata(dto, sellerAmount, isP2P),
//                        Description = $"Payment for {TruncateString(dto.Name, 500)}"
//                    };
//                }

//                if (!string.IsNullOrWhiteSpace(dto.ClientEmail))
//                    sessionOptions.CustomerCreation = "always";

//                var service = new SessionService();

//                var requestOptions = GetRequestOptions();
//                if (isP2P)
//                {
//                    requestOptions.StripeAccount = sellerStripeAccountId;
//                }

//                var session = await service.CreateAsync(sessionOptions, requestOptions);

//                var responseData = new PaymentSessionResponse
//                {
//                    SessionId = session.Id,
//                    PaymentUrl = session.Url,
//                    ExpiresAt = session.ExpiresAt,
//                    PaymentType = PaymentType.Stripe,
//                    PaymentStatus = PaymentStatus.Pending,
//                    InvoiceId = dto.InvoiceId,
//                    Amount = dto.Cost,
//                    Currency = "USD",
//                    RawResponse = JsonSerializer.Serialize(session)
//                };

//                return new GeneralResponse<PaymentSessionResponse>(true, "Payment session created successfully", responseData);
//            }
//            catch (StripeException ex)
//            {
//                var errorMessage = $"Stripe error: {ex.StripeError?.Message}";
//                if (ex.StripeError?.Code != null)
//                    errorMessage += $", Code: {ex.StripeError.Code}";
//                if (ex.StripeError?.Type != null)
//                    errorMessage += $", Type: {ex.StripeError.Type}";

//                return new GeneralResponse<PaymentSessionResponse>(false, errorMessage);
//            }
//            catch (Exception ex)
//            {
//                return new GeneralResponse<PaymentSessionResponse>(false, "An unexpected error occurred while creating the payment session");
//            }
//        }

//        public override async Task<GeneralResponse<bool>> CancelPaymentAsync(string paymentId)
//        {
//            try
//            {
//                var service = new PaymentIntentService();
//                await service.CancelAsync(paymentId);

//                return new GeneralResponse<bool>(true, "Payment cancelled successfully", true);
//            }
//            catch (StripeException ex)
//            {
//                return new GeneralResponse<bool>(false, GetUserFriendlyError(ex));
//            }
//            catch (Exception ex)
//            {
//                return new GeneralResponse<bool>(false, "Failed to cancel payment");
//            }
//        }

//        public override async Task<GeneralResponse<PaymentStatusResponse>> GetPaymentStatusAsync(string paymentId)
//        {
//            try
//            {
//                var service = new PaymentIntentService();
//                var paymentIntent = await service.GetAsync(paymentId);

//                var status = MapStripeStatus(paymentIntent.Status);

//                var statusResponse = new PaymentStatusResponse
//                {
//                    PaymentId = paymentId,
//                    Status = status,
//                    LastUpdated = DateTime.UtcNow,
//                    RawResponse = JsonSerializer.Serialize(paymentIntent)
//                };

//                return new GeneralResponse<PaymentStatusResponse>(true, "Status retrieved successfully", statusResponse);
//            }
//            catch (StripeException ex)
//            {
//                return new GeneralResponse<PaymentStatusResponse>(false, GetUserFriendlyError(ex));
//            }
//            catch (Exception ex)
//            {
//                return new GeneralResponse<PaymentStatusResponse>(false, "Failed to get payment status");
//            }
//        }

//        public override async Task<GeneralResponse<bool>> HandleWebhookAsync(WebhookPayloadDTO payload)
//        {
//            try
//            {
//                if (payload == null || string.IsNullOrEmpty(payload.RawPayload))
//                    return new GeneralResponse<bool>(false, "Webhook payload is required");

//                // Verify Stripe signature for security
//                if (!VerifyStripeSignature(payload))
//                    return new GeneralResponse<bool>(false, "Invalid Stripe signature");

//                var stripeEvent = EventUtility.ConstructEvent(
//                    payload.RawPayload,
//                    payload.Signature,
//                    _options.WebhookSecret
//                );

//                string invoiceId = string.Empty;
//                string paymentIntentId = string.Empty;
//                decimal amount = 0;
//                string currency = "usd";

//                return new GeneralResponse<bool>(true, "Webhook processed successfully", true);
//            }
//            catch (StripeException ex)
//            {
//                return new GeneralResponse<bool>(false, $"Stripe webhook error: {ex.Message}");
//            }
//            catch (JsonException jsonEx)
//            {
//                return new GeneralResponse<bool>(false, $"Invalid JSON in webhook payload: {jsonEx.Message}");
//            }
//            catch (Exception ex)
//            {
//                return new GeneralResponse<bool>(false, $"Error processing webhook: {ex.Message}");
//            }
//        }

//        public decimal GetMinimumAmount(string currency = "usd")
//        {
//            var minimums = new Dictionary<string, decimal>(StringComparer.OrdinalIgnoreCase)
//            {
//                ["usd"] = 0.50m,
//                ["eur"] = 0.50m,
//                ["gbp"] = 0.30m,
//                ["cad"] = 0.50m,
//                ["aud"] = 0.50m,
//                ["aed"] = 2.00m,
//                ["sar"] = 2.00m,
//            };

//            return minimums.TryGetValue(currency.ToLower(), out var minAmount) ? minAmount : 0.50m;
//        }

//        #region Helper Methods

//        private bool VerifyStripeSignature(WebhookPayloadDTO payload)
//        {
//            try
//            {
//                if (string.IsNullOrEmpty(_options.WebhookSecret))
//                    return true;

//                if (string.IsNullOrEmpty(payload.Signature))
//                    return false;

//                return !string.IsNullOrEmpty(payload.RawPayload) &&
//                       !string.IsNullOrEmpty(payload.Signature) &&
//                       !string.IsNullOrEmpty(_options.WebhookSecret);
//            }
//            catch
//            {
//                return false;
//            }
//        }

//        private async Task<string?> GetSellerIdFromInvoice(Core.Entities.Invoice invoice)
//        {
//            if (!string.IsNullOrEmpty(invoice.UserId))
//                return invoice?.User?.StripeAccountId;

//            if (!string.IsNullOrEmpty(invoice.ClientId))
//                return invoice?.User?.StripeAccountId;

//            return null;
//        }

//        private Dictionary<string, string> CreateMetadata(PaymentCreateDTO dto, decimal sellerAmount, bool isP2P)
//        {
//            var metadata = dto.Metadata ?? new Dictionary<string, string>();
//            metadata["invoice_id"] = dto.InvoiceId;
//            metadata["created_at_utc"] = DateTime.UtcNow.ToString("O");
//            metadata["seller_amount"] = sellerAmount.ToString("F2");
//            metadata["is_p2p"] = isP2P.ToString();

//            if (!string.IsNullOrEmpty(dto.ClientEmail))
//                metadata["customer_email"] = dto.ClientEmail;

//            if (!string.IsNullOrEmpty(dto.ClientId))
//                metadata["client_id"] = dto.ClientId;

//            return metadata;
//        }

//        private RequestOptions GetRequestOptions()
//        {
//            return new RequestOptions { IdempotencyKey = Guid.NewGuid().ToString() };
//        }

//        private string GetUserFriendlyError(StripeException ex)
//        {
//            return ex.StripeError?.Type switch
//            {
//                "card_error" => "There was an issue with your card. Please check your details and try again.",
//                "invalid_request_error" => "Invalid payment request. Please contact support.",
//                "api_error" => "Temporary payment service issue. Please try again in a moment.",
//                "authentication_error" => "Payment authentication failed. Please try again.",
//                "rate_limit_error" => "Too many payment attempts. Please wait a moment and try again.",
//                _ => ex.StripeError?.Message ?? "An unexpected payment error occurred. Please try again."
//            };
//        }

//        private PaymentStatus MapStripeStatus(string stripeStatus)
//        {
//            return stripeStatus switch
//            {
//                "succeeded" => PaymentStatus.Completed,
//                "processing" => PaymentStatus.Pending,
//                "requires_payment_method" => PaymentStatus.Failed,
//                "requires_action" => PaymentStatus.Pending,
//                "requires_capture" => PaymentStatus.Pending,
//                "canceled" => PaymentStatus.Cancelled,
//                _ => PaymentStatus.Failed
//            };
//        }

//        internal class ValidationResult
//        {
//            public bool Success { get; set; }
//            public string Message { get; set; } = string.Empty;

//            public static ValidationResult SuccessFull() => new ValidationResult { Success = true, Message = "Valid" };
//            public static ValidationResult Fail(string message) => new ValidationResult { Success = false, Message = message };
//        }

//        #endregion
//    }
//}