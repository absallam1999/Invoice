using AutoMapper;
using invoice.Core.DTO;
using invoice.Core.DTO.Payment;
using invoice.Core.Entities;
using invoice.Core.Enums;
using invoice.Core.Interfaces.Services;
using invoice.Helpers;
using invoice.Repo;
using Microsoft.EntityFrameworkCore;
using System.Globalization;

namespace invoice.Services
{
    public class PaymentService : IPaymentService
    {
        private readonly IMapper _mapper;
        private readonly ILogger<PaymentService> _logger;
        private readonly IRepository<Payment> _paymentRepo;
        private readonly IRepository<Invoice> _invoiceRepo;
        private readonly IPaymentGatewayService _gatewayService;
        private readonly IRepository<PaymentMethod> _paymentMethodRepo;

        public PaymentService(
            IRepository<PaymentMethod> paymentMethodRepo,
            IRepository<Payment> paymentRepo,
            IRepository<Invoice> invoiceRepo,
            IPaymentGatewayService gatewayService,
            ILogger<PaymentService> logger,
            IMapper mapper)
        {
            _paymentRepo = paymentRepo;
            _invoiceRepo = invoiceRepo;
            _paymentMethodRepo = paymentMethodRepo;
            _gatewayService = gatewayService;
            _mapper = mapper;
            _logger = logger;
        }

        #region CRUD / Reads

        public async Task<GeneralResponse<IEnumerable<PaymentReadDTO>>> GetAllAsync(string userId = null)
        {
            var payments = await _paymentRepo.GetAllAsync(userId, x => x.User, x => x.Invoice, x => x.PaymentMethod);
            return new GeneralResponse<IEnumerable<PaymentReadDTO>>
            {
                Success = true,
                Message = payments.Any() ? "Payments retrieved successfully" : "No payments found",
                Data = _mapper.Map<IEnumerable<PaymentReadDTO>>(payments)
            };
        }

        public async Task<GeneralResponse<PaymentReadDTO>> GetByIdAsync(string id, string userId = null)
        {
            if (string.IsNullOrWhiteSpace(id))
                return new GeneralResponse<PaymentReadDTO> { Success = false, Message = "Payment ID is required" };

            var payment = await _paymentRepo.GetByIdAsync(id, userId, q => q
                .Include(x => x.User)
                .Include(x => x.Invoice)
                .Include(x => x.PaymentMethod));

            if (payment == null)
                return new GeneralResponse<PaymentReadDTO> { Success = false, Message = "Payment not found" };

            return new GeneralResponse<PaymentReadDTO>
            {
                Success = true,
                Message = "Payment retrieved successfully",
                Data = _mapper.Map<PaymentReadDTO>(payment)
            };
        }

        public async Task<GeneralResponse<IEnumerable<PaymentReadDTO>>> GetByInvoiceIdAsync(string invoiceId, string userId = null)
        {
            if (string.IsNullOrWhiteSpace(invoiceId))
                return new GeneralResponse<IEnumerable<PaymentReadDTO>> { Success = false, Message = "Invoice ID is required" };

            var payments = await _paymentRepo.QueryAsync(
                p => p.InvoiceId == invoiceId && (userId == null || p.UserId == userId),
                x => x.User, x => x.PaymentMethod);

            return new GeneralResponse<IEnumerable<PaymentReadDTO>>
            {
                Success = true,
                Message = payments.Any() ? "Payments retrieved successfully" : "No payments found for this invoice",
                Data = _mapper.Map<IEnumerable<PaymentReadDTO>>(payments)
            };
        }

        public async Task<GeneralResponse<IEnumerable<PaymentReadDTO>>> GetByPaymentMethodIdAsync(string paymentMethodId, string userId = null)
        {
            if (string.IsNullOrWhiteSpace(paymentMethodId))
                return new GeneralResponse<IEnumerable<PaymentReadDTO>> { Success = false, Message = "Payment method ID is required" };

            var payments = await _paymentRepo.QueryAsync(
                p => p.PaymentMethodId == paymentMethodId && (userId == null || p.UserId == userId),
                x => x.User, x => x.Invoice);

            return new GeneralResponse<IEnumerable<PaymentReadDTO>>
            {
                Success = true,
                Message = payments.Any() ? "Payments retrieved successfully" : "No payments found for this method",
                Data = _mapper.Map<IEnumerable<PaymentReadDTO>>(payments)
            };
        }

        #endregion

