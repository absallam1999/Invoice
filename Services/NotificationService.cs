using AutoMapper;
using invoice.Core.Interfaces.Services;
using invoice.Core.DTO;
using invoice.Core.Entities;
using invoice.Repo;
using Microsoft.EntityFrameworkCore;
namespace invoice.Services
{
    public class NotificationService : INotificationService
    {
        private readonly IRepository<Notification> _notificationRepo;
        private readonly IMapper _mapper;

        public NotificationService(IRepository<Notification> notificationRepo, IMapper mapper)
        {
            _notificationRepo = notificationRepo;
            _mapper = mapper;
        }

        public async Task<GeneralResponse<IEnumerable<Notification>>> GetAllAsync(string userId = null)
        {
            var notifications = await _notificationRepo.GetAllAsync(userId, n => n.User);

            return new GeneralResponse<IEnumerable<Notification>>
            {
                Success = true,
                Message = notifications.Any() ? "Notifications retrieved successfully" : "No notifications found",
                Data = notifications
            };
        }

        public async Task<GeneralResponse<Notification>> GetByIdAsync(string id, string userId = null)
        {
            var notification = await _notificationRepo.GetByIdAsync(id, userId, q => q.Include(n => n.User));

            if (notification == null)
                return new GeneralResponse<Notification>
                {
                    Success = false,
                    Message = "Notification not found"
                };

            return new GeneralResponse<Notification>
            {
                Success = true,
                Data = notification
            };
        }

        public async Task<GeneralResponse<IEnumerable<Notification>>> GetByUserAsync(string userId)
        {
            var notifications = await _notificationRepo.QueryAsync(n => n.UserId == userId);
            return new GeneralResponse<IEnumerable<Notification>>
            {
                Success = true,
                Message = notifications.Any() ? "Notifications retrieved successfully" : "No notifications found for this user",
                Data = notifications
            };
        }

        public async Task<GeneralResponse<IEnumerable<Notification>>> SearchAsync(string keyword, string userId = null)
        {
            var notifications = await _notificationRepo.QueryAsync(
                n => (string.IsNullOrEmpty(userId) || n.UserId == userId) &&
                     (n.Title.Contains(keyword) || n.Message.Contains(keyword))
            );

            return new GeneralResponse<IEnumerable<Notification>>
            {
                Success = true,
                Message = notifications.Any() ? "Search results" : "No matching notifications found",
                Data = notifications
            };
        }

        public async Task<GeneralResponse<Notification>> CreateAsync(Notification notification)
        {
            var response = await _notificationRepo.AddAsync(notification);
            return new GeneralResponse<Notification>
            {
                Success = response.Success,
                Message = response.Success ? "Notification created successfully" : response.Message,
                Data = response.Data
            };
        }

        public async Task<GeneralResponse<IEnumerable<Notification>>> CreateRangeAsync(IEnumerable<Notification> notifications)
        {
            var response = await _notificationRepo.AddRangeAsync(notifications);
            return new GeneralResponse<IEnumerable<Notification>>
            {
                Success = response.Success,
                Message = response.Success ? "Notifications created successfully" : response.Message,
                Data = response.Data
            };
        }

        public async Task<GeneralResponse<Notification>> UpdateAsync(string id, Notification notification)
        {
            var existing = await _notificationRepo.GetByIdAsync(id);
            if (existing == null)
                return new GeneralResponse<Notification> { Success = false, Message = "Notification not found" };

            _mapper.Map(notification, existing);

            var response = await _notificationRepo.UpdateAsync(existing);
            return new GeneralResponse<Notification>
            {
                Success = response.Success,
                Message = response.Success ? "Notification updated successfully" : response.Message,
                Data = response.Data
            };
        }

        public async Task<GeneralResponse<IEnumerable<Notification>>> UpdateRangeAsync(IEnumerable<Notification> notifications)
        {
            var entities = new List<Notification>();
            foreach (var notification in notifications)
            {
                var existing = await _notificationRepo.GetByIdAsync(notification.Id);
                if (existing != null)
                {
                    _mapper.Map(notification, existing);
                    entities.Add(existing);
                }
            }

            var response = await _notificationRepo.UpdateRangeAsync(entities);
            return new GeneralResponse<IEnumerable<Notification>>
            {
                Success = response.Success,
                Message = response.Success ? "Notifications updated successfully" : response.Message,
                Data = response.Data
            };
        }

        public async Task<GeneralResponse<bool>> DeleteAsync(string id)
        {
            var existing = await _notificationRepo.GetByIdAsync(id);
            if (existing == null)
                return new GeneralResponse<bool> { Success = false, Message = "Notification not found", Data = false };

            var response = await _notificationRepo.DeleteAsync(id);
            return new GeneralResponse<bool>
            {
                Success = response.Success,
                Message = response.Message,
                Data = response.Success
            };
        }

        public async Task<GeneralResponse<bool>> DeleteRangeAsync(IEnumerable<string> ids)
        {
            var response = await _notificationRepo.DeleteRangeAsync(ids);
            return new GeneralResponse<bool>
            {
                Success = response.Success,
                Message = response.Message,
                Data = response.Success
            };
        }

        public async Task<bool> ExistsAsync(string id)
        {
            return await _notificationRepo.ExistsAsync(n => n.Id == id);
        }

        public async Task<int> CountAsync(string userId)
        {
            return await _notificationRepo.CountAsync(userId);

        }
    }
}

