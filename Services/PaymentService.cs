using AutoMapper;
using invoice.Core.DTO;
using invoice.Core.DTO.Payment;
using invoice.Core.Entites;
using invoice.Core.Enums;
using invoice.Core.Interfaces.Services;
using invoice.Repo;
using Microsoft.EntityFrameworkCore;
using System.Globalization;

namespace invoice.Services
{
    public class PaymentService : IPaymentService
    {
        private readonly IRepository<Payment> _paymentRepo;
        private readonly IRepository<Invoice> _invoiceRepo;
        private readonly IRepository<PaymentMethod> _paymentMethodRepo;
        private readonly IPaymentGatewayService _gatewayService;
        private readonly IPayInvoiceService _payInvoiceService;
        private readonly IMapper _mapper;

        public PaymentService(
            IRepository<Payment> paymentRepo,
            IRepository<Invoice> invoiceRepo,
            IPaymentGatewayService gatewayService,
            IRepository<PaymentMethod> paymentMethodRepo,
            IPayInvoiceService payInvoiceService,
            IMapper mapper)
        {
            _paymentRepo = paymentRepo;
            _invoiceRepo = invoiceRepo;
            _gatewayService = gatewayService;
            _payInvoiceService = payInvoiceService;
            _paymentMethodRepo = paymentMethodRepo;
            _mapper = mapper;
        }

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

            var payment = await _paymentRepo.GetByIdAsync(id, userId, q => q.Include(x => x.User).Include(x => x.Invoice).Include(x => x.PaymentMethod));
            if (payment == null)
                return new GeneralResponse<PaymentReadDTO> { Success = false, Message = "Payment not found" };

            return new GeneralResponse<PaymentReadDTO>
            {
                Success = true,
                Data = _mapper.Map<PaymentReadDTO>(payment)
            };
        }

        public async Task<GeneralResponse<IEnumerable<PaymentReadDTO>>> GetByInvoiceIdAsync(string invoiceId, string userId = null)
        {
            if (string.IsNullOrWhiteSpace(invoiceId))
                return new GeneralResponse<IEnumerable<PaymentReadDTO>> { Success = false, Message = "Invoice ID is required" };

            var payments = await _paymentRepo.QueryAsync(p => p.InvoiceId == invoiceId && (userId == null || p.UserId == userId), x => x.User, x => x.PaymentMethod);
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

            var payments = await _paymentRepo.QueryAsync(p => p.PaymentMethodId == paymentMethodId && (userId == null || p.UserId == userId), x => x.User, x => x.Invoice);
            return new GeneralResponse<IEnumerable<PaymentReadDTO>>
            {
                Success = true,
                Message = payments.Any() ? "Payments retrieved successfully" : "No payments found for this payment method",
                Data = _mapper.Map<IEnumerable<PaymentReadDTO>>(payments)
            };
        }

