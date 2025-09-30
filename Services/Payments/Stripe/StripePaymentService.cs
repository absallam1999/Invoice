using invoice.Core.DTO;
using invoice.Core.DTO.Payment;
using invoice.Core.DTO.PaymentResponse;
using invoice.Core.Entities;
using invoice.Core.Enums;
using invoice.Core.Interfaces.Services;
using invoice.Repo;
using Microsoft.Extensions.Options;
using Stripe;
using Stripe.Checkout;
using System.Text.Json;

namespace invoice.Services.Payments.Stripe
{
    public class StripePaymentService : PaymentGatewayBase, IPaymentGateway
    {
        private readonly StripeOptions _options;
        private readonly IRepository<Core.Entities.Invoice> _invoiceRepo;
        private readonly IRepository<Commission> _commissionRepo;
        private readonly decimal _defaultCommissionRate;

        public override PaymentType PaymentType => PaymentType.Stripe;

        public StripePaymentService(
            IConfiguration configuration,
            IOptions<StripeOptions> options,
            IRepository<Core.Entities.Invoice> invoiceRepo = null,
            IRepository<Commission> commissionRepo = null)
            : base(configuration)
        {
            _options = options.Value;
            _invoiceRepo = invoiceRepo;
            _commissionRepo = commissionRepo;
            StripeConfiguration.ApiKey = _options.SecretKey;

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

                Core.Entities.Invoice? invoice = null;
                if (!string.IsNullOrEmpty(dto.InvoiceId) && _invoiceRepo != null)
                {
                    invoice = await _invoiceRepo.GetByIdAsync(dto.InvoiceId);
                }

                var currency = (dto.Currency ?? "USD").ToLowerInvariant();
                if (currency.Length != 3)
                    currency = "usd";

                var unitAmount = (long)(dto.Cost * 100);
                if (unitAmount < 50)
                    return new GeneralResponse<PaymentSessionResponse>(false, $"Payment amount too small. Minimum amount is {GetMinimumAmount(currency)}");

                var domain = NormalizeDomain(Domain);

                var (commissionRate, commissionValue, sellerAmount) = await CalculateCommissionAsync(dto, invoice);

                var sellerStripeAccountId = GetSellerStripeAccountId(dto, invoice);
                var isP2P = !string.IsNullOrEmpty(sellerStripeAccountId);

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
                    Metadata = CreateMetadata(dto, commissionRate, commissionValue, sellerAmount, isP2P),
                    ExpiresAt = DateTime.UtcNow.AddHours(24),
                    AllowPromotionCodes = true,
                    BillingAddressCollection = "required",
                    AutomaticTax = new SessionAutomaticTaxOptions { Enabled = false },
                    TaxIdCollection = new SessionTaxIdCollectionOptions { Enabled = false }
                };

                if (isP2P)
                {
                    sessionOptions.PaymentIntentData = new SessionPaymentIntentDataOptions
                    {
                        Metadata = CreateMetadata(dto, commissionRate, commissionValue, sellerAmount, isP2P),
                        Description = $"Payment for {TruncateString(dto.Name, 500)}",
                        TransferData = new SessionPaymentIntentDataTransferDataOptions
                        {
                            Destination = sellerStripeAccountId,
                            Amount = (long)(sellerAmount * 100) 
                        },
                    };
                }
                else
                {
                    sessionOptions.PaymentIntentData = new SessionPaymentIntentDataOptions
                    {
                        Metadata = CreateMetadata(dto, commissionRate, commissionValue, sellerAmount, isP2P),
                        Description = $"Payment for {TruncateString(dto.Name, 500)}"
                    };
                }

                if (!string.IsNullOrWhiteSpace(dto.ClientEmail))
                    sessionOptions.CustomerCreation = "always";

                var service = new SessionService();

                var requestOptions = GetRequestOptions();
                if (isP2P && !string.IsNullOrEmpty(sellerStripeAccountId))
                {
                    requestOptions.StripeAccount = sellerStripeAccountId;
                }

                var session = await service.CreateAsync(sessionOptions, requestOptions);

                if (invoice != null && _commissionRepo != null)
                {
                    var existingCommissions = await _commissionRepo.GetAllAsync();
                    var existingCommission = existingCommissions.FirstOrDefault(c => c.InvoiceId == invoice.Id);

                    if (existingCommission == null)
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
                }

                var responseData = new PaymentSessionResponse
                {
                    SessionId = session.Id,
                    PaymentUrl = session.Url,
                    ExpiresAt = session.ExpiresAt,
                    PaymentType = PaymentType.Stripe,
                    PaymentStatus = PaymentStatus.Pending,
                    InvoiceId = dto.InvoiceId,
                    Amount = dto.Cost,
                    Currency = "USD",
                    RawResponse = JsonSerializer.Serialize(session)
                };

