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
            var notifications = await _repository.GetAll(n => n.User);
            var dtoList = notifications.Select(n => new NotificationDetailsDTO
            {
                Id = n.Id,
                Title = n.Title,
                Message = n.Message,
                UserId = n.UserId,
                UserName = n.User?.UserName
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
            var notification = await _repository.GetById(id, n => n.User);
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
                UserId = notification.UserId,
                UserName = notification.User?.UserName
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

            return Ok(new GeneralResponse<Notification>
            {
                Success = true,
                Message = "Notification created successfully.",
                Data = notification
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

            return Ok(new GeneralResponse<Notification>
            {
                Success = true,
                Message = "Notification updated successfully.",
                Data = existing
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

        [HttpGet("user/{userId}")]
        public async Task<IActionResult> GetUserNotifications(string userId)
        {
            if (string.IsNullOrEmpty(userId))
            {
                return BadRequest(new GeneralResponse<object>
                {
                    Success = false,
                    Message = "User ID is required.",
                    Data = null
                });
            }

            var notifications = await _repository.Query(n => n.UserId == userId, n => n.User);

            var result = notifications.Select(n => new NotificationDetailsDTO
            {
                Id = n.Id,
                Title = n.Title,
                Message = n.Message,
                UserId = n.UserId,
                UserName = n.User?.UserName
            });

            return Ok(new GeneralResponse<IEnumerable<NotificationDetailsDTO>>
            {
                Success = true,
                Message = "User notifications retrieved successfully.",
                Data = result
            });
        }
    }
}