        public async Task<GeneralResponse<PaymentReadDTO>> CreateAsync(PaymentCreateDTO dto, string userId)
        {
            if (dto == null)
                return new GeneralResponse<PaymentReadDTO>(false, "Payment data is required");

            if (string.IsNullOrWhiteSpace(dto.InvoiceId))
                return new GeneralResponse<PaymentReadDTO>(false, "Invoice ID is required");

            var invoice = await _invoiceRepo.GetByIdAsync(dto.InvoiceId, userId, q => q.Include(i => i.Payments));
            if (invoice == null)
                return new GeneralResponse<PaymentReadDTO>(false, "Invoice not found");

            PaymentMethod paymentMethod = null;
            if (!string.IsNullOrEmpty(dto.PaymentMethodId))
            {
                paymentMethod = await _paymentMethodRepo.GetByIdAsync(dto.PaymentMethodId);
            }

            if (paymentMethod == null && Enum.TryParse<PaymentType>(dto.Name, true, out var parsedType))
            {
                paymentMethod = new PaymentMethod { Name = parsedType };
                var methodResult = await _paymentMethodRepo.AddAsync(paymentMethod);
                if (!methodResult.Success)
                    return new GeneralResponse<PaymentReadDTO>(false, "Failed to create payment method");
            }

            if (paymentMethod == null)
                return new GeneralResponse<PaymentReadDTO>(false, "Payment method not found or invalid");

            if (paymentMethod.Name is PaymentType.Stripe
                or PaymentType.PayPal
                or PaymentType.ApplePay
                or PaymentType.GooglePay
                or PaymentType.CreditCard
                or PaymentType.DebitCard
                or PaymentType.BankTransfer
                or PaymentType.STCPay
                or PaymentType.Mada)
            {
                var payInvoiceResponse = await _payInvoiceService.CreatePaymentSessionAsync(
                    dto.InvoiceId,
                    paymentMethod.Name,
                    userId
                );

                if (!payInvoiceResponse.Success)
                    return new GeneralResponse<PaymentReadDTO>(false, payInvoiceResponse.Message);

                var paymentDto = _mapper.Map<PaymentReadDTO>(payInvoiceResponse.Data);
                return new GeneralResponse<PaymentReadDTO>(true, "Online payment session created successfully", paymentDto);
            }

            var payment = _mapper.Map<Payment>(dto);
            payment.UserId = userId;
            payment.Cost = dto.Cost;
            payment.PaymentMethodId = paymentMethod.Id;
            payment.Type = paymentMethod.Name;
            payment.CreatedAt = DateTime.UtcNow;
            payment.UpdatedAt = DateTime.UtcNow;

            invoice.Payments.Add(payment);

            var invoiceUpdate = await _invoiceRepo.UpdateAsync(invoice);
            if (!invoiceUpdate.Success)
                return new GeneralResponse<PaymentReadDTO>(false, "Failed to add payment to invoice");

            return new GeneralResponse<PaymentReadDTO>(
                true,
                "Payment created successfully",
                _mapper.Map<PaymentReadDTO>(payment)
            );
        }

