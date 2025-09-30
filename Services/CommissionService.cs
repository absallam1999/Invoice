using invoice.Core.DTO;
using invoice.Core.Entities;
using invoice.Core.Interfaces.Services;
using invoice.Repo;

namespace invoice.Services
{
    public class CommissionService : ICommissionService
    {
        private readonly IRepository<Commission> _commissionRepository;

        public CommissionService(IRepository<Commission> commissionRepository)
        {
            _commissionRepository = commissionRepository;
        }

        public async Task<GeneralResponse<Commission>> GetByIdAsync(string id)
        {
            var entity = await _commissionRepository.GetByIdAsync(id);
            return entity == null
                ? new GeneralResponse<Commission>(false, "Commission not found")
                : new GeneralResponse<Commission>(true, "Commission retrieved successfully", entity);
        }

        public async Task<GeneralResponse<IEnumerable<Commission>>> GetAllAsync()
        {
            var entities = await _commissionRepository.GetAllAsync();
            return new GeneralResponse<IEnumerable<Commission>>(true, "Commissions retrieved successfully", entities);
        }

        public async Task<GeneralResponse<IEnumerable<Commission>>> GetBySellerIdAsync(string sellerId)
        {
            var entities = await _commissionRepository.QueryAsync(c => c.SellerId == sellerId);
            return new GeneralResponse<IEnumerable<Commission>>(true, "Commissions retrieved successfully", entities);
        }

        public async Task<GeneralResponse<Commission>> AddAsync(Commission commission)
        {
            return await _commissionRepository.AddAsync(commission);
        }

        public async Task<GeneralResponse<Commission>> UpdateAsync(Commission commission)
        {
            return await _commissionRepository.UpdateAsync(commission);
        }

        public async Task<GeneralResponse<Commission>> DeleteAsync(string id)
        {
            return await _commissionRepository.DeleteAsync(id);
        }

        public async Task<GeneralResponse<Commission>> SettleCommissionAsync(string commissionId)
        {
            var entity = await _commissionRepository.GetByIdAsync(commissionId);
            if (entity == null)
                return new GeneralResponse<Commission>(false, "Commission not found");

            entity.Settled = true;
            return await _commissionRepository.UpdateAsync(entity);
        }

        public async Task<GeneralResponse<decimal>> GetUnsettledTotalForSellerAsync(string sellerId)
        {
            var commissions = await _commissionRepository.QueryAsync(c => c.SellerId == sellerId && !c.Settled);
            var total = commissions.Sum(c => c.Value);
            return new GeneralResponse<decimal>(true, "Unsettled total calculated", total);
        }

        public async Task<GeneralResponse<decimal>> GetTotalForSellerAsync(string sellerId)
        {
            var commissions = await _commissionRepository.QueryAsync(c => c.SellerId == sellerId);
            var total = commissions.Sum(c => c.Value);
            return new GeneralResponse<decimal>(true, "Total calculated", total);
        }

        public async Task<GeneralResponse<decimal>> GetTotalSettledForSellerAsync(string sellerId)
        {
            var commissions = await _commissionRepository.QueryAsync(c => c.SellerId == sellerId && c.Settled);
            var total = commissions.Sum(c => c.Value);
            return new GeneralResponse<decimal>(true, "Settled total calculated", total);
        }

        public async Task<GeneralResponse<decimal>> GetTotalCommissionsAsync()
        {
            var commissions = await _commissionRepository.GetAllAsync();
            var total = commissions.Sum(c => c.Value);
            return new GeneralResponse<decimal>(true, "Total commissions calculated", total);
        }

        public async Task<GeneralResponse<decimal>> GetTotalSettledCommissionsAsync()
        {
            var commissions = await _commissionRepository.QueryAsync(c => c.Settled);
            var total = commissions.Sum(c => c.Value);
            return new GeneralResponse<decimal>(true, "Total settled commissions calculated", total);
        }

        public async Task<GeneralResponse<decimal>> GetTotalUnsettledCommissionsAsync()
        {
            var commissions = await _commissionRepository.QueryAsync(c => !c.Settled);
            var total = commissions.Sum(c => c.Value);
            return new GeneralResponse<decimal>(true, "Total unsettled commissions calculated", total);
        }

        public async Task<GeneralResponse<Commission>> GetByInvoiceIdAsync(string invoiceId)
        {
            var commission = await _commissionRepository.GetSingleByUserIdAsync(null,
                q => q.Where(c => c.InvoiceId == invoiceId));
            return commission == null
                ? new GeneralResponse<Commission>(false, "Commission not found for this invoice")
                : new GeneralResponse<Commission>(true, "Commission retrieved successfully", commission);
        }

        public async Task<GeneralResponse<int>> CountAsync(string sellerId = null)
        {
            var count = string.IsNullOrEmpty(sellerId)
                ? await _commissionRepository.CountAsync()
                : await _commissionRepository.CountAsync(predicate: c => c.SellerId == sellerId);

            return new GeneralResponse<int>(true, "Commission count retrieved successfully", count);
        }

        public async Task<GeneralResponse<bool>> ExistsAsync(string commissionId)
        {
            var exists = await _commissionRepository.ExistsAsync(c => c.Id == commissionId);
            return new GeneralResponse<bool>(true, "Commission existence check completed", exists);
        }
    }
}
