using invoice.Core.DTO;
using invoice.Core.DTO.Payment;
using invoice.Core.DTO.PaymentResponse;
using invoice.Core.Enums;
using invoice.Core.Interfaces.Services;
using invoice.Helpers;
using invoice.Services.Payments;

namespace invoice.Services
{
    public class PaymentGatewayService : IPaymentGatewayService
    {
        private readonly PaymentGatewayFactory _gatewayFactory;

        public PaymentGatewayService(
            PaymentGatewayFactory gatewayFactory)
        {
            _gatewayFactory = gatewayFactory;
        }

        public async Task<GeneralResponse<PaymentSessionResponse>> CreatePaymentSessionAsync(PaymentCreateDTO dto, PaymentType type)
        {
            try
            {
                if (!_gatewayFactory.IsOnlinePayment(type))
                {
                    return new GeneralResponse<PaymentSessionResponse>
                    {
                        Success = true,
                        Message = $"{type} selected, no online session required.",
                        Data = new PaymentSessionResponse
                        {
                            PaymentType = type,
                            InvoiceId = dto.InvoiceId,
                            Amount = dto.Cost,
                            Currency = dto.Currency,
                            PaymentStatus = PaymentStatus.Pending
                        }
                    };
                }

                var gateway = _gatewayFactory.GetGateway(type);
                return await gateway.CreatePaymentSessionAsync(dto);
            }
            catch (NotSupportedException ex)
            {
                return new GeneralResponse<PaymentSessionResponse>(false, $"Unsupported gateway: {ex.Message}");
            }
            catch (Exception ex)
            {
                return new GeneralResponse<PaymentSessionResponse>(false,
                    $"Gateway error while creating session: {ex.InnerException?.Message ?? ex.Message}");
            }
        }

        public async Task<GeneralResponse<bool>> CancelPaymentAsync(string paymentId, PaymentType paymentType)
        {
            try
            {
                if (!_gatewayFactory.IsOnlinePayment(paymentType))
                {
                    return new GeneralResponse<bool>
                    {
                        Success = true,
                        Message = $"{paymentType} payments are handled offline",
                        Data = true
                    };
                }

                var gateway = _gatewayFactory.GetGateway(paymentType);
                return await gateway.CancelPaymentAsync(paymentId);
            }
            catch (NotSupportedException ex)
            {
                return new GeneralResponse<bool>
                {
                    Success = false,
                    Message = ex.Message
                };
            }
        }

        public async Task<GeneralResponse<PaymentStatusResponse>> GetPaymentStatusAsync(string paymentId, PaymentType paymentType)
        {
            try
            {
                if (!_gatewayFactory.IsOnlinePayment(paymentType))
                {
                    return new GeneralResponse<PaymentStatusResponse>
                    {
                        Success = true,
                        Message = $"{paymentType} status checked offline",
                        Data = new PaymentStatusResponse
                        {
                            PaymentId = paymentId,
                            Status = PaymentStatus.Completed,
                            LastUpdated = GetSaudiTime.Now()
                        }
                    };
                }

                var gateway = _gatewayFactory.GetGateway(paymentType);
                return await gateway.GetPaymentStatusAsync(paymentId);
            }
            catch (NotSupportedException ex)
            {
                return new GeneralResponse<PaymentStatusResponse>
                {
                    Success = false,
                    Message = ex.Message
                };
            }
        }
    }
}