                return new GeneralResponse<PaymentSessionResponse>(true, "Payment session created successfully", responseData);
            }
            catch (StripeException ex)
            {
                var errorMessage = $"Stripe error: {ex.StripeError?.Message}";
                if (ex.StripeError?.Code != null)
                    errorMessage += $", Code: {ex.StripeError.Code}";
                if (ex.StripeError?.Type != null)
                    errorMessage += $", Type: {ex.StripeError.Type}";

                return new GeneralResponse<PaymentSessionResponse>(false, errorMessage);
            }
            catch (Exception ex)
            {
                return new GeneralResponse<PaymentSessionResponse>(false, "An unexpected error occurred while creating the payment session");
            }
        }

        public override async Task<GeneralResponse<bool>> CancelPaymentAsync(string paymentId)
        {
            try
            {
                var service = new PaymentIntentService();
                await service.CancelAsync(paymentId);

                await UpdateCommissionOnCancellation(paymentId);

                return new GeneralResponse<bool>(true, "Payment cancelled successfully", true);
            }
            catch (StripeException ex)
            {
                return new GeneralResponse<bool>(false, GetUserFriendlyError(ex));
            }
            catch (Exception ex)
            {
                return new GeneralResponse<bool>(false, "Failed to cancel payment");
            }
        }

        public override async Task<GeneralResponse<bool>> RefundPaymentAsync(string paymentId)
        {
            try
            {
                var options = new RefundCreateOptions { PaymentIntent = paymentId };
                var service = new RefundService();
                await service.CreateAsync(options);

                await UpdateCommissionOnRefund(paymentId);

                return new GeneralResponse<bool>(true, "Refund processed successfully", true);
            }
            catch (StripeException ex)
            {
                return new GeneralResponse<bool>(false, GetUserFriendlyError(ex));
            }
            catch (Exception ex)
            {
                return new GeneralResponse<bool>(false, "Failed to process refund");
            }
        }

        public override async Task<GeneralResponse<PaymentStatusResponse>> GetPaymentStatusAsync(string paymentId)
        {
            try
            {
                var service = new PaymentIntentService();
                var paymentIntent = await service.GetAsync(paymentId);

                var status = MapStripeStatus(paymentIntent.Status);

                var statusResponse = new PaymentStatusResponse
                {
                    PaymentId = paymentId,
                    Status = status,
                    LastUpdated = DateTime.UtcNow,
                    RawResponse = JsonSerializer.Serialize(paymentIntent)
                };

                return new GeneralResponse<PaymentStatusResponse>(true, "Status retrieved successfully", statusResponse);
            }
            catch (StripeException ex)
            {
                return new GeneralResponse<PaymentStatusResponse>(false, GetUserFriendlyError(ex));
            }
            catch (Exception ex)
            {
                return new GeneralResponse<PaymentStatusResponse>(false, "Failed to get payment status");
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

                        if (paymentIntent != null)
                        {
                            await HandleSuccessfulPaymentAsync(paymentIntent);
                        }
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

                        if (canceledPayment != null)
                        {
                            await UpdateCommissionOnCancellation(canceledPayment.Id);
                        }
                        break;

                    case "charge.refunded":
                        var charge = stripeEvent.Data.Object as Charge;
                        paymentId = charge?.PaymentIntentId;
                        status = PaymentStatus.Refunded;

                        if (paymentId != null)
                        {
                            await UpdateCommissionOnRefund(paymentId);
                        }
                        break;

                    default:
                        break;
                }

                var webhookResponse = new PaymentWebhookResponse
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
                };

                return new GeneralResponse<PaymentWebhookResponse>(true, "Webhook processed successfully", webhookResponse);
            }
            catch (StripeException ex)
            {
                return new GeneralResponse<PaymentWebhookResponse>(false, "Webhook signature verification failed");
            }
            catch (Exception ex)
            {
                return new GeneralResponse<PaymentWebhookResponse>(false, "Failed to process webhook");
            }
        }

        #region Commission Management

        private async Task<(decimal commissionRate, decimal commissionValue, decimal sellerAmount)> CalculateCommissionAsync(PaymentCreateDTO dto, Core.Entities.Invoice? invoice = null)
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

        private async Task<decimal> GetCommissionRateAsync(PaymentCreateDTO dto, Core.Entities.Invoice? invoice = null)
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

        private string? GetSellerStripeAccountId(PaymentCreateDTO dto, Core.Entities.Invoice? invoice = null)
        {
            if (dto.Metadata != null && dto.Metadata.TryGetValue("seller_stripe_account_id", out var accountId) && !string.IsNullOrWhiteSpace(accountId))
                return accountId;

            if (invoice?.User?.StripeAccountId != null)
                return invoice.User.StripeAccountId;

            return null;
        }

        private async Task<string?> GetSellerIdFromInvoice(Core.Entities.Invoice invoice)
        {
            if (!string.IsNullOrEmpty(invoice.UserId))
                return invoice.UserId;

            if (!string.IsNullOrEmpty(invoice.ClientId))
                return invoice.ClientId;

            return null;
        }

        private async Task HandleSuccessfulPaymentAsync(PaymentIntent paymentIntent)
        {
            if (_invoiceRepo == null || _commissionRepo == null) return;

            try
            {
                var invoiceId = paymentIntent.Metadata?.GetValueOrDefault("invoice_id");
                if (string.IsNullOrEmpty(invoiceId)) return;

                var invoice = await _invoiceRepo.GetByIdAsync(invoiceId);
                if (invoice == null) return;

                var commissions = await _commissionRepo.GetAllAsync();
                var commission = commissions.FirstOrDefault(c => c.InvoiceId == invoice.Id);

                if (commission == null)
                {
                    var (commissionRate, commissionValue, sellerAmount) = await CalculateCommissionAsync(
                        new PaymentCreateDTO { Cost = (decimal)(paymentIntent.Amount / 100m) }, invoice);

                    var sellerId = await GetSellerIdFromInvoice(invoice);
                    if (string.IsNullOrEmpty(sellerId)) return;

                    commission = new Commission
                    {
                        InvoiceId = invoice.Id,
                        SellerId = sellerId,
                        Value = commissionValue,
                        CreatedAt = DateTime.UtcNow,
                        Settled = true
                    };
                    await _commissionRepo.AddAsync(commission);
                    await _commissionRepo.SaveChangesAsync();
                }
                else
                {
                    commission.Settled = true;
                    await _commissionRepo.UpdateAsync(commission);
                    await _commissionRepo.SaveChangesAsync();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error handling successful payment: {ex.Message}");
            }
        }

        private async Task UpdateCommissionOnCancellation(string paymentId)
        {
            try
            {
                if (_commissionRepo == null) return;

                var service = new PaymentIntentService();
                var paymentIntent = await service.GetAsync(paymentId);

                var invoiceId = paymentIntent.Metadata?.GetValueOrDefault("invoice_id");
                if (string.IsNullOrEmpty(invoiceId)) return;

                var commissions = await _commissionRepo.GetAllAsync();
                var commission = commissions.FirstOrDefault(c => c.InvoiceId == invoiceId);

                if (commission != null)
                {
                    commission.Settled = false;
                    await _commissionRepo.UpdateAsync(commission);
                    await _commissionRepo.SaveChangesAsync();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed to update commission on cancellation: {ex.Message}");
            }
        }

        private async Task UpdateCommissionOnRefund(string paymentId)
        {
            try
            {
                if (_commissionRepo == null) return;

                var service = new PaymentIntentService();
                var paymentIntent = await service.GetAsync(paymentId);

                var invoiceId = paymentIntent.Metadata?.GetValueOrDefault("invoice_id");
                if (string.IsNullOrEmpty(invoiceId)) return;

                var commissions = await _commissionRepo.GetAllAsync();
                var commission = commissions.FirstOrDefault(c => c.InvoiceId == invoiceId);

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

        #endregion

        #region Helper Methods

        private Dictionary<string, string> CreateMetadata(PaymentCreateDTO dto, decimal commissionRate, decimal commissionValue, decimal sellerAmount, bool isP2P)
        {
            var metadata = dto.Metadata ?? new Dictionary<string, string>();
            metadata["invoice_id"] = dto.InvoiceId;
            metadata["created_at_utc"] = DateTime.UtcNow.ToString("O");
            metadata["commission_rate"] = commissionRate.ToString("F4");
            metadata["commission_value"] = commissionValue.ToString("F2");
            metadata["seller_amount"] = sellerAmount.ToString("F2");
            metadata["is_p2p"] = isP2P.ToString();

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

        private ValidationResult ValidatePaymentRequest(PaymentCreateDTO dto)
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

        private static string TruncateString(string input, int maxLength)
        {
            if (string.IsNullOrEmpty(input)) return input;
            return input.Length <= maxLength ? input : input.Substring(0, maxLength);
        }

        internal class ValidationResult
        {
            public bool Success { get; set; }
            public string Message { get; set; } = string.Empty;

            public static ValidationResult SuccessFull() => new ValidationResult { Success = true, Message = "Valid" };
            public static ValidationResult Fail(string message) => new ValidationResult { Success = false, Message = message };
        }

        #endregion
    }
}