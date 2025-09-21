using invoice.Core.DTO.Notification;
using invoice.Core.Interfaces.Services;
using invoice.Core.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace invoice.Controllers
{
    //[Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class NotificationController : ControllerBase
    {
        private readonly INotificationService _notificationService;

        public NotificationController(INotificationService notificationService)
        {
            _notificationService = notificationService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var result = await _notificationService.GetAllAsync();
            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(string id)
        {
            var result = await _notificationService.GetByIdAsync(id);
            if (!result.Success) return NotFound(result);
            return Ok(result);
        }

        [HttpGet("user/{userId}")]
        public async Task<IActionResult> GetByUser(string userId)
        {
            var result = await _notificationService.GetByUserAsync(userId);
            return Ok(result);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] NotificationCreateDTO dto)
        {
            var notification = await _notificationService.CreateAsync(
                new Notification
                {
                    Title = dto.Title,
                    Message = dto.Message,
                    Type = dto.Type,
                    UserId = dto.UserId
                });
            return Ok(notification);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(string id, [FromBody] NotificationUpdateDTO dto)
        {
            var notification = await _notificationService.UpdateAsync(id,
                new Notification
                {
                    Id = dto.Id,
                    Title = dto.Title,
                    Type = dto.Type,
                    Message = dto.Message
                });

            if (!notification.Success) return NotFound(notification);
            return Ok(notification);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            var result = await _notificationService.DeleteAsync(id);
            if (!result.Success) return NotFound(result);
            return Ok(result);
        }

        [HttpDelete]
        public async Task<IActionResult> DeleteRange([FromBody] IEnumerable<string> ids)
        {
            var result = await _notificationService.DeleteRangeAsync(ids);
            return Ok(result);
        }
    }
}
