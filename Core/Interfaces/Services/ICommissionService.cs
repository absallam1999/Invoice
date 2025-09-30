using invoice.Core.Entities;
using invoice.Core.DTO;

namespace invoice.Core.Interfaces.Services
{
    public interface ICommissionService
    {
        Task<GeneralResponse<Commission>> GetByIdAsync(string id);
        Task<GeneralResponse<IEnumerable<Commission>>> GetAllAsync();
        Task<GeneralResponse<IEnumerable<Commission>>> GetBySellerIdAsync(string sellerId);
        Task<GeneralResponse<Commission>> AddAsync(Commission commission);
        Task<GeneralResponse<Commission>> UpdateAsync(Commission commission);
        Task<GeneralResponse<Commission>> DeleteAsync(string id);

        Task<GeneralResponse<Commission>> SettleCommissionAsync(string commissionId);

        Task<GeneralResponse<decimal>> GetUnsettledTotalForSellerAsync(string sellerId);
        Task<GeneralResponse<decimal>> GetTotalForSellerAsync(string sellerId);
        Task<GeneralResponse<decimal>> GetTotalSettledForSellerAsync(string sellerId);

        Task<GeneralResponse<decimal>> GetTotalCommissionsAsync();
        Task<GeneralResponse<decimal>> GetTotalSettledCommissionsAsync();
        Task<GeneralResponse<decimal>> GetTotalUnsettledCommissionsAsync();

        Task<GeneralResponse<Commission>> GetByInvoiceIdAsync(string invoiceId);

        Task<GeneralResponse<int>> CountAsync(string sellerId = null);
        Task<GeneralResponse<bool>> ExistsAsync(string commissionId);
    }
}
