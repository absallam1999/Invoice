using invoice.Core.DTO;
using invoice.Core.DTO.Order;

namespace invoice.Core.Interfaces.Services
{
    public interface IOrderService
    {
        Task<GeneralResponse<IEnumerable<OrderReadDTO>>> GetAllAsync(string userId);
        Task<GeneralResponse<OrderReadDTO>> GetByIdAsync(string id, string userId);

        Task<GeneralResponse<OrderReadDTO>> AddAsync(OrderCreateDTO dto, string userId);
        Task<GeneralResponse<OrderReadDTO>> UpdateAsync(OrderUpdateDTO dto, string userId);
        Task<GeneralResponse<OrderReadDTO>> DeleteAsync(string id, string userId);

        Task<GeneralResponse<IEnumerable<OrderReadDTO>>> AddRangeAsync(IEnumerable<OrderCreateDTO> dtos, string userId);
        Task<GeneralResponse<IEnumerable<OrderReadDTO>>> UpdateRangeAsync(IEnumerable<OrderUpdateDTO> dtos, string userId);
        Task<GeneralResponse<IEnumerable<OrderReadDTO>>> DeleteRangeAsync(IEnumerable<string> ids, string userId);
    }
}
