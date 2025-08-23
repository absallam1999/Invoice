using invoice.Core.DTO;
using invoice.Core.DTO.ContactInformation;

namespace invoice.Core.Interfaces.Services
{
    public interface IContactInfoService
    {
        Task<GeneralResponse<IEnumerable<ContactInfoReadDTO>>> GetAllAsync(string userId);
        Task<GeneralResponse<ContactInfoReadDTO>> GetByIdAsync(string id, string userId);
        Task<GeneralResponse<IEnumerable<ContactInfoReadDTO>>> GetByStoreIdAsync(string storeId, string userId);
        Task<GeneralResponse<ContactInfoReadDTO>> GetByEmailAsync(string email, string userId);
        Task<GeneralResponse<IEnumerable<ContactInfoReadDTO>>> SearchAsync(string keyword, string userId);

        Task<GeneralResponse<ContactInfoReadDTO>> CreateAsync(ContactInfoCreateDTO dto, string userId);
        Task<GeneralResponse<IEnumerable<ContactInfoReadDTO>>> CreateRangeAsync(IEnumerable<ContactInfoCreateDTO> dtos, string userId);

        Task<GeneralResponse<ContactInfoReadDTO>> UpdateAsync(string id, ContactInfoUpdateDTO dto, string userId);
        Task<GeneralResponse<IEnumerable<ContactInfoReadDTO>>> UpdateRangeAsync(IEnumerable<ContactInfoUpdateDTO> dtos, string userId);

        Task<GeneralResponse<bool>> DeleteAsync(string id, string userId);
        Task<GeneralResponse<bool>> DeleteRangeAsync(IEnumerable<string> ids, string userId);

        Task<bool> ExistsAsync(string id, string userId);
        Task<int> CountAsync(string userId);
    }
}
