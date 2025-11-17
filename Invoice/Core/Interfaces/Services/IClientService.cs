using invoice.Core.DTO.Client;
using invoice.Core.DTO;

namespace invoice.Core.Interfaces.Services
{
    public interface IClientService
    {
        Task<GeneralResponse<IEnumerable<GetAllClientsDTO>>> GetAllAsync(string userId);
        Task<GeneralResponse<ClientReadDTO>> GetByIdAsync(string id, string userId);
        Task<GeneralResponse<IEnumerable<ClientReadDTO>>> GetByNameAsync(string name, string userId);
        Task<GeneralResponse<IEnumerable<ClientReadDTO>>> SearchAsync(string keyword, string userId);

        Task<GeneralResponse<ClientReadDTO>> CreateAsync(ClientCreateDTO dto, string userId);
        Task<GeneralResponse<IEnumerable<ClientReadDTO>>> CreateRangeAsync(IEnumerable<ClientCreateDTO> dtos, string userId);

        Task<GeneralResponse<ClientReadDTO>> UpdateAsync(string id, ClientUpdateDTO dto, string userId);
        Task<GeneralResponse<bool>> DeleteAsync(string id, string userId);
        Task<GeneralResponse<bool>> DeleteRangeAsync(IEnumerable<string> ids, string userId);

        Task<bool> ExistsAsync(string id, string userId);
        Task<int> CountAsync(string userId);
    }
}