        public async Task<GeneralResponse<IEnumerable<PaymentReadDTO>>> CreateRangeAsync(
        IEnumerable<PaymentCreateDTO> dtos, string userId)
        {
            if (dtos == null || !dtos.Any())
                return new GeneralResponse<IEnumerable<PaymentReadDTO>>(false, "No payment data provided");

            var createdPayments = new List<Payment>();
            var skipped = new List<string>();

            foreach (var dto in dtos)
            {
                if (dto == null || string.IsNullOrWhiteSpace(dto.InvoiceId))
                {
                    skipped.Add("Invalid DTO or missing InvoiceId");
                    continue;
                }

                var invoice = await _invoiceRepo.GetByIdAsync(dto.InvoiceId, userId, q => q.Include(i => i.Payments));
                if (invoice == null)
                {
                    skipped.Add($"Invoice {dto.InvoiceId} not found");
                    continue;
                }

                PaymentMethod paymentMethod = null;
                if (!string.IsNullOrEmpty(dto.PaymentMethodId))
                    paymentMethod = await _paymentMethodRepo.GetByIdAsync(dto.PaymentMethodId);

                if (paymentMethod == null && Enum.TryParse<PaymentType>(dto.Name, true, out var parsedType))
                {
                    paymentMethod = new PaymentMethod { Name = parsedType };
                    var methodResult = await _paymentMethodRepo.AddAsync(paymentMethod);
                    if (!methodResult.Success)
                    {
                        skipped.Add($"Invoice {dto.InvoiceId}: Failed to create payment method");
                        continue;
                    }
                }

                if (paymentMethod == null)
                {
                    skipped.Add($"Invoice {dto.InvoiceId}: Invalid payment method");
                    continue;
                }

                if (paymentMethod.Name is PaymentType.Stripe
                    or PaymentType.PayPal
                    or PaymentType.ApplePay
                    or PaymentType.GooglePay
                    or PaymentType.CreditCard
                    or PaymentType.DebitCard
                    or PaymentType.BankTransfer
                    or PaymentType.STCPay
                    or PaymentType.Mada)
                {
                    var payInvoiceResponse = await _payInvoiceService.CreatePaymentSessionAsync(
                        dto.InvoiceId, paymentMethod.Name, userId);

                    if (!payInvoiceResponse.Success)
                    {
                        skipped.Add($"Invoice {dto.InvoiceId}: {payInvoiceResponse.Message}");
                        continue;
                    }

                    var payment = _mapper.Map<Payment>(payInvoiceResponse.Data);
                    payment.UserId = userId;
                    payment.PaymentMethodId = paymentMethod.Id;
                    payment.Type = paymentMethod.Name;

                    invoice.Payments.Add(payment);

                    var updateResult = await _invoiceRepo.UpdateAsync(invoice);
                    if (updateResult.Success)
                        createdPayments.Add(payment);
                    else
                        skipped.Add($"Invoice {dto.InvoiceId}: Failed to update invoice");

                    continue;
                }

                var newPayment = _mapper.Map<Payment>(dto);
                newPayment.UserId = userId;
                newPayment.Cost = dto.Cost;
                newPayment.PaymentMethodId = paymentMethod.Id;
                newPayment.Type = paymentMethod.Name;
                newPayment.CreatedAt = DateTime.UtcNow;
                newPayment.UpdatedAt = DateTime.UtcNow;

                invoice.Payments.Add(newPayment);

                var invoiceUpdate = await _invoiceRepo.UpdateAsync(invoice);
                if (invoiceUpdate.Success)
                    createdPayments.Add(newPayment);
                else
                    skipped.Add($"Invoice {dto.InvoiceId}: Failed to update invoice");
            }

            var success = createdPayments.Any();
            var message = success
                ? $"Created {createdPayments.Count} payments successfully. Skipped {skipped.Count}."
                : "No payments created";

            if (skipped.Any())
                message += " Details: " + string.Join("; ", skipped);

            return new GeneralResponse<IEnumerable<PaymentReadDTO>>(
                success,
                message,
                _mapper.Map<IEnumerable<PaymentReadDTO>>(createdPayments)
            );
        }


        public async Task<GeneralResponse<PaymentReadDTO>> UpdateAsync(string id, PaymentUpdateDTO dto, string userId)
        {
            if (string.IsNullOrWhiteSpace(id))
                return new GeneralResponse<PaymentReadDTO>(false, "Payment ID is required");

            if (dto == null)
                return new GeneralResponse<PaymentReadDTO>(false, "Payment data is required");

            var payment = await _paymentRepo.GetByIdAsync(id, userId);
            if (payment == null)
                return new GeneralResponse<PaymentReadDTO>(false, "Payment not found");

            PaymentMethod paymentMethod = null;
            if (!string.IsNullOrEmpty(dto.PaymentMethodId))
                paymentMethod = await _paymentMethodRepo.GetByIdAsync(dto.PaymentMethodId);

            if (paymentMethod == null && Enum.TryParse<PaymentType>(dto.Name, true, out var parsedType))
            {
                paymentMethod = new PaymentMethod { Name = parsedType };
                var methodResult = await _paymentMethodRepo.AddAsync(paymentMethod);
                if (!methodResult.Success)
                    return new GeneralResponse<PaymentReadDTO>(false, "Failed to create payment method");
            }

            if (paymentMethod != null)
                payment.PaymentMethodId = paymentMethod.Id;

            payment.Name = dto.Name;
            payment.Cost = dto.Cost;
            payment.Status = dto.Status;
            payment.UpdatedAt = DateTime.UtcNow;

            var updateResponse = await _paymentRepo.UpdateAsync(payment);
            if (!updateResponse.Success)
                return new GeneralResponse<PaymentReadDTO>(false, "Failed to update payment");

            if (payment.PaymentMethodId != null &&
                payment.PaymentMethod.Name is PaymentType.Stripe
                    or PaymentType.PayPal
                    or PaymentType.ApplePay
                    or PaymentType.GooglePay
                    or PaymentType.CreditCard
                    or PaymentType.DebitCard
                    or PaymentType.BankTransfer
                    or PaymentType.STCPay
                    or PaymentType.Mada)
            {
                var payInvoiceResult = await _payInvoiceService.GetByPaymentIdAsync(payment.Id, userId);
                if (payInvoiceResult.Success && payInvoiceResult.Data != null)
                {
                    var payInvoice = payInvoiceResult.Data;

                    payInvoice.Amount = payment.Cost;
                    payInvoice.InvoiceId = payment.InvoiceId;
                    payInvoice.PaymentMethodId = payment.PaymentMethodId;
                    payInvoice.PaymentSessionId = payment.GatewaySessionId;

                    if (!string.IsNullOrWhiteSpace(payment.Currency))
                        payInvoice.Currency = payment.Currency;

                    payInvoice.UpdatedAt = DateTime.UtcNow;

                    await _payInvoiceService.UpdateAsync(payInvoice.Id, payInvoice);
                }
            }

            return new GeneralResponse<PaymentReadDTO>(
                true,
                "Payment updated successfully",
                _mapper.Map<PaymentReadDTO>(updateResponse.Data)
            );
        }