        #region Create / Gateway Integration
        public async Task<GeneralResponse<PaymentReadDTO>> CreateAsync(PaymentCreateDTO dto, string userId)
        {
            if (dto == null)
                return new GeneralResponse<PaymentReadDTO>(false, "Payment data is required.");

            var invoice = await _invoiceRepo.GetByIdAsync(dto.InvoiceId, userId, q => q.Include(i => i.Payments));

            if (invoice == null)
                return new GeneralResponse<PaymentReadDTO>(false, "Invoice not found.");

            if (invoice.Payments?.Any() == true)
                return new GeneralResponse<PaymentReadDTO>(false,
                    $"Invoice with ID: #{invoice.Id} already has existing payment.");

            var payment = _mapper.Map<Payment>(dto);
            payment.UserId = userId;
            payment.Status = PaymentStatus.Pending;
            payment.Type = await DeterminePaymentTypeAsync(dto.PaymentMethodId);
            payment.ExpiresAt ??= GetSaudiTime.Now().AddDays(7);

            payment.GatewaySessionId = Guid.NewGuid().ToString();
            payment.Link = payment.Link ?? string.Empty;

            try
            {
                if (payment.Type != PaymentType.Cash && payment.Type != PaymentType.Delivery)
                {
                    var sessionResponse = await _gatewayService.CreatePaymentSessionAsync(dto, payment.Type);

                    if (sessionResponse.Success && sessionResponse.Data != null)
                    {
                        payment.GatewaySessionId = sessionResponse.Data.SessionId ?? payment.GatewaySessionId;
                        payment.Link = sessionResponse.Data.PaymentUrl ?? payment.Link;
                        payment.ExpiresAt = sessionResponse.Data.ExpiresAt == default
                            ? payment.ExpiresAt
                            : sessionResponse.Data.ExpiresAt;
                        payment.Status = sessionResponse.Data.PaymentStatus;

                        _logger.LogInformation(
                            "Payment {PaymentId} session created for gateway {Gateway} (Session={SessionId}).",
                            payment.Id, payment.Type, payment.GatewaySessionId);
                    }
                    else
                    {
                        _logger.LogWarning("Failed to create gateway session: {Message}", sessionResponse.Message);
                        payment.Status = PaymentStatus.Failed;

                        await _paymentRepo.AddAsync(payment);
                        await _paymentRepo.SaveChangesAsync();

                        return new GeneralResponse<PaymentReadDTO>(false,
                            $"Failed to create gateway session: {sessionResponse.Message}");
                    }
                }
                else
                {
                    _logger.LogInformation("Offline payment created (Type={PaymentType}).", payment.Type);
                }

                await _paymentRepo.AddAsync(payment);
                await _paymentRepo.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating payment or gateway session for Payment {PaymentId}.", payment.Id);
                return new GeneralResponse<PaymentReadDTO>(false,
                    $"Failed to create payment: {ex.Message}");
            }

            var readDto = _mapper.Map<PaymentReadDTO>(payment);
            return new GeneralResponse<PaymentReadDTO>(true, "Payment created successfully.", readDto);
        }

        #endregion

        #region Update / Delete / Existence / Counts

        public async Task<GeneralResponse<PaymentReadDTO>> UpdateAsync(string id, PaymentUpdateDTO dto, string userId)
        {
            var payment = await _paymentRepo.GetByIdAsync(id, userId);
            if (payment == null)
                return new GeneralResponse<PaymentReadDTO> { Success = false, Message = "Payment not found" };

            _mapper.Map(dto, payment);
            payment.UpdatedAt = GetSaudiTime.Now();

            await _paymentRepo.UpdateAsync(payment);
            await _paymentRepo.SaveChangesAsync();

            return new GeneralResponse<PaymentReadDTO>
            {
                Success = true,
                Message = "Payment updated successfully",
                Data = _mapper.Map<PaymentReadDTO>(payment)
            };
        }

        public async Task<GeneralResponse<bool>> DeleteAsync(string id, string userId)
        {
            var payment = await _paymentRepo.GetByIdAsync(id, userId);
            if (payment == null)
                return new GeneralResponse<bool> { Success = false, Message = "Payment not found" };

            payment.IsDeleted = true;
            payment.DeletedAt = GetSaudiTime.Now();
            await _paymentRepo.UpdateAsync(payment);
            await _paymentRepo.SaveChangesAsync();

            return new GeneralResponse<bool> { Success = true, Message = "Payment deleted successfully", Data = true };
        }

        public async Task<bool> ExistsAsync(string id, string userId = null)
            => await _paymentRepo.ExistsAsync(p => p.Id == id && (userId == null || p.UserId == userId));

