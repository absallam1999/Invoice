using invoice.Core.DTO;
using invoice.Core.DTO.Payment;
using invoice.Core.DTO.PaymentResponse;
using invoice.Core.Enums;

namespace invoice.Core.Interfaces.Services
{
    public interface IPaymentGateway
    {
        PaymentType PaymentType { get; }
        Task<GeneralResponse<PaymentSessionResponse>> CreatePaymentSessionAsync(PaymentCreateDTO dto);
        Task<GeneralResponse<bool>> CancelPaymentAsync(string paymentId);
        Task<GeneralResponse<bool>> RefundPaymentAsync(string paymentId);
        Task<GeneralResponse<PaymentStatusResponse>> GetPaymentStatusAsync(string paymentId);
        Task<GeneralResponse<PaymentWebhookResponse>> ProcessWebhookAsync(string payload, string signature);
    }
}