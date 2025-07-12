using invoice.Data;
using invoice.DTO;
using invoice.DTO.InvoiceItem;
using invoice.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace invoice.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class InvoiceItemController : ControllerBase
    {
        private readonly IRepository<InvoiceItem> _repository;

        public InvoiceItemController(IRepository<InvoiceItem> repository)
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
                Message = "Invoice items retrieved successfully.",
                Data = items
            });
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(string id)
        {
            var item = await _repository.GetById(id);
            if (item == null)
                return NotFound(new GeneralResponse<object>
                {
                    Success = false,
                    Message = $"Invoice item with ID {id} not found.",
                    Data = null
                });

            return Ok(new GeneralResponse<object>
            {
                Success = true,
                Message = "Invoice item retrieved successfully.",
                Data = item
            });
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateInvoiceItemDTO dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(new GeneralResponse<object>
                {
                    Success = false,
                    Message = "Validation failed.",
                    Data = ModelState
                });

            var item = new InvoiceItem
            {
                InvoiceId = dto.InvoiceId,
                ProductId = dto.ProductId,
                Quantity = dto.Quantity,
                UnitPrice = dto.UnitPrice,
                Subtotal = dto.Quantity * dto.UnitPrice
            };

            await _repository.Add(item);

            return Ok(new GeneralResponse<object>
            {
                Success = true,
                Message = "Invoice item created successfully.",
                Data = item
            });
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(string id, [FromBody] UpdateInvoiceItemDTO dto)
        {
            if (id != dto.Id)
                return BadRequest(new GeneralResponse<object>
                {
                    Success = false,
                    Message = "ID mismatch.",
                    Data = null
                });

            if (!ModelState.IsValid)
                return BadRequest(new GeneralResponse<object>
                {
                    Success = false,
                    Message = "Validation failed.",
                    Data = ModelState
                });

            var existing = await _repository.GetById(id);
            if (existing == null)
                return NotFound(new GeneralResponse<object>
                {
                    Success = false,
                    Message = $"Invoice item with ID {id} not found.",
                    Data = null
                });

            existing.InvoiceId = dto.InvoiceId;
            existing.ProductId = dto.ProductId;
            existing.Quantity = dto.Quantity;
            existing.UnitPrice = dto.UnitPrice;
            existing.Subtotal = dto.Quantity * dto.UnitPrice;

            await _repository.Update(existing);

            return Ok(new GeneralResponse<object>
            {
                Success = true,
                Message = "Invoice item updated successfully.",
                Data = existing
            });
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            var item = await _repository.GetById(id);
            if (item == null)
                return NotFound(new GeneralResponse<object>
                {
                    Success = false,
                    Message = $"Invoice item with ID {id} not found.",
                    Data = null
                });

            await _repository.Delete(id);

            return Ok(new GeneralResponse<object>
            {
                Success = true,
                Message = "Invoice item deleted successfully.",
                Data = null
            });
        }
    }
}