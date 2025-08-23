using invoice.Core.DTO;
using invoice.Core.DTO.Payment;
using invoice.Core.Enums;

namespace invoice.Core.Interfaces.Services
{
    public interface IPaymentGatewayService
    {
        Task<GeneralResponse<string>> CreatePaymentSessionAsync(PaymentCreateDTO dto, PaymentType type);
        Task<GeneralResponse<string>> CreateStripeSessionAsync(PaymentCreateDTO dto);
        Task<GeneralResponse<string>> CreatePayPalSessionAsync(PaymentCreateDTO dto);
        Task<GeneralResponse<string>> CreateApplePaySessionAsync(PaymentCreateDTO dto);
        Task<GeneralResponse<string>> CreateGooglePaySessionAsync(PaymentCreateDTO dto);

        Task<GeneralResponse<bool>> CancelPaymentAsync(string paymentId, PaymentType paymentType);
        Task<GeneralResponse<bool>> RefundPaymentAsync(string paymentId, PaymentType paymentType);
    }
}
