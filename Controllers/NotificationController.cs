using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using invoice.Data;
using invoice.Models;
using invoice.DTO;

namespace invoice.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class NotificationController : ControllerBase
    {
        private readonly IRepository<Notification> _repository;

        public NotificationController(IRepository<Notification> repository)
        {
            _repository = repository;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var items = await _repository.GetAll();
            var dtoList = items.Select(n => new NotificationDetailsDTO
            {
                Id = n.Id,
                Title = n.Title,
                Message = n.Message,
                UserId = n.UserId
            });

            return Ok(new GeneralResponse<IEnumerable<NotificationDetailsDTO>>
            {
                Success = true,
                Message = "All notifications retrieved.",
                Data = dtoList
            });
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(string id)
        {
            var notification = await _repository.GetById(id);
            if (notification == null)
            {
                return NotFound(new GeneralResponse<object>
                {
                    Success = false,
                    Message = $"Notification with ID {id} not found.",
                    Data = null
                });
            }

            var dto = new NotificationDetailsDTO
            {
                Id = notification.Id,
                Title = notification.Title,
                Message = notification.Message,
                UserId = notification.UserId
            };

            return Ok(new GeneralResponse<NotificationDetailsDTO>
            {
                Success = true,
                Message = "Notification found.",
                Data = dto
            });
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateNotificationDTO dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new GeneralResponse<object>
                {
                    Success = false,
                    Message = "Validation failed.",
                    Data = ModelState
                });
            }

            var notification = new Notification
            {
                Title = dto.Title,
                Message = dto.Message,
                UserId = dto.UserId
            };

            await _repository.Add(notification);

            var result = new NotificationDetailsDTO
            {
                Id = notification.Id,
                Title = notification.Title,
                Message = notification.Message,
                UserId = notification.UserId
            };

            return Ok(new GeneralResponse<NotificationDetailsDTO>
            {
                Success = true,
                Message = "Notification created successfully.",
                Data = result
            });
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(string id, [FromBody] UpdateNotificationDTO dto)
        {
            if (id != dto.Id)
            {
                return BadRequest(new GeneralResponse<object>
                {
                    Success = false,
                    Message = "ID mismatch.",
                    Data = null
                });
            }

            var existing = await _repository.GetById(id);
            if (existing == null)
            {
                return NotFound(new GeneralResponse<object>
                {
                    Success = false,
                    Message = $"Notification with ID {id} not found.",
                    Data = null
                });
            }

            existing.Title = dto.Title;
            existing.Message = dto.Message;
            existing.UserId = dto.UserId;

            await _repository.Update(existing);

            var updated = new NotificationDetailsDTO
            {
                Id = existing.Id,
                Title = existing.Title,
                Message = existing.Message,
                UserId = existing.UserId
            };

            return Ok(new GeneralResponse<NotificationDetailsDTO>
            {
                Success = true,
                Message = "Notification updated successfully.",
                Data = updated
            });
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            var notification = await _repository.GetById(id);
            if (notification == null)
            {
                return NotFound(new GeneralResponse<object>
                {
                    Success = false,
                    Message = $"Notification with ID {id} not found.",
                    Data = null
                });
            }

            await _repository.Delete(id);
            return Ok(new GeneralResponse<object>
            {
                Success = true,
                Message = "Notification deleted successfully.",
                Data = null
            });
        }

        [HttpGet("my")]
        public async Task<IActionResult> GetUserNotifications()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var all = await _repository.GetAll();
            var userNotifications = all
                .Where(n => n.UserId == userId)
                .Select(n => new NotificationDetailsDTO
                {
                    Id = n.Id,
                    Title = n.Title,
                    Message = n.Message,
                    UserId = n.UserId
                });

            return Ok(new GeneralResponse<IEnumerable<NotificationDetailsDTO>>
            {
                Success = true,
                Message = "User notifications retrieved.",
                Data = userNotifications
            });
        }
    }
}
