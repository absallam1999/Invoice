using invoice.Core.DTO;
using invoice.Core.DTO.Payment;
using invoice.Core.DTO.PaymentResponse;
using invoice.Core.Enums;

namespace invoice.Core.Interfaces.Services
{
    public interface IPaymentGatewayService
    {
        Task<GeneralResponse<PaymentSessionResponse>> CreatePaymentSessionAsync(PaymentCreateDTO dto, PaymentType type);
        Task<GeneralResponse<PaymentStatusResponse>> GetPaymentStatusAsync(string paymentId, PaymentType paymentType);
        Task<GeneralResponse<bool>> CancelPaymentAsync(string paymentId, PaymentType paymentType);
    }
}