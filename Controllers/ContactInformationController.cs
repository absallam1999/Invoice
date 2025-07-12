using invoice.Data;
using invoice.DTO;
using invoice.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace invoice.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class ContactInformationController : ControllerBase
    {
        private readonly IRepository<ContactInformation> _repository;

        public ContactInformationController(IRepository<ContactInformation> repository)
        {
            _repository = repository;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var items = await _repository.GetAll();
            return Ok(new GeneralResponse<object>
            {
                Success = true,
                Message = "Contact information list retrieved successfully.",
                Data = items
            });
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(string id)
        {
            var item = await _repository.GetById(id);
            if (item == null)
            {
                return NotFound(new GeneralResponse<object>
                {
                    Success = false,
                    Message = $"No contact information found with ID {id}.",
                    Data = null
                });
            }

            return Ok(new GeneralResponse<object>
            {
                Success = true,
                Message = "Contact information retrieved successfully.",
                Data = item
            });
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] ContactInformation contact)
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

            await _repository.Add(contact);
            return Ok(new GeneralResponse<object>
            {
                Success = true,
                Message = "Contact information created successfully.",
                Data = contact
            });
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(string id, [FromBody] ContactInformation updated)
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
                    Message = $"No contact information found with ID {id}.",
                    Data = null
                });
            }

            existing.location = updated.location;
            existing.Facebook = updated.Facebook;
            existing.WhatsApp = updated.WhatsApp;
            existing.Instagram = updated.Instagram;
            existing.StoreId = updated.StoreId;

            await _repository.Update(existing);

            return Ok(new GeneralResponse<object>
            {
                Success = true,
                Message = "Contact information updated successfully.",
                Data = existing
            });
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            var item = await _repository.GetById(id);
            if (item == null)
            {
                return NotFound(new GeneralResponse<object>
                {
                    Success = false,
                    Message = $"No contact information found with ID {id}.",
                    Data = null
                });
            }

            await _repository.Delete(id);
            return Ok(new GeneralResponse<object>
            {
                Success = true,
                Message = "Contact information deleted successfully.",
                Data = null
            });
        }
    }
}