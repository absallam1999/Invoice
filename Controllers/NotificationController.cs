using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;
using invoice.Models;
using invoice.Data;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using invoice.DTO;
using System.Linq;

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
            var notifications = await _repository.GetAll();
            return Ok(new GeneralResponse<IEnumerable<Notification>>
            {
                Success = true,
                Message = "All notifications retrieved.",
                Data = notifications
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

            return Ok(new GeneralResponse<Notification>
            {
                Success = true,
                Message = "Notification found.",
                Data = notification
            });
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] Notification notification)
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

            await _repository.Add(notification);
            return Ok(new GeneralResponse<Notification>
            {
                Success = true,
                Message = "Notification created successfully.",
                Data = notification
            });
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(string id, [FromBody] Notification updated)
        {
            if (id != updated.Id)
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

            existing.Title = updated.Title;
            existing.Message = updated.Message;
            existing.UserId = updated.UserId;

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

        [HttpGet("my")]
        public async Task<IActionResult> GetUserNotifications()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var all = await _repository.GetAll();
            var userNotifications = all.Where(n => n.UserId == userId);

            return Ok(new GeneralResponse<IEnumerable<Notification>>
            {
                Success = true,
                Message = "User notifications retrieved.",
                Data = userNotifications
            });
        }
    }
}