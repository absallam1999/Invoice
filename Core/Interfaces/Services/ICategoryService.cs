using invoice.Core.DTO.Category;
using invoice.Core.DTO;

namespace invoice.Core.Interfaces.Services
{
    public interface ICategoryService
    {
        Task<GeneralResponse<CategoryReadDTO>> GetByIdAsync(string id, string userId);
        Task<GeneralResponse<IEnumerable<CategoryReadDTO>>> GetAllAsync(string userId);

        Task<GeneralResponse<IEnumerable<CategoryReadDTO>>> GetByUserIdAsync(string userId);
        Task<GeneralResponse<IEnumerable<CategoryReadDTO>>> QueryAsync(string userId, string keyword);

        Task<GeneralResponse<CategoryReadDTO>> CreateAsync(CategoryCreateDTO dto, string userId);
        Task<GeneralResponse<IEnumerable<CategoryReadDTO>>> CreateRangeAsync(IEnumerable<CategoryCreateDTO> dtos, string userId);

        Task<GeneralResponse<CategoryReadDTO>> UpdateAsync(string id, CategoryUpdateDTO dto, string userId);
       // Task<GeneralResponse<IEnumerable<CategoryReadDTO>>> UpdateRangeAsync(IEnumerable<CategoryUpdateRangeDTO> dtos, string userId);
        Task<GeneralResponse<bool>> DeleteAsync(string id, string userId);
        Task<GeneralResponse<bool>> DeleteRangeAsync(IEnumerable<string> ids, string userId);

        Task<bool> ExistsAsync(string id, string userId);
        Task<int> CountAsync(string userId);
    }
}
