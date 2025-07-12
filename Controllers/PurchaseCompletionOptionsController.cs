using invoice.Data;
using invoice.DTO;
using invoice.DTO.PurchaseCompletionOptions;
using invoice.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace invoice.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class PurchaseCompletionOptionsController : ControllerBase
    {
        private readonly IRepository<PurchaseCompletionOptions> _repository;

        public PurchaseCompletionOptionsController(IRepository<PurchaseCompletionOptions> repository)
        {
            _repository = repository;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var options = await _repository.GetAll();
            return Ok(new GeneralResponse<object>
            {
                Success = true,
                Message = "Options retrieved successfully.",
                Data = options
            });
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(string id)
        {
            var option = await _repository.GetById(id);
            if (option == null)
            {
                return NotFound(new GeneralResponse<object>
                {
                    Success = false,
                    Message = $"Option with ID {id} not found.",
                    Data = null
                });
            }

            return Ok(new GeneralResponse<object>
            {
                Success = true,
                Message = "Option retrieved successfully.",
                Data = option
            });
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreatePurchaseCompletionOptionsDTO dto)
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

            var option = new PurchaseCompletionOptions
            {
                SendEmail = dto.SendEmail,
                StoreId = dto.StoreId
            };

            await _repository.Add(option);
            return Ok(new GeneralResponse<object>
            {
                Success = true,
                Message = "Option created successfully.",
                Data = option
            });
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(string id, [FromBody] UpdatePurchaseCompletionOptionsDTO dto)
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
                    Message = $"Option with ID {id} not found.",
                    Data = null
                });
            }

            existing.SendEmail = dto.SendEmail;
            existing.StoreId = dto.StoreId;

            await _repository.Update(existing);
            return Ok(new GeneralResponse<object>
            {
                Success = true,
                Message = "Option updated successfully.",
                Data = existing
            });
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            var option = await _repository.GetById(id);
            if (option == null)
            {
                return NotFound(new GeneralResponse<object>
                {
                    Success = false,
                    Message = $"Option with ID {id} not found.",
                    Data = null
                });
            }

            await _repository.Delete(id);
            return Ok(new GeneralResponse<object>
            {
                Success = true,
                Message = "Option deleted successfully.",
                Data = null
            });
        }
    }
}
