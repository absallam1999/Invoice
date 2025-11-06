using invoice.Core.DTO;
using invoice.Core.DTO.Payment;
using invoice.Core.DTO.PaymentResponse;
using invoice.Core.DTO.PaymentResponse.TapPayments;
using invoice.Core.Enums;

namespace invoice.Core.Interfaces.Services
{
    public interface IPaymentGateway
    {
        PaymentType PaymentType { get; }
        Task<GeneralResponse<PaymentSessionResponse>> CreatePaymentSessionAsync(PaymentCreateDTO dto);
        Task<GeneralResponse<PaymentStatusResponse>> GetPaymentStatusAsync(string paymentId);
        Task<GeneralResponse<bool>> CancelPaymentAsync(string paymentId);
        Task<string?> CreateMerchantAccountAsync(TapUserDto dto,string UserId);


    }
}