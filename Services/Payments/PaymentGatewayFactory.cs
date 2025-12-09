using invoice.Core.Enums;
using invoice.Core.Interfaces.Services;
using invoice.Services.Payments.Edfa;
using invoice.Services.Payments.Paypal;
using invoice.Services.Payments.Stripe;
using invoice.Services.Payments.TabPayments;

namespace invoice.Services.Payments
{
    public class PaymentGatewayFactory
    {
        private readonly IServiceProvider _serviceProvider;

        public PaymentGatewayFactory(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public IPaymentGateway GetGateway(PaymentType paymentType)
        {
            return paymentType switch
            {
               // PaymentType.Edfa => _serviceProvider.GetRequiredService<EdfaPaymentService>(),
               // PaymentType.Stripe => _serviceProvider.GetRequiredService<StripePaymentService>(),
               // PaymentType.PayPal => _serviceProvider.GetRequiredService<PayPalPaymentService>(),
                PaymentType.TabPayments => _serviceProvider.GetRequiredService<TabPaymentsService>(),

                // Future gateways (to implement later)
                PaymentType.GooglePay => throw new NotSupportedException("Google Pay gateway is not implemented yet"),
                PaymentType.STCPay => throw new NotSupportedException("STC Pay gateway is not implemented yet"),
                PaymentType.Sadad => throw new NotSupportedException("Sadad gateway is not implemented yet"),
                PaymentType.Cash => throw new NotSupportedException("Cash payments are handled offline"),
                PaymentType.CreditCard => throw new NotSupportedException("Generic Credit Card payments should be mapped to Stripe/PayPal"),
                PaymentType.DebitCard => throw new NotSupportedException("Generic Debit Card payments should be mapped to Stripe/PayPal"),
                PaymentType.BankTransfer => throw new NotSupportedException("Bank transfers are handled manually"),
                PaymentType.Delivery => throw new NotSupportedException("Delivery payments are handled manually"),

                _ => throw new NotSupportedException($"Payment type {paymentType} is not supported")
            };
        }

        public bool IsGatewaySupported(PaymentType paymentType)
        {
            return paymentType switch
            {
                PaymentType.Stripe => true,
                PaymentType.PayPal => true,
                PaymentType.TabPayments => true,
                PaymentType.Edfa => true,
                _ => false
            };
        }

        // Helper method to handle offline payments gracefully
        public bool IsOnlinePayment(PaymentType paymentType)
        {
            return paymentType switch
            {
                PaymentType.Cash => false,
                PaymentType.Delivery => false,
                PaymentType.BankTransfer => false,
                _ => IsGatewaySupported(paymentType)
            };
        }
    }
}