        public async Task<int> CountAsync(string userId = null)
        {
            try
            {
                return await _paymentRepo.CountAsync(userId);
            }
            catch (MissingMethodException)
            {
                var all = await _paymentRepo.GetAllAsync();
                return all.Count(p => userId == null || p.UserId == userId);
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "CountAsync fallback executed due to repo implementation differences.");
                var all = await _paymentRepo.GetAllAsync();
                return all.Count(p => userId == null || p.UserId == userId);
            }
        }

        public async Task<int> CountByInvoiceAsync(string invoiceId, string userId = null)
        {
            var payments = await _paymentRepo.QueryAsync(p => p.InvoiceId == invoiceId && (userId == null || p.UserId == userId));
            return payments.Count();
        }

        public async Task<int> CountByPaymentMethodAsync(string paymentMethodId, string userId = null)
        {
            var payments = await _paymentRepo.QueryAsync(p => p.PaymentMethodId == paymentMethodId && (userId == null || p.UserId == userId));
            return payments.Count();
        }

        #endregion

        #region Status operations (Cancel / Complete / Fail / Expire / Reactivate)

        public async Task<GeneralResponse<bool>> CancelPaymentAsync(string paymentId, string userId)
        {
            var payment = await _paymentRepo.GetByIdAsync(paymentId, userId);
            if (payment == null) return new GeneralResponse<bool> { Success = false, Message = "Payment not found" };

            if (payment.Status == PaymentStatus.Cancelled)
                return new GeneralResponse<bool> { Success = true, Message = "Payment already cancelled", Data = true };

            try
            {
                var gatewayResult = await _gatewayService.CancelPaymentAsync(paymentId, payment.Type);
                if (!gatewayResult.Success)
                {
                    _logger.LogWarning("Gateway cancel failed for payment {PaymentId}: {Message}", paymentId, gatewayResult.Message);
                    payment.Status = PaymentStatus.Failed;
                }
                else
                {
                    payment.Status = PaymentStatus.Cancelled;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while calling gateway cancel for payment {PaymentId}", paymentId);
                payment.Status = PaymentStatus.Failed;
            }

            await _paymentRepo.UpdateAsync(payment);
            await _paymentRepo.SaveChangesAsync();

            return new GeneralResponse<bool> { Success = true, Message = "Payment cancellation processed", Data = payment.Status == PaymentStatus.Cancelled };
        }

        public async Task<GeneralResponse<bool>> MarkAsCompletedAsync(string paymentId, string userId)
        {
            var payment = await _paymentRepo.GetByIdAsync(paymentId, userId);
            if (payment == null) return new GeneralResponse<bool> { Success = false, Message = "Payment not found" };

            if (payment.Status == PaymentStatus.Completed)
                return new GeneralResponse<bool> { Success = true, Message = "Payment already completed", Data = true };

            payment.Status = PaymentStatus.Completed;
            payment.UpdatedAt = GetSaudiTime.Now();

            await _paymentRepo.UpdateAsync(payment);
            await _paymentRepo.SaveChangesAsync();

            return new GeneralResponse<bool> { Success = true, Message = "Payment marked as completed", Data = true };
        }

        public async Task<GeneralResponse<bool>> MarkAsFailedAsync(string paymentId, string failureReason, string userId)
        {
            var payment = await _paymentRepo.GetByIdAsync(paymentId, userId);
            if (payment == null) return new GeneralResponse<bool> { Success = false, Message = "Payment not found" };

            payment.Status = PaymentStatus.Failed;
            payment.UpdatedAt = GetSaudiTime.Now();

            payment.Link = string.IsNullOrWhiteSpace(payment.Link)
                ? $"?error={Uri.EscapeDataString(failureReason)}"
                : $"{payment.Link}?error={Uri.EscapeDataString(failureReason)}";

            await _paymentRepo.UpdateAsync(payment);
            await _paymentRepo.SaveChangesAsync();

            return new GeneralResponse<bool> { Success = true, Message = "Payment marked as failed", Data = true };
        }

        public async Task<GeneralResponse<bool>> ExpirePaymentAsync(string paymentId, string userId)
        {
            var payment = await _paymentRepo.GetByIdAsync(paymentId, userId);
            if (payment == null) return new GeneralResponse<bool> { Success = false, Message = "Payment not found" };

            if (payment.Status == PaymentStatus.Expired)
                return new GeneralResponse<bool> { Success = true, Message = "Payment already expired", Data = true };

            payment.Status = PaymentStatus.Expired;
            payment.UpdatedAt = GetSaudiTime.Now();

            await _paymentRepo.UpdateAsync(payment);
            await _paymentRepo.SaveChangesAsync();

            return new GeneralResponse<bool> { Success = true, Message = "Payment expired", Data = true };
        }

        public async Task<GeneralResponse<bool>> ReactivatePaymentAsync(string paymentId, string userId)
        {
            var payment = await _paymentRepo.GetByIdAsync(paymentId, userId);
            if (payment == null) return new GeneralResponse<bool> { Success = false, Message = "Payment not found" };

            payment.Status = PaymentStatus.Pending;
            payment.ExpiresAt = GetSaudiTime.Now().AddDays(7);
            payment.UpdatedAt = GetSaudiTime.Now();

            await _paymentRepo.UpdateAsync(payment);
            await _paymentRepo.SaveChangesAsync();

            return new GeneralResponse<bool> { Success = true, Message = "Payment reactivated", Data = true };
        }

        #endregion

        #region Aggregations / Analytics

        public async Task<GeneralResponse<decimal>> GetTotalPaidByInvoiceAsync(string invoiceId, string userId = null)
        {
            if (string.IsNullOrWhiteSpace(invoiceId))
                return new GeneralResponse<decimal> { Success = false, Message = "Invoice ID is required" };

            var payments = await _paymentRepo.QueryAsync(
                p => p.InvoiceId == invoiceId &&
                     p.Status == PaymentStatus.Completed &&
                     (userId == null || p.UserId == userId));

            var total = payments.Any() ? payments.Sum(p => p.Cost) : 0;

            return new GeneralResponse<decimal>
            {
                Success = true,
                Message = "Total calculated successfully",
                Data = total
            };
        }

        public async Task<GeneralResponse<decimal>> GetTotalPaidByPaymentMethodAsync(string paymentMethodId, string userId = null)
        {
            if (string.IsNullOrWhiteSpace(paymentMethodId))
                return new GeneralResponse<decimal> { Success = false, Message = "Payment method ID is required" };

            var payments = await _paymentRepo.QueryAsync(
                p => p.PaymentMethodId == paymentMethodId &&
                     p.Status == PaymentStatus.Completed &&
                     (userId == null || p.UserId == userId));

            var total = payments.Any() ? payments.Sum(p => p.Cost) : 0;

            return new GeneralResponse<decimal>
            {
                Success = true,
                Message = "Total calculated successfully",
                Data = total
            };
        }

        public async Task<GeneralResponse<decimal>> GetTotalPaidByUserAsync(string userId)
        {
            if (string.IsNullOrWhiteSpace(userId))
                return new GeneralResponse<decimal> { Success = false, Message = "User ID is required" };

            var payments = await _paymentRepo.QueryAsync(
                p => p.UserId == userId &&
                     p.Status == PaymentStatus.Completed);

            var total = payments.Any() ? payments.Sum(p => p.Cost) : 0;

            return new GeneralResponse<decimal>
            {
                Success = true,
                Message = "User total payments calculated successfully",
                Data = total
            };
        }

        public async Task<GeneralResponse<IDictionary<string, decimal>>> GetMonthlyRevenueAsync(int year, string userId = null)
        {
            var payments = await _paymentRepo.QueryAsync(p =>
                p.Status == PaymentStatus.Completed &&
                p.CreatedAt.Year == year &&
                (userId == null || p.UserId == userId));

            var monthlyRevenue = payments
                .GroupBy(p => p.CreatedAt.Month)
                .OrderBy(g => g.Key)
                .ToDictionary(
                    g => CultureInfo.InvariantCulture.DateTimeFormat.GetAbbreviatedMonthName(g.Key),
                    g => g.Sum(p => p.Cost)
                );

            return new GeneralResponse<IDictionary<string, decimal>> { Success = true, Data = monthlyRevenue };
        }

        public async Task<GeneralResponse<IDictionary<string, decimal>>> GetRevenueByPaymentMethodAsync(string userId = null)
        {
            var payments = await _paymentRepo.QueryAsync(p =>
                p.Status == PaymentStatus.Completed &&
                (userId == null || p.UserId == userId),
                x => x.PaymentMethod);

            var byMethod = payments
                .GroupBy(p => p.PaymentMethod?.Id ?? "Unknown")
                .ToDictionary(g => g.Key, g => g.Sum(p => p.Cost));

            return new GeneralResponse<IDictionary<string, decimal>> { Success = true, Data = byMethod };
        }

        #endregion

        #region Helpers

        private async Task<PaymentType> DeterminePaymentTypeAsync(string paymentMethodId)
        {
            if (string.IsNullOrWhiteSpace(paymentMethodId))
                return PaymentType.None;

            var method = await _paymentMethodRepo.GetByIdAsync(paymentMethodId);
            return method?.Name ?? PaymentType.None;
        }

        #endregion
    }
}