        public async Task<GeneralResponse<IEnumerable<PaymentReadDTO>>> UpdateRangeAsync(
            IEnumerable<PaymentUpdateDTO> dtos, string userId)
        {
            if (dtos == null || !dtos.Any())
                return new GeneralResponse<IEnumerable<PaymentReadDTO>>(false, "Payment data is required");

            var updatedPayments = new List<Payment>();
            var skippedPayments = new List<string>();

            foreach (var dto in dtos)
            {
                if (string.IsNullOrWhiteSpace(dto.Id))
                {
                    skippedPayments.Add("Missing ID");
                    continue;
                }

                var payment = await _paymentRepo.GetByIdAsync(dto.Id, userId);
                if (payment == null)
                {
                    skippedPayments.Add(dto.Id);
                    continue;
                }

                PaymentMethod paymentMethod = null;
                if (!string.IsNullOrEmpty(dto.PaymentMethodId))
                    paymentMethod = await _paymentMethodRepo.GetByIdAsync(dto.PaymentMethodId);

                if (paymentMethod == null && Enum.TryParse<PaymentType>(dto.Name, true, out var parsedType))
                {
                    paymentMethod = new PaymentMethod { Name = parsedType };
                    var methodResult = await _paymentMethodRepo.AddAsync(paymentMethod);
                    if (!methodResult.Success)
                    {
                        skippedPayments.Add(dto.Id);
                        continue;
                    }
                }

                if (paymentMethod != null)
                    payment.PaymentMethodId = paymentMethod.Id;

                payment.Name = dto.Name;
                payment.Cost = dto.Cost;
                payment.Status = dto.Status;
                payment.UpdatedAt = DateTime.UtcNow;

                updatedPayments.Add(payment);
            }

            if (!updatedPayments.Any())
                return new GeneralResponse<IEnumerable<PaymentReadDTO>>(false, "No payments found to update");

            var response = await _paymentRepo.UpdateRangeAsync(updatedPayments);
            if (!response.Success)
                return new GeneralResponse<IEnumerable<PaymentReadDTO>>(false, "Failed to update payments");

            foreach (var payment in updatedPayments)
            {
                if (payment.PaymentMethodId != null &&
                    payment.PaymentMethod.Name is PaymentType.Stripe
                        or PaymentType.PayPal
                        or PaymentType.ApplePay
                        or PaymentType.GooglePay
                        or PaymentType.CreditCard
                        or PaymentType.DebitCard
                        or PaymentType.BankTransfer
                        or PaymentType.STCPay
                        or PaymentType.Mada)
                {
                    var payInvoiceResponse = await _payInvoiceService.GetByPaymentIdAsync(payment.Id);
                    if (payInvoiceResponse != null)
                    {
                        var payInvoice = payInvoiceResponse.Data;

                        payInvoice.Amount = payment.Cost;
                        payInvoice.InvoiceId = payment.InvoiceId;
                        payInvoice.PaymentMethodId = payment.PaymentMethodId;
                        payInvoice.PaymentSessionId = payment.GatewaySessionId;

                        if (!string.IsNullOrWhiteSpace(payment.Currency))
                            payInvoice.Currency = payment.Currency;

                        payInvoice.UpdatedAt = DateTime.UtcNow;

        
                        await _payInvoiceService.UpdateAsync(userId, payInvoice);
                    }
                }
            }

            return new GeneralResponse<IEnumerable<PaymentReadDTO>>(
                true,
                $"Updated {updatedPayments.Count} payments successfully. Skipped {skippedPayments.Count}.",
                _mapper.Map<IEnumerable<PaymentReadDTO>>(response.Data)
            );
        }

