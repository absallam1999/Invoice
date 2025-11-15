using invoice.Core.DTO;
using invoice.Core.DTO.Language;
using invoice.Core.Entities;
using invoice.Core.Enums;

namespace invoice.Core.Interfaces.Services
{
    public interface ILanguageService
    {
        Task<GeneralResponse<IEnumerable<Language>>> GetAllAsync();
        Task<GeneralResponse<Language>> GetByIdAsync(string id);

        Task<GeneralResponse<IEnumerable<Language>>> GetByNameAsync(LanguageName name);
        Task<GeneralResponse<IEnumerable<Language>>> GetByTargetAsync(LanguageTarget target);
        Task<GeneralResponse<IEnumerable<Language>>> SearchAsync(string keyword);

        Task<GeneralResponse<Language>> CreateAsync(CreateLanguageDTO language);
        Task<GeneralResponse<IEnumerable<Language>>> CreateRangeAsync(IEnumerable<CreateLanguageDTO> languages);

        Task<GeneralResponse<Language>> UpdateAsync(string id, UpdateLanguageDTO language);
        Task<GeneralResponse<IEnumerable<Language>>> UpdateRangeAsync(IEnumerable<UpdateLanguageDTO> languages);

        Task<GeneralResponse<bool>> DeleteAsync(string id);
        Task<GeneralResponse<bool>> DeleteRangeAsync(IEnumerable<string> ids);

        Task<bool> ExistsAsync(string id);
        Task<int> CountAsync();
    }
}