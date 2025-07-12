using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using invoice.Data;
using invoice.Models;
using invoice.DTO;

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
            var dtoList = items.Select(c => new ContactInformationDetailsDTO
            {
                Id = c.Id,
                Location = c.location,
                Facebook = c.Facebook,
                WhatsApp = c.WhatsApp,
                Instagram = c.Instagram,
                StoreId = c.StoreId
            });

            return Ok(new GeneralResponse<IEnumerable<ContactInformationDetailsDTO>>
            {
                Success = true,
                Message = "Contact information list retrieved successfully.",
                Data = dtoList
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

            var dto = new ContactInformationDetailsDTO
            {
                Id = item.Id,
                Location = item.location,
                Facebook = item.Facebook,
                WhatsApp = item.WhatsApp,
                Instagram = item.Instagram,
                StoreId = item.StoreId
            };

            return Ok(new GeneralResponse<ContactInformationDetailsDTO>
            {
                Success = true,
                Message = "Contact information retrieved successfully.",
                Data = dto
            });
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateContactInformationDTO dto)
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

            var contact = new ContactInformation
            {
                location = dto.Location,
                Facebook = dto.Facebook,
                WhatsApp = dto.WhatsApp,
                Instagram = dto.Instagram,
                StoreId = dto.StoreId
            };

            await _repository.Add(contact);

            var result = new ContactInformationDetailsDTO
            {
                Id = contact.Id,
                Location = contact.location,
                Facebook = contact.Facebook,
                WhatsApp = contact.WhatsApp,
                Instagram = contact.Instagram,
                StoreId = contact.StoreId
            };

            return Ok(new GeneralResponse<ContactInformationDetailsDTO>
            {
                Success = true,
                Message = "Contact information created successfully.",
                Data = result
            });
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(string id, [FromBody] UpdateContactInformationDTO dto)
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
                    Message = $"No contact information found with ID {id}.",
                    Data = null
                });
            }

            existing.location = dto.Location;
            existing.Facebook = dto.Facebook;
            existing.WhatsApp = dto.WhatsApp;
            existing.Instagram = dto.Instagram;
            existing.StoreId = dto.StoreId;

            await _repository.Update(existing);

            var updated = new ContactInformationDetailsDTO
            {
                Id = existing.Id,
                Location = existing.location,
                Facebook = existing.Facebook,
                WhatsApp = existing.WhatsApp,
                Instagram = existing.Instagram,
                StoreId = existing.StoreId
            };

            return Ok(new GeneralResponse<ContactInformationDetailsDTO>
            {
                Success = true,
                Message = "Contact information updated successfully.",
                Data = updated
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