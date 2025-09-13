using AutoMapper;
using invoice.Core.DTO;
using invoice.Core.DTO.Payment;
using invoice.Core.Entites;
using invoice.Core.Enums;
using invoice.Core.Interfaces.Services;
using invoice.Repo;
using Microsoft.EntityFrameworkCore;

namespace invoice.Services
{
    public class PaymentService : IPaymentService
    {
        private readonly IRepository<Payment> _paymentRepo;
        private readonly IRepository<Invoice> _invoiceRepo;
        private readonly IPaymentGatewayService _gatewayService;
        private readonly IRepository<PaymentMethod> _paymentMethodRepo;
        private readonly IRepository<PaymentLink> _paymentLinkRepo;
        private readonly IMapper _mapper;

        public PaymentService(
            IRepository<Payment> paymentRepo,
            IRepository<Invoice> invoiceRepo,
            IPaymentGatewayService gatewayService,
            IRepository<PaymentLink> paymentLinkRepo,
            IRepository<PaymentMethod> paymentMethodRepo,
            IMapper mapper)
        {
            _paymentRepo = paymentRepo;
            _invoiceRepo = invoiceRepo;
            _gatewayService = gatewayService;
            _paymentLinkRepo = paymentLinkRepo;
            _paymentMethodRepo = paymentMethodRepo;
            _mapper = mapper;
        }

        public async Task<GeneralResponse<IEnumerable<PaymentReadDTO>>> GetAllAsync(string userId = null)
        {
            var payments = await _paymentRepo.GetAllAsync(userId, x => x.User, x => x.Invoice, x => x.PaymentMethod, x => x.PaymentLink);
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

            var payment = await _paymentRepo.GetByIdAsync(id, userId, q => q.Include(x => x.User).Include(x => x.Invoice).Include(x => x.PaymentMethod).Include(x => x.PaymentLink));
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

            var payments = await _paymentRepo.QueryAsync(p => p.InvoiceId == invoiceId && (userId == null || p.UserId == userId), x => x.User, x => x.PaymentMethod, x => x.PaymentLink);
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

            var payments = await _paymentRepo.QueryAsync(p => p.PaymentMethodId == paymentMethodId && (userId == null || p.UserId == userId), x => x.User, x => x.Invoice, x => x.PaymentLink);
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
                return new GeneralResponse<PaymentReadDTO> { Success = false, Message = "Payment data is required" };

            if (string.IsNullOrWhiteSpace(dto.InvoiceId))
                return new GeneralResponse<PaymentReadDTO> { Success = false, Message = "Invoice ID is required" };

            var invoice = await _invoiceRepo.GetByIdAsync(dto.InvoiceId);
            if (invoice == null)
                return new GeneralResponse<PaymentReadDTO> { Success = false, Message = "Invoice not found" };

            PaymentMethod paymentMethod = null;
            if (!string.IsNullOrEmpty(dto.PaymentMethodId))
            {
                paymentMethod = await _paymentMethodRepo.GetByIdAsync(dto.PaymentMethodId);
            }

            if (paymentMethod == null && Enum.TryParse<PaymentType>(dto.Name, out var paymentType))
            {
                paymentMethod = new PaymentMethod
                {
                    Name = paymentType
                };
                var methodResult = await _paymentMethodRepo.AddAsync(paymentMethod);
                if (!methodResult.Success)
                    return new GeneralResponse<PaymentReadDTO> { Success = false, Message = "Failed to create payment method" };
            }

            if (paymentMethod == null)
                return new GeneralResponse<PaymentReadDTO> { Success = false, Message = "Payment method not found or invalid" };

            var paymentLinks = await _paymentLinkRepo.QueryAsync(
                pl => pl.InvoiceId == invoice.Id && pl.Id == dto.PaymentLinkId
            );

            PaymentLink paymentLink = paymentLinks.FirstOrDefault();

            if (paymentLink == null)
            {
                paymentLink = new PaymentLink
                {
                    InvoiceId = invoice.Id,
                    Value = dto.Cost,
                    PaymentsNumber = "1",
                    Description = $"Payment link for invoice {invoice.Id}",
                    Message = "Generated automatically",
                    Link = string.Empty,
                };

                if (paymentMethod.Name is PaymentType.Stripe or PaymentType.PayPal or PaymentType.ApplePay or PaymentType.GooglePay)
                {
                    var linkResponse = await _gatewayService.CreatePaymentSessionAsync(dto, paymentMethod.Name);
                    if (!linkResponse.Success)
                        return new GeneralResponse<PaymentReadDTO> { Success = false, Message = "Failed to generate payment link" };

                    paymentLink.Link = linkResponse.Data.PaymentUrl;
                }

                var linkResult = await _paymentLinkRepo.AddAsync(paymentLink);
                if (!linkResult.Success)
                    return new GeneralResponse<PaymentReadDTO> { Success = false, Message = "Failed to create payment link" };
            }

            var payment = _mapper.Map<Payment>(dto);
            payment.UserId = userId;
            payment.PaymentMethodId = paymentMethod.Id;
            payment.PaymentLinkId = paymentLink.Id;
            payment.CreatedAt = DateTime.UtcNow;
            payment.UpdatedAt = DateTime.UtcNow;

            var response = await _paymentRepo.AddAsync(payment);
            if (!response.Success)
                return new GeneralResponse<PaymentReadDTO> { Success = false, Message = "Failed to create payment" };

            var resultDto = _mapper.Map<PaymentReadDTO>(response.Data);

            return new GeneralResponse<PaymentReadDTO>
            {
                Success = true,
                Message = "Payment created successfully",
                Data = resultDto
            };
        }

