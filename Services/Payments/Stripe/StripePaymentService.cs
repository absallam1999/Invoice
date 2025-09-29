using invoice.Core.DTO;
using invoice.Core.DTO.Payment;
using invoice.Core.DTO.PaymentResponse;
using invoice.Core.Enums;
using invoice.Core.Interfaces.Services;
using Microsoft.Extensions.Options;
using Stripe;
using Stripe.Checkout;
using System.Text.Json;

namespace invoice.Services.Payments.Stripe
{
    public class StripePaymentService : PaymentGatewayBase, IPaymentGateway
    {
        private readonly StripeOptions _options;

        public override PaymentType PaymentType => PaymentType.Stripe;

        public StripePaymentService(
            IConfiguration configuration,
            IOptions<StripeOptions> options)
            : base(configuration)
        {
            _options = options.Value;
            StripeConfiguration.ApiKey = _options.SecretKey;
        }

        public override async Task<GeneralResponse<PaymentSessionResponse>> CreatePaymentSessionAsync(PaymentCreateDTO dto)
        {
            try
            {
                var validationResult = ValidatePaymentRequest(dto);
                if (!validationResult.Success)
                    return validationResult;

                var currency = (dto.Currency ?? "USD").ToLowerInvariant();
                if (currency.Length != 3)
                    currency = "usd";

                var unitAmount = (long)(dto.Cost * 100);
                if (unitAmount < 50)
                    return CreateErrorResponse<PaymentSessionResponse>($"Payment amount too small. Minimum amount is {GetMinimumAmount(currency)}");

                var domain = NormalizeDomain(Domain);

                var sessionOptions = new SessionCreateOptions
                {
                    PaymentMethodTypes = new List<string> { "card", "link" },
                    LineItems = new List<SessionLineItemOptions>
            {
                new SessionLineItemOptions
                {
                    PriceData = new SessionLineItemPriceDataOptions
                    {
                        UnitAmount = unitAmount,
                        Currency = currency,
                        ProductData = new SessionLineItemPriceDataProductDataOptions
                        {
                            Name = TruncateString(dto.Name, 500),
                            Description = TruncateString(dto.Description, 500)
                        }
                    },
                    Quantity = 1
                }
            },
                    Mode = "payment",
                    SuccessUrl = $"{domain}payments/success?session_id={{CHECKOUT_SESSION_ID}}&invoice={Uri.EscapeDataString(dto.InvoiceId)}",
                    CancelUrl = $"{domain}payments/cancel?invoice={Uri.EscapeDataString(dto.InvoiceId)}",
                    ClientReferenceId = dto.InvoiceId,
                    CustomerEmail = !string.IsNullOrWhiteSpace(dto.ClientEmail) ? dto.ClientEmail : null,
                    Metadata = CreateMetadata(dto),
                    PaymentIntentData = new SessionPaymentIntentDataOptions
                    {
                        Metadata = CreateMetadata(dto),
                        Description = $"Payment for {TruncateString(dto.Name, 500)}"
                    },
                    ExpiresAt = DateTime.UtcNow.AddHours(24),
                    AllowPromotionCodes = true,
                    BillingAddressCollection = "required",
                    //AutomaticTax = new SessionAutomaticTaxOptions { Enabled = true },
                    //TaxIdCollection = new SessionTaxIdCollectionOptions { Enabled = true }
                    
                    // FOR TESTING
                    AutomaticTax = new SessionAutomaticTaxOptions { Enabled = false},
                    TaxIdCollection = new SessionTaxIdCollectionOptions { Enabled = false }
                };

                if (!string.IsNullOrWhiteSpace(dto.ClientEmail))
                    sessionOptions.CustomerCreation = "always";

                var service = new SessionService();
                var session = await service.CreateAsync(sessionOptions, GetRequestOptions());

                return CreateSuccessResponse(
                    session.Id,
                    session.Url,
                    dto,
                    session.ExpiresAt,
                    JsonSerializer.Serialize(session)
                );
            }
            catch (StripeException ex)
            {
                var errorMessage = $"Stripe error: {ex.StripeError?.Message}, Code: {ex.StripeError?.Code}, Type: {ex.StripeError?.Type}";

                return new GeneralResponse<PaymentSessionResponse>
                {
                    Success = false,
                    Message = errorMessage,
                    Data = null
                };
            }
            catch (Exception ex)
            {
                return new GeneralResponse<PaymentSessionResponse>
                {
                    Success = false,
                    Message = "An unexpected error occurred while creating the payment session",
                    Data = null
                };
            }
        }

        public override async Task<GeneralResponse<bool>> CancelPaymentAsync(string paymentId)
        {
            try
            {
                var service = new PaymentIntentService();
                await service.CancelAsync(paymentId);

                return new GeneralResponse<bool> { Success = true, Data = true };
            }
            catch (StripeException ex)
            {
                return CreateErrorResponse<bool>(GetUserFriendlyError(ex), ex.StripeError?.Code);
            }
        }

        public override async Task<GeneralResponse<bool>> RefundPaymentAsync(string paymentId)
        {
            try
            {
                var options = new RefundCreateOptions { PaymentIntent = paymentId };
                var service = new RefundService();
                await service.CreateAsync(options);

                return new GeneralResponse<bool> { Success = true, Data = true };
            }
            catch (StripeException ex)
            {
                return CreateErrorResponse<bool>(GetUserFriendlyError(ex), ex.StripeError?.Code);
            }
        }

