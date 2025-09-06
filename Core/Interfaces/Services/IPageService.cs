using invoice.Core.DTO;
using invoice.Core.DTO.Page;

namespace invoice.Core.Interfaces.Services
{
    public interface IPageService
    {
        Task<GeneralResponse<IEnumerable<PageReadDTO>>> GetAllAsync(string storeId = null, string languageId = null);
        Task<GeneralResponse<PageReadDTO>> GetByIdAsync(string id);
        Task<GeneralResponse<PageReadDTO>> GetByTitleAsync(string title, string storeId = null, string languageId = null);
        Task<GeneralResponse<IEnumerable<PageReadDTO>>> SearchAsync(string keyword, string storeId = null, string languageId = null);
        Task<int> CountAsync(string storeId = null, string languageId = null);
        Task<bool> ExistsAsync(string id);

        Task<GeneralResponse<PageReadDTO>> CreateAsync(PageCreateDTO dto, PageImageDTO image);
        Task<GeneralResponse<IEnumerable<PageReadDTO>>> CreateRangeAsync(PageCreateRangeRequest request);

        Task<GeneralResponse<PageReadDTO>> UpdateAsync(PageUpdateDTO dto, PageImageDTO image);
        Task<GeneralResponse<PageReadDTO>> UpdateImageAsync(string id, IFormFile image);
        Task<GeneralResponse<IEnumerable<PageReadDTO>>> UpdateRangeAsync(PageUpdateRangeRequest request);

        Task<GeneralResponse<bool>> DeleteAsync(string id);
        Task<GeneralResponse<bool>> DeleteRangeAsync(IEnumerable<string> ids);
    }
}
