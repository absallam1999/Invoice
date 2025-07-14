using invoice.Data;
using invoice.DTO;
using invoice.DTO.PaymentMethod;
using invoice.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace invoice.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class PaymentMethodController : ControllerBase
    {
        private readonly IRepository<PaymentMethod> _repository;

        public PaymentMethodController(IRepository<PaymentMethod> repository)
        {
            _repository = repository;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var methods = await _repository.GetAll();

            var result = methods.Select(m => new PaymentMethodDTO
            {
                Id = m.Id,
                Name = m.Name
            });

            return Ok(new GeneralResponse<IEnumerable<PaymentMethodDTO>>
            {
                Success = true,
                Message = "Payment methods retrieved successfully.",
                Data = result
            });
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(string id)
        {
            var method = await _repository.GetById(id);
            if (method == null)
            {
                return NotFound(new GeneralResponse<object>
                {
                    Success = false,
                    Message = $"Payment method with ID {id} not found.",
                    Data = null
                });
            }

            var dto = new PaymentMethodDTO
            {
                Id = method.Id,
                Name = method.Name
            };

            return Ok(new GeneralResponse<PaymentMethodDTO>
            {
                Success = true,
                Message = "Payment method retrieved successfully.",
                Data = dto
            });
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreatePaymentMethodDTO dto)
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

            var method = new PaymentMethod { Name = dto.Name };
            await _repository.Add(method);

            return Ok(new GeneralResponse<PaymentMethod>
            {
                Success = true,
                Message = "Payment method created successfully.",
                Data = method
            });
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(string id, [FromBody] UpdatePaymentMethodDTO dto)
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
                    Message = $"Payment method with ID {id} not found.",
                    Data = null
                });
            }

            existing.Name = dto.Name;
            await _repository.Update(existing);

            return Ok(new GeneralResponse<PaymentMethod>
            {
                Success = true,
                Message = "Payment method updated successfully.",
                Data = existing
            });
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            var method = await _repository.GetById(id);
            if (method == null)
            {
                return NotFound(new GeneralResponse<object>
                {
                    Success = false,
                    Message = $"Payment method with ID {id} not found.",
                    Data = null
                });
            }

            await _repository.Delete(id);

            return Ok(new GeneralResponse<object>
            {
                Success = true,
                Message = "Payment method deleted successfully.",
                Data = null
            });
        }
    }
}
