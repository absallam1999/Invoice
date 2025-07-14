using invoice.Data;
using invoice.DTO.PaymentLink;
using invoice.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using invoice.DTO;

namespace invoice.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class PaymentLinkController : ControllerBase
    {
        private readonly IRepository<PaymentLink> _repository;

        public PaymentLinkController(IRepository<PaymentLink> repository)
        {
            _repository = repository;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var links = await _repository.GetAll(l => l.Payment);
            var result = links.Select(l => new PaymentLinkDetailsDTO
            {
                Id = l.Id,
                Link = l.Link,
                Value = l.Value,
                PaymentsNumber = l.PaymentsNumber,
                Description = l.Description,
                Message = l.Message,
                Image = l.Image,
                Terms = l.Terms,
                IsDeleted = l.IsDeleted,
                PaymentId = l.PaymentId,
                PaymentName = l.Payment?.Name
            });

            return Ok(new GeneralResponse<IEnumerable<PaymentLinkDetailsDTO>>
            {
                Success = true,
                Message = "Payment links retrieved successfully.",
                Data = result
            });
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(string id)
        {
            var link = await _repository.GetById(id, l => l.Payment);
            if (link == null)
            {
                return NotFound(new GeneralResponse<object>
                {
                    Success = false,
                    Message = $"No payment link found with ID {id}.",
                    Data = null
                });
            }

            var dto = new PaymentLinkDetailsDTO
            {
                Id = link.Id,
                Link = link.Link,
                Value = link.Value,
                PaymentsNumber = link.PaymentsNumber,
                Description = link.Description,
                Message = link.Message,
                Image = link.Image,
                Terms = link.Terms,
                IsDeleted = link.IsDeleted,
                PaymentId = link.PaymentId,
                PaymentName = link.Payment?.Name
            };

            return Ok(new GeneralResponse<PaymentLinkDetailsDTO>
            {
                Success = true,
                Message = "Payment link retrieved successfully.",
                Data = dto
            });
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreatePaymentLinkDTO dto)
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

            var link = new PaymentLink
            {
                Link = dto.Link,
                Value = dto.Value,
                PaymentsNumber = dto.PaymentsNumber,
                Description = dto.Description,
                Message = dto.Message,
                Image = dto.Image,
                Terms = dto.Terms,
                IsDeleted = dto.IsDeleted,
                PaymentId = dto.PaymentId
            };

            await _repository.Add(link);

            return Ok(new GeneralResponse<PaymentLink>
            {
                Success = true,
                Message = "Payment link created successfully.",
                Data = link
            });
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(string id, [FromBody] UpdatePaymentLinkDTO dto)
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
                    Message = $"No payment link found with ID {id}.",
                    Data = null
                });
            }

            existing.Link = dto.Link;
            existing.Value = dto.Value;
            existing.PaymentsNumber = dto.PaymentsNumber;
            existing.Description = dto.Description;
            existing.Message = dto.Message;
            existing.Image = dto.Image;
            existing.Terms = dto.Terms;
            existing.IsDeleted = dto.IsDeleted;
            existing.PaymentId = dto.PaymentId;

            await _repository.Update(existing);

            return Ok(new GeneralResponse<PaymentLink>
            {
                Success = true,
                Message = "Payment link updated successfully.",
                Data = existing
            });
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            var link = await _repository.GetById(id);
            if (link == null)
            {
                return NotFound(new GeneralResponse<object>
                {
                    Success = false,
                    Message = $"No payment link found with ID {id}.",
                    Data = null
                });
            }

            await _repository.Delete(id);

            return Ok(new GeneralResponse<object>
            {
                Success = true,
                Message = "Payment link deleted successfully.",
                Data = null
            });
        }
    }
}