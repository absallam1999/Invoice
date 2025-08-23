using invoice.Core.DTO;
using invoice.Core.Entites;

namespace invoice.Core.Interfaces.Services
{
    public interface IPageService
    {
        Task<GeneralResponse<IEnumerable<Page>>> GetAllAsync(string storeId = null, string languageId = null);
        Task<GeneralResponse<Page>> GetByIdAsync(string id);
        Task<GeneralResponse<Page>> GetByTitleAsync(string title, string storeId = null, string languageId = null);
        Task<GeneralResponse<IEnumerable<Page>>> SearchAsync(string keyword, string storeId = null, string languageId = null);

        Task<GeneralResponse<Page>> CreateAsync(Page page);
        Task<GeneralResponse<IEnumerable<Page>>> CreateRangeAsync(IEnumerable<Page> pages);

        Task<GeneralResponse<Page>> UpdateAsync(string id, Page page);
        Task<GeneralResponse<IEnumerable<Page>>> UpdateRangeAsync(IEnumerable<Page> pages);

        Task<GeneralResponse<bool>> DeleteAsync(string id);
        Task<GeneralResponse<bool>> DeleteRangeAsync(IEnumerable<string> ids);

        Task<bool> ExistsAsync(string id);
        Task<int> CountAsync(string storeId = null, string languageId = null);
    }
}