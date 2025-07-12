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
            var links = await _repository.GetAll();
            return Ok(new GeneralResponse<object>
            {
                Success = true,
                Message = "Payment links retrieved successfully.",
                Data = links
            });
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(string id)
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

            return Ok(new GeneralResponse<object>
            {
                Success = true,
                Message = "Payment link retrieved successfully.",
                Data = link
            });
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] PaymentLink link)
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

            await _repository.Add(link);
            return Ok(new GeneralResponse<object>
            {
                Success = true,
                Message = "Payment link created successfully.",
                Data = link
            });
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(string id, [FromBody] PaymentLink updated)
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
                    Message = $"No payment link found with ID {id}.",
                    Data = null
                });
            }

            existing.Link = updated.Link;
            existing.Value = updated.Value;
            existing.PaymentsNumber = updated.PaymentsNumber;
            existing.Description = updated.Description;
            existing.Message = updated.Message;
            existing.Image = updated.Image;
            existing.Terms = updated.Terms;
            existing.IsDeleted = updated.IsDeleted;
            existing.PaymentId = updated.PaymentId;

            await _repository.Update(existing);

            return Ok(new GeneralResponse<object>
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