        public async Task<GeneralResponse<bool>> DeleteAsync(string id, string userId)
        {
            if (string.IsNullOrWhiteSpace(id))
                return new GeneralResponse<bool> { Success = false, Message = "Payment ID is required" };

            var payment = await _paymentRepo.GetByIdAsync(id, userId);
            if (payment == null)
                return new GeneralResponse<bool> { Success = false, Message = "Payment not found", Data = false };

            var response = await _paymentRepo.DeleteAsync(id);
            return new GeneralResponse<bool>
            {
                Success = response.Success,
                Message = response.Success ? "Payment deleted successfully" : response.Message,
                Data = response.Success
            };
        }

        public async Task<GeneralResponse<bool>> DeleteRangeAsync(IEnumerable<string> ids, string userId)
        {
            if (ids == null || !ids.Any())
                return new GeneralResponse<bool> { Success = false, Message = "Payment IDs are required", Data = false };

            var entities = new List<Payment>();
            foreach (var id in ids)
            {
                if (string.IsNullOrWhiteSpace(id))
                    continue;

                var payment = await _paymentRepo.GetByIdAsync(id, userId);
                if (payment != null)
                    entities.Add(payment);
            }

            if (!entities.Any())
                return new GeneralResponse<bool> { Success = false, Message = "No payments found to delete", Data = false };

            var response = await _paymentRepo.DeleteRangeAsync(entities.Select(e => e.Id));
            return new GeneralResponse<bool>
            {
                Success = response.Success,
                Message = response.Success ? "Payments deleted successfully" : response.Message,
                Data = response.Success
            };
        }

        public async Task<bool> ExistsAsync(string id, string userId = null)
        {
            if (string.IsNullOrWhiteSpace(id))
                return false;

            return await _paymentRepo.ExistsAsync(p => p.Id == id && (userId == null || p.UserId == userId));
        }

        public async Task<int> CountAsync(string userId = null)
        {
            return await _paymentRepo.CountAsync(p => userId == null || p.UserId == userId);
        }

        public async Task<int> CountByInvoiceAsync(string invoiceId, string userId = null)
        {
            if (string.IsNullOrWhiteSpace(invoiceId))
                return 0;

            return await _paymentRepo.CountAsync(p => p.InvoiceId == invoiceId && (userId == null || p.UserId == userId));
        }

        public async Task<int> CountByPaymentMethodAsync(string paymentMethodId, string userId = null)
        {
            if (string.IsNullOrWhiteSpace(paymentMethodId))
                return 0;

            return await _paymentRepo.CountAsync(p => p.PaymentMethodId == paymentMethodId && (userId == null || p.UserId == userId));
        }

