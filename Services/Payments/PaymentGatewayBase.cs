using invoice.Core.DTO;
using invoice.Core.DTO.Payment;
using invoice.Core.DTO.PaymentResponse;
using invoice.Core.Enums;
using invoice.Core.Interfaces.Services;
using Microsoft.Extensions.Logging;

namespace invoice.Services.Payments
{
    public abstract class PaymentGatewayBase : IPaymentGateway
    {
        protected readonly string Domain;
        protected readonly IConfiguration Configuration;

        public abstract PaymentType PaymentType { get; }

        protected PaymentGatewayBase(IConfiguration configuration)
        {
            Configuration = configuration;
            Domain = Configuration["AppSettings:BaseUrl"] ?? string.Empty;
        }

        public abstract Task<GeneralResponse<PaymentSessionResponse>> CreatePaymentSessionAsync(PaymentCreateDTO dto);
        public abstract Task<GeneralResponse<bool>> CancelPaymentAsync(string paymentId);
        public abstract Task<GeneralResponse<bool>> RefundPaymentAsync(string paymentId);
        public abstract Task<GeneralResponse<PaymentStatusResponse>> GetPaymentStatusAsync(string paymentId);
        public abstract Task<GeneralResponse<PaymentWebhookResponse>> ProcessWebhookAsync(string payload, string signature);

        protected virtual GeneralResponse<PaymentSessionResponse> ValidatePaymentRequest(PaymentCreateDTO dto)
        {
            if (dto == null)
                return CreateErrorResponse<PaymentSessionResponse>("Payment request cannot be null");

            if (string.IsNullOrWhiteSpace(dto.InvoiceId))
                return CreateErrorResponse<PaymentSessionResponse>("Invoice ID is required");

            if (dto.Cost <= 0)
                return CreateErrorResponse<PaymentSessionResponse>("Payment amount must be greater than zero");

            if (string.IsNullOrWhiteSpace(dto.Currency))
                return CreateErrorResponse<PaymentSessionResponse>("Currency is required");

            return new GeneralResponse<PaymentSessionResponse> { Success = true };
        }

        protected GeneralResponse<T> CreateErrorResponse<T>(string message, string errorCode = null) where T : new()
        {
            return new GeneralResponse<T>
            {
                Success = false,
                Message = message
            };
        }

        protected GeneralResponse<PaymentSessionResponse> CreateSuccessResponse(
            string sessionId,
            string paymentUrl,
            PaymentCreateDTO dto,
            DateTime? expiresAt = null,
            string rawResponse = null)
        {
            return new GeneralResponse<PaymentSessionResponse>
            {
                Success = true,
                Message = "Payment session created successfully",
                Data = new PaymentSessionResponse
                {
                    SessionId = sessionId,
                    PaymentUrl = paymentUrl,
                    ExpiresAt = expiresAt ?? DateTime.UtcNow.AddHours(24),
                    RawResponse = rawResponse,
                    InvoiceId = dto.InvoiceId,
                    Amount = dto.Cost,
                    Currency = dto.Currency,
                    PaymentType = PaymentType
                }
            };
        }

        protected string TruncateString(string input, int maxLength)
        {
            return string.IsNullOrEmpty(input) || input.Length <= maxLength
                ? input
                : input.Substring(0, maxLength - 3) + "...";
        }

        protected string NormalizeDomain(string domain)
        {
            if (string.IsNullOrEmpty(domain))
                return domain;

            if (!domain.EndsWith("/"))
                return domain + "/";

            return domain;
        }
    }
}