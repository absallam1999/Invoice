using invoice.Core.DTO.Notification;
using invoice.Core.Interfaces.Services;
using invoice.Core.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace invoice.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class NotificationController : ControllerBase
    {
        private readonly INotificationService _notificationService;

        public NotificationController(INotificationService notificationService)
        {
            _notificationService = notificationService;
        }

        [HttpGet("all")]
        public async Task<IActionResult> GetAll()
        {
            var result = await _notificationService.GetAllAsync();
            return result.Success ? Ok(result) : BadRequest(result);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(string id)
        {
            var result = await _notificationService.GetByIdAsync(id);
            return result.Success ? Ok(result) : NotFound(result);
        }

        [HttpGet("user/{userId}")]
        public async Task<IActionResult> GetByUser(string userId)
        {
            var result = await _notificationService.GetByUserAsync(userId);
            return result.Success ? Ok(result) : BadRequest(result);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] NotificationCreateDTO dto)
        {
            var result = await _notificationService.CreateAsync(new Notification
            {
                Title = dto.Title,
                Message = dto.Message,
                Type = dto.Type,
                UserId = dto.UserId
            });

            return result.Success ? Ok(result) : BadRequest(result);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(string id, [FromBody] NotificationUpdateDTO dto)
        {
            var result = await _notificationService.UpdateAsync(id, new Notification
            {
                Id = dto.Id,
                Title = dto.Title,
                Type = dto.Type,
                Message = dto.Message
            });

            return result.Success ? Ok(result) : NotFound(result);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            var result = await _notificationService.DeleteAsync(id);
            return result.Success ? Ok(result) : NotFound(result);
        }

        [HttpDelete]
        public async Task<IActionResult> DeleteRange([FromBody] IEnumerable<string> ids)
        {
            var result = await _notificationService.DeleteRangeAsync(ids);
            return result.Success ? Ok(result) : BadRequest(result);
        }
    }
}