        public async Task<GeneralResponse<string>> ProcessPaymentAsync(string paymentMethodId, PaymentCreateDTO dto, string userId)
        {
            if (dto == null)
                return new GeneralResponse<string>(false, "Payment data is required");

            if (string.IsNullOrWhiteSpace(dto.InvoiceId))
                return new GeneralResponse<string>(false, "Invoice ID is required");

            var invoice = await _invoiceRepo.GetByIdAsync(dto.InvoiceId, userId, q => q.Include(i => i.Payments));
            if (invoice == null)
                return new GeneralResponse<string>(false, "Invoice not found");

            PaymentMethod paymentMethod = null;

            if (!string.IsNullOrWhiteSpace(paymentMethodId))
            {
                paymentMethod = await _paymentMethodRepo.GetByIdAsync(paymentMethodId);
            }

            if (paymentMethod == null && Enum.TryParse<PaymentType>(dto.Name, true, out var parsedType))
            {
                paymentMethod = new PaymentMethod { Name = parsedType };
                var methodResult = await _paymentMethodRepo.AddAsync(paymentMethod);
                if (!methodResult.Success)
                    return new GeneralResponse<string>(false, "Failed to create payment method");
            }

            if (paymentMethod == null)
                return new GeneralResponse<string>(false, "Payment method not found or invalid");

            if (paymentMethod.Name is PaymentType.Cash or PaymentType.Delivery)
            {
                var payment = _mapper.Map<Payment>(dto);
                payment.UserId = userId;
                payment.Cost = dto.Cost;
                payment.PaymentMethodId = paymentMethod.Id;
                payment.Type = paymentMethod.Name;
                payment.Status = PaymentStatus.Completed;
                payment.CreatedAt = DateTime.UtcNow;
                payment.UpdatedAt = DateTime.UtcNow;

                invoice.Payments.Add(payment);

                var invoiceUpdate = await _invoiceRepo.UpdateAsync(invoice);
                if (!invoiceUpdate.Success)
                    return new GeneralResponse<string>(false, "Failed to record payment");

                return new GeneralResponse<string>(
                    true,
                    $"Payment recorded as {paymentMethod.Name}"
                );
            }

            var sessionResponse = await _gatewayService.CreatePaymentSessionAsync(dto, paymentMethod.Name);
            if (!sessionResponse.Success)
            {
                return new GeneralResponse<string>(
                    false,
                    $"Failed to create payment session: {sessionResponse.Message}"
                );
            }

            return new GeneralResponse<string>(
                true,
                $"{paymentMethod.Name} session created successfully",
                sessionResponse.Data.PaymentUrl
            );
        }

        public async Task<GeneralResponse<bool>> RefundPaymentAsync(string paymentId, string userId)
        {
            if (string.IsNullOrWhiteSpace(paymentId))
                return new GeneralResponse<bool> { Success = false, Message = "Payment ID is required" };

            var payment = await _paymentRepo.GetByIdAsync(paymentId, userId);
            if (payment == null)
                return new GeneralResponse<bool> { Success = false, Message = "Payment not found" };

            if (string.IsNullOrWhiteSpace(payment.GatewaySessionId))
                return new GeneralResponse<bool> { Success = false, Message = "Cannot refund a non-gateway payment" };

            var refundResult = await _gatewayService.RefundPaymentAsync(payment.GatewaySessionId, payment.Type);
            if (!refundResult.Success)
            {
                return new GeneralResponse<bool> { Success = false, Message = $"Refund failed: {refundResult.Message}" };
            }

            payment.Status = PaymentStatus.Refunded;
            payment.UpdatedAt = DateTime.UtcNow;
            await _paymentRepo.UpdateAsync(payment);

            return new GeneralResponse<bool> { Success = true, Message = "Payment refunded successfully" };
        }