        public override async Task<GeneralResponse<PaymentStatusResponse>> GetPaymentStatusAsync(string paymentId)
        {
            try
            {
                var service = new PaymentIntentService();
                var paymentIntent = await service.GetAsync(paymentId);

                var status = MapStripeStatus(paymentIntent.Status);

                return new GeneralResponse<PaymentStatusResponse>
                {
                    Success = true,
                    Data = new PaymentStatusResponse
                    {
                        PaymentId = paymentId,
                        Status = status,
                        LastUpdated = DateTime.UtcNow,
                        RawResponse = JsonSerializer.Serialize(paymentIntent)
                    }
                };
            }
            catch (StripeException ex)
            {
                return CreateErrorResponse<PaymentStatusResponse>(GetUserFriendlyError(ex), ex.StripeError?.Code);
            }
        }

        public override async Task<GeneralResponse<PaymentWebhookResponse>> ProcessWebhookAsync(string payload, string signature)
        {
            try
            {
                var stripeEvent = EventUtility.ConstructEvent(
                    payload,
                    signature,
                    _options.WebhookSecret
                );

                string paymentId = null;
                PaymentStatus status = PaymentStatus.Pending;
                string eventType = stripeEvent.Type;

                switch (stripeEvent.Type)
                {
                    case "payment_intent.succeeded":
                        var paymentIntent = stripeEvent.Data.Object as PaymentIntent;
                        paymentId = paymentIntent?.Id;
                        status = PaymentStatus.Completed;
                        break;

                    case "payment_intent.payment_failed":
                        var failedPayment = stripeEvent.Data.Object as PaymentIntent;
                        paymentId = failedPayment?.Id;
                        status = PaymentStatus.Failed;
                        break;

                    case "payment_intent.canceled":
                        var canceledPayment = stripeEvent.Data.Object as PaymentIntent;
                        paymentId = canceledPayment?.Id;
                        status = PaymentStatus.Cancelled;
                        break;

                    case "charge.refunded":
                        var charge = stripeEvent.Data.Object as Charge;
                        paymentId = charge?.PaymentIntentId;
                        status = PaymentStatus.Refunded;
                        break;

                    default:
                        break;
                }

                return new GeneralResponse<PaymentWebhookResponse>
                {
                    Success = true,
                    Data = new PaymentWebhookResponse
                    {
                        Processed = true,
                        EventType = eventType,
                        PaymentId = paymentId,
                        Status = status,
                        GatewayName = "Stripe",
                        Timestamp = DateTime.UtcNow,
                        Metadata = new Dictionary<string, string>
                {
                    { "stripe_event_id", stripeEvent.Id },
                    { "livemode", stripeEvent.Livemode.ToString() }
                }
                    }
                };
            }
            catch (StripeException ex)
            {
                return CreateErrorResponse<PaymentWebhookResponse>("Webhook signature verification failed", "invalid_signature");
            }
            catch (Exception ex)
            {
                return CreateErrorResponse<PaymentWebhookResponse>("Failed to process webhook");
            }
        }

        private Dictionary<string, string> CreateMetadata(PaymentCreateDTO dto)
        {
            var metadata = dto.Metadata ?? new Dictionary<string, string>();
            metadata["invoice_id"] = dto.InvoiceId;
            metadata["created_at_utc"] = DateTime.UtcNow.ToString("O");

            if (!string.IsNullOrEmpty(dto.ClientEmail))
                metadata["customer_email"] = dto.ClientEmail;

            if (!string.IsNullOrEmpty(dto.ClientId))
                metadata["client_id"] = dto.ClientId;

            return metadata;
        }

        private RequestOptions GetRequestOptions()
        {
            return new RequestOptions { IdempotencyKey = Guid.NewGuid().ToString() };
        }

        private string GetUserFriendlyError(StripeException ex)
        {
            return ex.StripeError?.Type switch
            {
                "card_error" => "There was an issue with your card. Please check your details and try again.",
                "invalid_request_error" => "Invalid payment request. Please contact support.",
                "api_error" => "Temporary payment service issue. Please try again in a moment.",
                "authentication_error" => "Payment authentication failed. Please try again.",
                "rate_limit_error" => "Too many payment attempts. Please wait a moment and try again.",
                _ => ex.StripeError?.Message ?? "An unexpected payment error occurred. Please try again."
            };
        }

        private PaymentStatus MapStripeStatus(string stripeStatus)
        {
            return stripeStatus switch
            {
                "succeeded" => PaymentStatus.Completed,
                "processing" => PaymentStatus.Pending,
                "requires_payment_method" => PaymentStatus.Failed,
                "requires_action" => PaymentStatus.Pending,
                "requires_capture" => PaymentStatus.Pending,
                "canceled" => PaymentStatus.Cancelled,
                _ => PaymentStatus.Failed
            };
        }

        private string GetMinimumAmount(string currency)
        {
            var minimums = new Dictionary<string, string>
            {
                ["usd"] = "$0.50",
                ["eur"] = "€0.50",
                ["gbp"] = "£0.30",
                ["jpy"] = "¥50",
                ["cad"] = "C$0.50"
            };

            return minimums.TryGetValue(currency, out var minimum)
                ? minimum
                : $"{0.50} {currency.ToUpper()}";
        }
    }
}