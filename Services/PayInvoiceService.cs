using AutoMapper;
using invoice.Core.DTO;
using invoice.Core.Entities;
using invoice.Core.Interfaces.Services;
using invoice.Repo;
using Microsoft.EntityFrameworkCore;

namespace invoice.Services
{
    public class PayInvoiceService : IPayInvoiceService
    {
        private readonly IRepository<PayInvoice> _payInvoiceRepo;
        private readonly IMapper _mapper;

        public PayInvoiceService(IRepository<PayInvoice> payInvoiceRepo, IMapper mapper)
        {
            _payInvoiceRepo = payInvoiceRepo;
            _mapper = mapper;
        }

        public async Task<GeneralResponse<IEnumerable<PayInvoice>>> GetAllAsync(string userId = null)
        {
            var payInvoices = await _payInvoiceRepo.GetAllAsync(userId, x => x.Invoice, x => x.PaymentMethod);
            return new GeneralResponse<IEnumerable<PayInvoice>>
            {
                Success = true,
                Message = payInvoices.Any() ? "PayInvoices retrieved successfully" : "No pay invoices found",
                Data = payInvoices
            };
        }

        public async Task<GeneralResponse<PayInvoice>> GetByIdAsync(string id, string userId = null)
        {
            var payInvoice = await _payInvoiceRepo.GetByIdAsync(id, userId, q => q.Include(p => p.Invoice).Include(p => p.PaymentMethod));
            if (payInvoice == null)
                return new GeneralResponse<PayInvoice> { Success = false, Message = "PayInvoice not found" };

            return new GeneralResponse<PayInvoice> { Success = true, Data = payInvoice };
        }

        public async Task<GeneralResponse<IEnumerable<PayInvoice>>> GetByInvoiceIdAsync(string invoiceId, string userId = null)
        {
            var payInvoices = await _payInvoiceRepo.QueryAsync(p => p.InvoiceId == invoiceId);
            return new GeneralResponse<IEnumerable<PayInvoice>>
            {
                Success = payInvoices.Any(),
                Message = payInvoices.Any() ? "PayInvoices retrieved successfully" : "No pay invoices found for this invoice",
                Data = payInvoices
            };
        }

        public async Task<GeneralResponse<IEnumerable<PayInvoice>>> GetByPaymentMethodIdAsync(string paymentMethodId, string userId = null)
        {
            var payInvoices = await _payInvoiceRepo.QueryAsync(p => p.PaymentMethodId == paymentMethodId);
            return new GeneralResponse<IEnumerable<PayInvoice>>
            {
                Success = payInvoices.Any(),
                Message = payInvoices.Any() ? "PayInvoices retrieved successfully" : "No pay invoices found for this payment method",
                Data = payInvoices
            };
        }

        public async Task<GeneralResponse<PayInvoice>> CreateAsync(PayInvoice payInvoice)
        {
            var response = await _payInvoiceRepo.AddAsync(payInvoice);
            if (!response.Success)
                return new GeneralResponse<PayInvoice> { Success = false, Message = "Failed to create pay invoice" };

            return new GeneralResponse<PayInvoice> { Success = true, Message = "PayInvoice created successfully", Data = response.Data };
        }

        public async Task<GeneralResponse<IEnumerable<PayInvoice>>> CreateRangeAsync(IEnumerable<PayInvoice> payInvoices)
        {
            var response = await _payInvoiceRepo.AddRangeAsync(payInvoices);
            if (!response.Success)
                return new GeneralResponse<IEnumerable<PayInvoice>> { Success = false, Message = "Failed to create pay invoices" };

            return new GeneralResponse<IEnumerable<PayInvoice>> { Success = true, Message = "PayInvoices created successfully", Data = response.Data };
        }

        public async Task<GeneralResponse<PayInvoice>> UpdateAsync(string id, PayInvoice payInvoice)
        {
            var existing = await _payInvoiceRepo.GetByIdAsync(id);
            if (existing == null)
                return new GeneralResponse<PayInvoice> { Success = false, Message = "PayInvoice not found" };

            _mapper.Map(payInvoice, existing);
            var response = await _payInvoiceRepo.UpdateAsync(existing);

            if (!response.Success)
                return new GeneralResponse<PayInvoice> { Success = false, Message = "Failed to update pay invoice" };

            return new GeneralResponse<PayInvoice> { Success = true, Message = "PayInvoice updated successfully", Data = response.Data };
        }

        public async Task<GeneralResponse<IEnumerable<PayInvoice>>> UpdateRangeAsync(IEnumerable<PayInvoice> payInvoices)
        {
            var ids = payInvoices.Select(p => p.Id).ToList();
            var existingEntities = (await _payInvoiceRepo.GetAllAsync())
                                    .Where(p => ids.Contains(p.Id))
                                    .ToList();

            foreach (var pi in payInvoices)
            {
                var existing = existingEntities.FirstOrDefault(p => p.Id == pi.Id);
                if (existing != null)
                    _mapper.Map(pi, existing);
            }

            var response = await _payInvoiceRepo.UpdateRangeAsync(existingEntities);
            if (!response.Success)
                return new GeneralResponse<IEnumerable<PayInvoice>> { Success = false, Message = "Failed to update pay invoices" };

            return new GeneralResponse<IEnumerable<PayInvoice>> { Success = true, Message = "PayInvoices updated successfully", Data = response.Data };
        }

        public async Task<GeneralResponse<bool>> DeleteAsync(string id)
        {
            var existing = await _payInvoiceRepo.GetByIdAsync(id);
            if (existing == null)
                return new GeneralResponse<bool> { Success = false, Message = "PayInvoice not found", Data = false };

            var response = await _payInvoiceRepo.DeleteAsync(id);
            return new GeneralResponse<bool> { Success = response.Success, Message = response.Message, Data = response.Success };
        }

        public async Task<GeneralResponse<bool>> DeleteRangeAsync(IEnumerable<string> ids)
        {
            var existingEntities = (await _payInvoiceRepo.GetAllAsync())
                                    .Where(p => ids.Contains(p.Id))
                                    .ToList();

            if (!existingEntities.Any())
                return new GeneralResponse<bool> { Success = false, Message = "No pay invoices found to delete", Data = false };

            var response = await _payInvoiceRepo.DeleteRangeAsync(ids);
            return new GeneralResponse<bool> { Success = response.Success, Message = response.Message, Data = response.Success };
        }

        public async Task<bool> ExistsAsync(string id)
        {
            return await _payInvoiceRepo.ExistsAsync(p => p.Id == id);
        }

        public async Task<int> CountAsync(string userId)
        {
            return await _payInvoiceRepo.CountAsync(userId);

        }
    }
}