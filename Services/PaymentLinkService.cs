using AutoMapper;

using invoice.Core.DTO;
using invoice.Core.DTO.Invoice;
using invoice.Core.DTO.PaymentLink;
using invoice.Core.Entites;
using invoice.Core.Enums;
using invoice.Core.Interfaces.Services;
using invoice.Repo;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace invoice.Services
{
    public class PaymentLinkService : IPaymentLinkService
    {
        private readonly IRepository<PaymentLink> _paymentLinkRepo;
        private readonly IRepository<Invoice> _invoiceRepo;
        //private readonly IPaymentGatewayService _gatewayService;
        private readonly IMapper _mapper;

        public PaymentLinkService(
            IRepository<PaymentLink> paymentLinkRepo,
            //IPaymentGatewayService gatewayService,
            IRepository<Invoice> invoiceRepo,
            IMapper mapper)
        {
            _paymentLinkRepo = paymentLinkRepo;
            //_gatewayService = gatewayService;
            _invoiceRepo = invoiceRepo;
            _mapper = mapper;
        }

        public async Task<GeneralResponse<PaymentLinkReadDTO>> CreateAsync(PaymentLinkCreateDTO dto, string userId)
        {
            var invoice = await _invoiceRepo.GetByIdAsync(dto.InvoiceId, userId);
            if (invoice == null)
                return new GeneralResponse<PaymentLinkReadDTO>
                {
                    Success = false,
                    Message = "Invoice not found"
                };

            var paymentLink = _mapper.Map<PaymentLink>(dto);
            paymentLink.InvoiceId = invoice.Id;

            var response = await _paymentLinkRepo.AddAsync(paymentLink);
            if (!response.Success)
                return new GeneralResponse<PaymentLinkReadDTO>
                {
                    Success = false,
                    Message = "Failed to create payment link"
                };

            return new GeneralResponse<PaymentLinkReadDTO>
            {
                Success = true,
                Message = "Payment link created successfully",
                Data = _mapper.Map<PaymentLinkReadDTO>(response.Data)
            };
        }

        public async Task<GeneralResponse<PaymentLinkReadDTO>> UpdateAsync(string id, PaymentLinkUpdateDTO dto, string userId)
        {
            var existing = await _paymentLinkRepo.GetByIdAsync(id, userId, q => q.Include(p => p.Payments));
            if (existing == null)
                return new GeneralResponse<PaymentLinkReadDTO>
                {
                    Success = false,
                    Message = "Payment link not found"
                };

            _mapper.Map(dto, existing);

            var result = await _paymentLinkRepo.UpdateAsync(existing);
            if (!result.Success)
                return new GeneralResponse<PaymentLinkReadDTO>
                {
                    Success = false,
                    Message = "Failed to update payment link"
                };

            return new GeneralResponse<PaymentLinkReadDTO>
            {
                Success = true,
                Message = "Payment link updated successfully",
                Data = _mapper.Map<PaymentLinkReadDTO>(result.Data)
            };
        }

        public async Task<GeneralResponse<IEnumerable<PaymentLinkReadDTO>>> UpdateRangeAsync(IEnumerable<PaymentLinkUpdateDTO> dtos, string userId)
        {
            var updatedLinks = new List<PaymentLink>();

            foreach (var dto in dtos)
            {
                var existing = await _paymentLinkRepo.GetByIdAsync(dto.Id, userId);
                if (existing == null) continue;

                _mapper.Map(dto, existing);
                var result = await _paymentLinkRepo.UpdateAsync(existing);
                if (result.Success)
                    updatedLinks.Add(result.Data);
            }

            return new GeneralResponse<IEnumerable<PaymentLinkReadDTO>>
            {
                Success = true,
                Message = "Payment links updated successfully",
                Data = _mapper.Map<IEnumerable<PaymentLinkReadDTO>>(updatedLinks)
            };
        }

        public async Task<GeneralResponse<bool>> DeleteAsync(string id, string userId)
        {
            var result = await _paymentLinkRepo.DeleteAsync(id);
            return new GeneralResponse<bool>
            {
                Success = result.Success,
                Message = result.Success ? "Payment link deleted successfully" : result.Message,
                Data = result.Success
            };
        }

        public async Task<GeneralResponse<IEnumerable<bool>>> DeleteRangeAsync(IEnumerable<string> ids, string userId)
        {
            var results = new List<bool>();

            foreach (var id in ids)
            {
                var result = await _paymentLinkRepo.DeleteAsync(id);
                results.Add(result.Success);
            }

            return new GeneralResponse<IEnumerable<bool>>
            {
                Success = true,
                Message = "Batch delete executed",
                Data = results
            };
        }

        public async Task<GeneralResponse<PaymentLinkReadDTO>> GetByIdAsync(string id, string userId)
        {
            var entity = await _paymentLinkRepo.GetByIdAsync(id, userId, q => q.Include(pl => pl.Payments));
            if (entity == null)
                return new GeneralResponse<PaymentLinkReadDTO>
                {
                    Success = false,
                    Message = "Payment link not found"
                };

            return new GeneralResponse<PaymentLinkReadDTO>
            {
                Success = true,
                Message = "Payment link retrieved successfully",
                Data = _mapper.Map<PaymentLinkReadDTO>(entity)
            };
        }

        public async Task<GeneralResponse<IEnumerable<PaymentLinkReadDTO>>> GetAllAsync(string userId)
        {
            var entities = await _paymentLinkRepo.GetAllAsync(userId, q => q.Payments);
            return new GeneralResponse<IEnumerable<PaymentLinkReadDTO>>
            {
                Success = true,
                Message = "Payment links retrieved successfully",
                Data = _mapper.Map<IEnumerable<PaymentLinkReadDTO>>(entities)
            };
        }

        public async Task<GeneralResponse<IEnumerable<PaymentLinkReadDTO>>> QueryAsync(Expression<Func<PaymentLink, bool>> predicate, string userId)
        {
            var entities = await _paymentLinkRepo.QueryAsync(predicate);

            if (!string.IsNullOrEmpty(userId))
                entities = entities.Where(pl => pl.Invoice.UserId == userId);

            return new GeneralResponse<IEnumerable<PaymentLinkReadDTO>>
            {
                Success = true,
                Message = "Query executed successfully",
                Data = _mapper.Map<IEnumerable<PaymentLinkReadDTO>>(entities)
            };
        }

        public async Task<GeneralResponse<PaymentLinkReadDTO>> GenerateLinkAsync(string invoiceId, decimal value, string userId)
        {
            var invoice = await _invoiceRepo.GetByIdAsync(invoiceId, userId);
            if (invoice == null)
                return new GeneralResponse<PaymentLinkReadDTO>
                {
                    Success = false,
                    Message = "Invoice not found"
                };

            var paymentLink = new PaymentLink
            {
                InvoiceId = invoice.Id,
                Value = value,
                PaymentsNumber = "1",
                Description = $"Auto-generated link for invoice {invoice.Id}",
                Message = "Generated automatically",
                Link = string.Empty
            };

            var result = await _paymentLinkRepo.AddAsync(paymentLink);
            if (!result.Success)
                return new GeneralResponse<PaymentLinkReadDTO>
                {
                    Success = false,
                    Message = "Failed to save payment link"
                };

            return new GeneralResponse<PaymentLinkReadDTO>
            {
                Success = true,
                Message = "Payment link generated successfully",
                Data = _mapper.Map<PaymentLinkReadDTO>(result.Data)
            };
        }

        public async Task<GeneralResponse<bool>> AttachPaymentAsync(string paymentLinkId, Payment payment, string userId)
        {
            var paymentLink = await _paymentLinkRepo.GetByIdAsync(paymentLinkId, userId, q => q.Include(pl => pl.Payments));
            if (paymentLink == null)
                return new GeneralResponse<bool>
                {
                    Success = false,
                    Message = "Payment link not found",
                    Data = false
                };

            payment.PaymentLinkId = paymentLink.Id;
            paymentLink.Payments.Add(payment);

            var result = await _paymentLinkRepo.UpdateAsync(paymentLink);
            return new GeneralResponse<bool>
            {
                Success = result.Success,
                Message = result.Success ? "Payment attached successfully" : "Failed to attach payment",
                Data = result.Success
            };
        }

        public async Task<GeneralResponse<InvoiceReadDTO>> RecalculateInvoiceTotalsAsync(string invoiceId, string userId)
        {
            var invoice = await _invoiceRepo.GetByIdAsync(invoiceId, userId, q =>
                q.Include(i => i.Payments)
                 .Include(i => i.InvoiceItems));

            if (invoice == null)
                return new GeneralResponse<InvoiceReadDTO>
                {
                    Success = false,
                    Message = "Invoice not found"
                };

            var itemsTotal = invoice.InvoiceItems.Sum(ii => ii.Quantity);

            decimal finalValue = itemsTotal;
            if (invoice.DiscountType.HasValue && invoice.DiscountValue.HasValue)
            {
                switch (invoice.DiscountType.Value)
                {
                    case DiscountType.Percentage:
                        finalValue -= itemsTotal * (invoice.DiscountValue.Value / 100m);
                        break;
                    case DiscountType.Amount:
                        finalValue -= invoice.DiscountValue.Value;
                        break;
                }
            }
            finalValue = Math.Max(0, finalValue);

            //invoice.TotalPaid = invoice.Payments.Sum(p => p.Cost);
            invoice.FinalValue = finalValue;
            //invoice.RemainingAmount = finalValue - invoice.TotalPaid;

            var result = await _invoiceRepo.UpdateAsync(invoice);

            return new GeneralResponse<InvoiceReadDTO>
            {
                Success = result.Success,
                Message = result.Success ? "Invoice totals recalculated" : "Failed to update invoice totals",
                Data = _mapper.Map<InvoiceReadDTO>(result.Data)
            };
        }

        public async Task<bool> ExistsAsync(string id, string userId)
        {
            var entity = await _paymentLinkRepo.GetByIdAsync(id, userId);
            return entity != null;
        }

        public async Task<int> CountAsync(string userId)
        {
            var all = await _paymentLinkRepo.GetAllAsync(userId);
            return all.Count();
        }
    }
}