        public async Task<GeneralResponse<IEnumerable<PaymentReadDTO>>> CreateRangeAsync(IEnumerable<PaymentCreateDTO> dtos, string userId)
        {
            if (dtos == null || !dtos.Any())
                return new GeneralResponse<IEnumerable<PaymentReadDTO>> { Success = false, Message = "Payment data is required" };

            var createdPayments = new List<Payment>();

            foreach (var dto in dtos)
            {
                if (string.IsNullOrWhiteSpace(dto.InvoiceId))
                    continue;

                var invoice = await _invoiceRepo.GetByIdAsync(dto.InvoiceId);
                if (invoice == null)
                    continue;

                PaymentMethod paymentMethod = null;
                if (!string.IsNullOrEmpty(dto.PaymentMethodId))
                {
                    paymentMethod = await _paymentMethodRepo.GetByIdAsync(dto.PaymentMethodId);
                }

                if (paymentMethod == null && Enum.TryParse<PaymentType>(dto.Name, out var paymentType))
                {
                    paymentMethod = new PaymentMethod { Name = paymentType };
                    var methodResult = await _paymentMethodRepo.AddAsync(paymentMethod);
                    if (!methodResult.Success)
                        continue;
                }

                if (paymentMethod == null)
                    continue;

                var paymentLinks = await _paymentLinkRepo.QueryAsync(
                    pl => pl.InvoiceId == invoice.Id && pl.Id == dto.PaymentLinkId
                );
                PaymentLink paymentLink = paymentLinks.FirstOrDefault();

                if (paymentLink == null)
                {
                    paymentLink = new PaymentLink
                    {
                        InvoiceId = invoice.Id,
                        Value = dto.Cost,
                        PaymentsNumber = "1",
                        Description = $"Payment link for invoice {invoice.Id}",
                        Message = "Generated automatically",
                        Link = string.Empty,
                    };

                    if (paymentMethod.Name is PaymentType.Stripe or PaymentType.PayPal or PaymentType.ApplePay or PaymentType.GooglePay)
                    {
                        var linkResponse = await _gatewayService.CreatePaymentSessionAsync(dto, paymentMethod.Name);
                        if (!linkResponse.Success)
                            continue;

                        paymentLink.Link = linkResponse.Data.PaymentUrl;
                    }

                    var linkResult = await _paymentLinkRepo.AddAsync(paymentLink);
                    if (!linkResult.Success)
                        continue;
                }

                var payment = _mapper.Map<Payment>(dto);
                payment.UserId = userId;
                payment.PaymentMethodId = paymentMethod.Id;
                payment.PaymentLinkId = paymentLink.Id;
                payment.CreatedAt = DateTime.UtcNow;
                payment.UpdatedAt = DateTime.UtcNow;

                var response = await _paymentRepo.AddAsync(payment);
                if (response.Success)
                {
                    createdPayments.Add(response.Data);
                }
            }

            return new GeneralResponse<IEnumerable<PaymentReadDTO>>
            {
                Success = true,
                Message = createdPayments.Any() ? "Payments created successfully" : "No payments were created",
                Data = _mapper.Map<IEnumerable<PaymentReadDTO>>(createdPayments)
            };
        }