        public async Task<GeneralResponse<bool>> CancelPaymentAsync(string paymentId, string userId)
        {
            if (string.IsNullOrWhiteSpace(paymentId))
                return new GeneralResponse<bool> { Success = false, Message = "Payment ID is required" };

            var payment = await _paymentRepo.GetByIdAsync(paymentId, userId);
            if (payment == null)
                return new GeneralResponse<bool> { Success = false, Message = "Payment not found" };

            if (!string.IsNullOrWhiteSpace(payment.GatewaySessionId))
            {
                var cancelResp = await _gatewayService.CancelPaymentAsync(payment.GatewaySessionId, payment.Type);
                if (!cancelResp.Success)
                    return new GeneralResponse<bool> { Success = false, Message = $"Gateway cancel failed: {cancelResp.Message}" };
            }

            payment.Status = PaymentStatus.Cancelled;
            payment.UpdatedAt = DateTime.UtcNow;
            var updateResp = await _paymentRepo.UpdateAsync(payment);

            return new GeneralResponse<bool>
            {
                Success = updateResp.Success,
                Message = updateResp.Success ? "Payment cancelled successfully" : updateResp.Message
            };
        }

        public async Task<GeneralResponse<bool>> MarkAsCompletedAsync(string paymentId, string userId)
        {
            if (string.IsNullOrWhiteSpace(paymentId))
                return new GeneralResponse<bool> { Success = false, Message = "Payment ID is required" };

            var payment = await _paymentRepo.GetByIdAsync(paymentId, userId);
            if (payment == null)
                return new GeneralResponse<bool> { Success = false, Message = "Payment not found" };

            payment.Status = PaymentStatus.Completed;
            payment.UpdatedAt = DateTime.UtcNow;

            var resp = await _paymentRepo.UpdateAsync(payment);
            return new GeneralResponse<bool> { Success = resp.Success, Message = resp.Success ? "Payment marked as completed" : resp.Message };
        }

        public async Task<GeneralResponse<bool>> MarkAsFailedAsync(string paymentId, string failureReason, string userId)
        {
            if (string.IsNullOrWhiteSpace(paymentId))
                return new GeneralResponse<bool> { Success = false, Message = "Payment ID is required" };

            var payment = await _paymentRepo.GetByIdAsync(paymentId, userId);
            if (payment == null)
                return new GeneralResponse<bool> { Success = false, Message = "Payment not found" };

            payment.Status = PaymentStatus.Failed;
            payment.UpdatedAt = DateTime.UtcNow;

            var resp = await _paymentRepo.UpdateAsync(payment);
            return new GeneralResponse<bool> { Success = resp.Success, Message = resp.Success ? "Payment marked as failed" : resp.Message };
        }

        public async Task<GeneralResponse<decimal>> GetTotalPaidByInvoiceAsync(string invoiceId, string userId = null)
        {
            if (string.IsNullOrWhiteSpace(invoiceId))
                return new GeneralResponse<decimal> { Success = false, Message = "Invoice ID is required" };

            var payments = await _paymentRepo.QueryAsync(p => p.InvoiceId == invoiceId && (userId == null || p.UserId == userId));
            return new GeneralResponse<decimal>
            {
                Success = true,
                Message = "Total paid amount retrieved successfully",
                Data = payments.Sum(p => p.Cost)
            };
        }

        public async Task<GeneralResponse<decimal>> GetTotalPaidByUserAsync(string userId)
        {
            if (string.IsNullOrWhiteSpace(userId))
                return new GeneralResponse<decimal> { Success = false, Message = "User ID is required" };

            var payments = await _paymentRepo.QueryAsync(p => p.UserId == userId);
            var total = payments.Sum(p => p.Cost);
            return new GeneralResponse<decimal> { Success = true, Data = total, Message = "Total paid by user retrieved" };
        }

