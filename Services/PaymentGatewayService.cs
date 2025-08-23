using invoice.Core.DTO;
using invoice.Core.DTO.Payment;
using invoice.Core.Enums;
using invoice.Core.Interfaces.Services;
using Stripe.Checkout;

namespace invoice.Services
{
    public class PaymentGatewayService : IPaymentGatewayService
    {
        private readonly string _domain = "http://localhost:7230/";

        public async Task<GeneralResponse<string>> CreatePaymentSessionAsync(PaymentCreateDTO dto, PaymentType type)
        {
            switch (type)
            {
                case PaymentType.Cash:
                case PaymentType.Delivery:
                    return new GeneralResponse<string>
                    {
                        Success = true,
                        Message = $"{type} selected, no online session required.",
                        Data = null
                    };

                case PaymentType.Stripe:
                    return await CreateStripeSessionAsync(dto);

                case PaymentType.PayPal:
                    return await CreatePayPalSessionAsync(dto);

                case PaymentType.ApplePay:
                    return await CreateApplePaySessionAsync(dto);

                case PaymentType.GooglePay:
                    return await CreateGooglePaySessionAsync(dto);

                case PaymentType.CreditCard:
                case PaymentType.DebitCard:
                case PaymentType.BankTransfer:
                case PaymentType.Mada:
                case PaymentType.STCPay:
                case PaymentType.Sadad:
                    // Generic or custom gateway stub
                    return new GeneralResponse<string>
                    {
                        Success = true,
                        Message = $"{type} payment session created (stub).",
                        Data = $"{_domain}Payments/{type}/Checkout?amount={dto.Cost}&invoice={dto.InvoiceId}"
                    };

                default:
                    return new GeneralResponse<string>
                    {
                        Success = false,
                        Message = $"Payment type {type} not supported."
                    };
            }
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
                            Currency = "usd",
                            ProductData = new SessionLineItemPriceDataProductDataOptions
                            {
                                Name = dto.Name
                            }
                        },
                        Quantity = 1
                    }
                },
                    Mode = "payment",
                    SuccessUrl = $"{_domain}Payments/Success?session_id={{CHECKOUT_SESSION_ID}}",
                    CancelUrl = $"{_domain}Payments/Cancel"
                };

                var service = new SessionService();
                var session = await Task.Run(() => service.Create(options));

                return new GeneralResponse<string>
                {
                    Success = true,
                    Message = "Stripe session created successfully.",
                    Data = session.Url
                };
            }
            catch (Exception ex)
            {
                return new GeneralResponse<string>
                {
                    Success = false,
                    Message = $"Stripe session creation failed: {ex.Message}"
                };
            }
        }

        public async Task<GeneralResponse<string>> CreatePayPalSessionAsync(PaymentCreateDTO dto)
        {
            // Implement PayPal checkout session creation here
            return new GeneralResponse<string>
            {
                Success = true,
                Message = "PayPal session created (stub).",
                Data = "https://paypal.com/checkout_stub"
            };
        }

        public async Task<GeneralResponse<string>> CreateApplePaySessionAsync(PaymentCreateDTO dto)
        {
            // Implement ApplePay session creation here
            return new GeneralResponse<string>
            {
                Success = true,
                Message = "ApplePay session created (stub).",
                Data = "https://applepay.com/checkout_stub"
            };
        }

        public async Task<GeneralResponse<string>> CreateGooglePaySessionAsync(PaymentCreateDTO dto)
        {
            // Implement GooglePay session creation here
            return new GeneralResponse<string>
            {
                Success = true,
                Message = "GooglePay session created (stub).",
                Data = "https://googlepay.com/checkout_stub"
            };
        }

        public async Task<GeneralResponse<bool>> CancelPaymentAsync(string paymentId, PaymentType paymentType)
        {
            // Switch based on paymentType to handle cancellation for each gateway
            return new GeneralResponse<bool>
            {
                Success = true,
                Message = $"Payment {paymentId} cancellation processed for {paymentType}.",
                Data = true
            };
        }

        public async Task<GeneralResponse<bool>> RefundPaymentAsync(string paymentId, PaymentType paymentType)
        {
            // Switch based on paymentType to handle refunds for each gateway
            return new GeneralResponse<bool>
            {
                Success = true,
                Message = $"Payment {paymentId} refunded for {paymentType}.",
                Data = true
            };
        }
    }
}
