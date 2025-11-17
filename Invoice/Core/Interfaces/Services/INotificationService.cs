using invoice.Core.DTO;
using invoice.Core.Entities;

namespace invoice.Core.Interfaces.Services
{
    public interface INotificationService
    {
        Task<GeneralResponse<IEnumerable<Notification>>> GetAllAsync(string userId = null);
        Task<GeneralResponse<Notification>> GetByIdAsync(string id, string userId = null);
        Task<GeneralResponse<IEnumerable<Notification>>> GetByUserAsync(string userId);

        Task<GeneralResponse<IEnumerable<Notification>>> SearchAsync(string keyword, string userId = null);

        Task<GeneralResponse<Notification>> CreateAsync(Notification notification);
        Task<GeneralResponse<IEnumerable<Notification>>> CreateRangeAsync(IEnumerable<Notification> notifications);

        Task<GeneralResponse<Notification>> UpdateAsync(string id, Notification notification);
        Task<GeneralResponse<IEnumerable<Notification>>> UpdateRangeAsync(IEnumerable<Notification> notifications);

        Task<GeneralResponse<bool>> DeleteAsync(string id);
        Task<GeneralResponse<bool>> DeleteRangeAsync(IEnumerable<string> ids);

        Task<bool> ExistsAsync(string id);
        Task<int> CountAsync(string userId = null);
    }
}