        public async Task<GeneralResponse<decimal>> GetTotalPaidByPaymentMethodAsync(string paymentMethodId, string userId = null)
        {
            if (string.IsNullOrWhiteSpace(paymentMethodId))
                return new GeneralResponse<decimal> { Success = false, Message = "Payment method ID is required" };

            var payments = await _paymentRepo.QueryAsync(p => p.PaymentMethodId == paymentMethodId && (userId == null || p.UserId == userId));
            var total = payments.Sum(p => p.Cost);
            return new GeneralResponse<decimal> { Success = true, Data = total, Message = "Total paid by payment method retrieved" };
        }

        public async Task<GeneralResponse<IDictionary<string, decimal>>> GetMonthlyRevenueAsync(int year, string userId = null)
        {
            var months = Enumerable.Range(1, 12).ToDictionary(m => CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(m), m => 0m);

            var query = _paymentRepo.GetQueryable().AsNoTracking();
            if (!string.IsNullOrWhiteSpace(userId))
                query = query.Where(p => p.UserId == userId);

            query = query.Where(p => p.CreatedAt.Year == year && p.Status == PaymentStatus.Completed);

            var grouped = await query
                .GroupBy(p => p.CreatedAt.Month)
                .Select(g => new { Month = g.Key, Total = g.Sum(x => x.Cost) })
                .ToListAsync();

            foreach (var g in grouped)
            {
                var name = CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(g.Month);
                months[name] = g.Total;
            }

            return new GeneralResponse<IDictionary<string, decimal>> { Success = true, Data = months, Message = "Monthly revenue retrieved" };
        }

        public async Task<GeneralResponse<IDictionary<string, decimal>>> GetRevenueByPaymentMethodAsync(string userId = null)
        {
            var query = _paymentRepo.GetQueryable()
                                    .AsNoTracking()
                                    .Include(p => p.PaymentMethod)
                                    .Where(p => p.Status == PaymentStatus.Completed);

            if (!string.IsNullOrWhiteSpace(userId))
                query = query.Where(p => p.UserId == userId);

            var grouped = await query
                .GroupBy(p => p.PaymentMethod != null ? p.PaymentMethod.Name.ToString() : "Unknown")
                .Select(g => new { Key = g.Key, Total = g.Sum(x => x.Cost) })
                .ToListAsync();

            var dict = grouped.ToDictionary(g => g.Key ?? "Unknown", g => g.Total);
            return new GeneralResponse<IDictionary<string, decimal>>
            {
                Success = true,
                Data = dict,
                Message = "Revenue by payment method retrieved"
            };
        }

        public async Task<GeneralResponse<bool>> ExpirePaymentAsync(string paymentId, string userId)
        {
            if (string.IsNullOrWhiteSpace(paymentId))
                return new GeneralResponse<bool> { Success = false, Message = "Payment ID is required" };

            var payment = await _paymentRepo.GetByIdAsync(paymentId, userId);
            if (payment == null)
                return new GeneralResponse<bool> { Success = false, Message = "Payment not found" };

            payment.Status = PaymentStatus.Expired;
            payment.UpdatedAt = DateTime.UtcNow;

            var resp = await _paymentRepo.UpdateAsync(payment);
            return new GeneralResponse<bool> { Success = resp.Success, Message = resp.Success ? "Payment expired" : resp.Message };
        }

        public async Task<GeneralResponse<bool>> ReactivatePaymentAsync(string paymentId, string userId)
        {
            if (string.IsNullOrWhiteSpace(paymentId))
                return new GeneralResponse<bool> { Success = false, Message = "Payment ID is required" };

            var payment = await _paymentRepo.GetByIdAsync(paymentId, userId);
            if (payment == null)
                return new GeneralResponse<bool> { Success = false, Message = "Payment not found" };

            payment.Status = PaymentStatus.Pending;
            payment.UpdatedAt = DateTime.UtcNow;

            var resp = await _paymentRepo.UpdateAsync(payment);
            return new GeneralResponse<bool> { Success = resp.Success, Message = resp.Success ? "Payment reactivated" : resp.Message };
        }
    }
}