        public async Task<GeneralResponse<PaymentReadDTO>> UpdateAsync(string id, PaymentUpdateDTO dto, string userId)
        {
            if (string.IsNullOrWhiteSpace(id))
                return new GeneralResponse<PaymentReadDTO> { Success = false, Message = "Payment ID is required" };

            if (dto == null)
                return new GeneralResponse<PaymentReadDTO> { Success = false, Message = "Payment data is required" };

            var payment = await _paymentRepo.GetByIdAsync(id, userId);
            if (payment == null)
                return new GeneralResponse<PaymentReadDTO> { Success = false, Message = "Payment not found" };

            PaymentMethod paymentMethod = null;
            if (!string.IsNullOrEmpty(dto.PaymentMethodId))
            {
                paymentMethod = await _paymentMethodRepo.GetByIdAsync(dto.PaymentMethodId);
            }

            if (paymentMethod == null && Enum.TryParse<PaymentType>(dto.Name, out var paymentType))
            {
                paymentMethod = new PaymentMethod { Name = paymentType };
                var methodResult = await _paymentMethodRepo.AddAsync(paymentMethod);
                if (!methodResult.Success)
                    return new GeneralResponse<PaymentReadDTO> { Success = false, Message = "Failed to create payment method" };
            }

            if (paymentMethod != null)
                payment.PaymentMethodId = paymentMethod.Id;

            _mapper.Map(dto, payment);
            payment.UpdatedAt = DateTime.UtcNow;

            var response = await _paymentRepo.UpdateAsync(payment);
            if (!response.Success)
                return new GeneralResponse<PaymentReadDTO> { Success = false, Message = "Failed to update payment" };

            return new GeneralResponse<PaymentReadDTO>
            {
                Success = true,
                Message = "Payment updated successfully",
                Data = _mapper.Map<PaymentReadDTO>(response.Data)
            };
        }

        public async Task<GeneralResponse<IEnumerable<PaymentReadDTO>>> UpdateRangeAsync(IEnumerable<PaymentUpdateDTO> dtos, string userId)
        {
            if (dtos == null || !dtos.Any())
                return new GeneralResponse<IEnumerable<PaymentReadDTO>> { Success = false, Message = "Payment data is required" };

            var updatedPayments = new List<Payment>();

            foreach (var dto in dtos)
            {
                if (string.IsNullOrWhiteSpace(dto.Id))
                    continue;

                var payment = await _paymentRepo.GetByIdAsync(dto.Id, userId);
                if (payment == null)
                    continue;

                PaymentMethod paymentMethod = null;
                if (!string.IsNullOrEmpty(dto.PaymentMethodId))
                {
                    paymentMethod = await _paymentMethodRepo.GetByIdAsync(dto.PaymentMethodId);
                }

                if (paymentMethod == null && Enum.TryParse<PaymentType>(dto.Name, out var paymentType))
                {
                    paymentMethod = new PaymentMethod { Name = paymentType };
                    var methodResult = await _paymentMethodRepo.AddAsync(paymentMethod);
                    if (!methodResult.Success)
                        continue;
                }

                if (paymentMethod != null)
                    payment.PaymentMethodId = paymentMethod.Id;

                _mapper.Map(dto, payment);
                payment.UpdatedAt = DateTime.UtcNow;
                updatedPayments.Add(payment);
            }

            if (!updatedPayments.Any())
                return new GeneralResponse<IEnumerable<PaymentReadDTO>> { Success = false, Message = "No payments found to update" };

            var response = await _paymentRepo.UpdateRangeAsync(updatedPayments);
            if (!response.Success)
                return new GeneralResponse<IEnumerable<PaymentReadDTO>> { Success = false, Message = "Failed to update payments" };

            return new GeneralResponse<IEnumerable<PaymentReadDTO>>
            {
                Success = true,
                Message = "Payments updated successfully",
                Data = _mapper.Map<IEnumerable<PaymentReadDTO>>(response.Data)
            };
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

        public async Task<GeneralResponse<string>> ProcessPaymentAsync(string paymentMethodId, PaymentCreateDTO dto, string userId)
        {
            if (string.IsNullOrWhiteSpace(paymentMethodId))
                return new GeneralResponse<string> { Success = false, Message = "Payment method ID is required" };

            if (dto == null)
                return new GeneralResponse<string> { Success = false, Message = "Payment data is required" };

            var method = await _paymentRepo.GetQueryable()
                .Select(p => p.PaymentMethod)
                .FirstOrDefaultAsync(pm => pm.Id == paymentMethodId);

            if (method == null)
                return new GeneralResponse<string> { Success = false, Message = "Payment method not found" };

            if (method.Name == PaymentType.Cash || method.Name == PaymentType.Delivery)
            {
                var payment = new Payment
                {
                    InvoiceId = dto.InvoiceId,
                    Cost = dto.Cost,
                    Name = dto.Name,
                    PaymentMethodId = paymentMethodId,
                    UserId = userId,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                };

                var result = await _paymentRepo.AddAsync(payment);
                if (!result.Success)
                    return new GeneralResponse<string> { Success = false, Message = "Failed to record payment" };

                return new GeneralResponse<string>
                {
                    Success = true,
                    Message = $"Payment recorded as {method.Name}"
                };
            }

            var sessionResponse = await _gatewayService.CreatePaymentSessionAsync(dto, method.Name);

            if (!sessionResponse.Success)
                return new GeneralResponse<string>
                {
                    Success = false,
                    Message = $"Failed to create payment session: {sessionResponse.Message}"
                };

            return new GeneralResponse<string>
            {
                Success = true,
                Message = $"{method.Name} session created successfully",
                Data = sessionResponse.Data.PaymentUrl
            };
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
    }
}