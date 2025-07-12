using invoice.Data;
using invoice.DTO;
using invoice.DTO.Invoice;
using invoice.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace invoice.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class InvoiceController : ControllerBase
    {
        private readonly IRepository<Invoice> _repository;

        public InvoiceController(IRepository<Invoice> repository)
        {
            _repository = repository;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var invoices = await _repository.GetAll();
            return Ok(new GeneralResponse<object>
            {
                Success = true,
                Message = "Invoices retrieved successfully.",
                Data = invoices
            });
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(string id)
        {
            var invoice = await _repository.GetById(id);
            if (invoice == null)
            {
                return NotFound(new GeneralResponse<object>
                {
                    Success = false,
                    Message = $"Invoice with ID {id} not found.",
                    Data = null
                });
            }

            return Ok(new GeneralResponse<object>
            {
                Success = true,
                Message = "Invoice retrieved successfully.",
                Data = invoice
            });
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateInvoiceDTO dto)
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

            var invoice = new Invoice
            {
                Number = dto.Number,
                CreateAt = dto.CreateAt,
                TaxNumber = dto.TaxNumber,
                Value = dto.Value,
                Description = dto.Description,
                InvoiceStatus = dto.InvoiceStatus,
                InvoiceType = dto.InvoiceType,
                UserId = dto.UserId,
                StoreId = dto.StoreId,
                ClientId = dto.ClientId,
                LanguageId = dto.LanguageId
            };

            await _repository.Add(invoice);

            return Ok(new GeneralResponse<object>
            {
                Success = true,
                Message = "Invoice created successfully.",
                Data = invoice
            });
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(string id, [FromBody] UpdateInvoiceDTO dto)
        {
            if (id != dto.Id)
            {
                return BadRequest(new GeneralResponse<object>
                {
                    Success = false,
                    Message = "Mismatched ID.",
                    Data = null
                });
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(new GeneralResponse<object>
                {
                    Success = false,
                    Message = "Validation failed.",
                    Data = ModelState
                });
            }

            var invoice = await _repository.GetById(id);
            if (invoice == null)
            {
                return NotFound(new GeneralResponse<object>
                {
                    Success = false,
                    Message = $"Invoice with ID {id} not found.",
                    Data = null
                });
            }

            invoice.Number = dto.Number;
            invoice.CreateAt = dto.CreateAt;
            invoice.TaxNumber = dto.TaxNumber;
            invoice.Value = dto.Value;
            invoice.Description = dto.Description;
            invoice.InvoiceStatus = dto.InvoiceStatus;
            invoice.InvoiceType = dto.InvoiceType;
            invoice.UserId = dto.UserId;
            invoice.StoreId = dto.StoreId;
            invoice.ClientId = dto.ClientId;
            invoice.LanguageId = dto.LanguageId;

            await _repository.Update(invoice);

            return Ok(new GeneralResponse<object>
            {
                Success = true,
                Message = "Invoice updated successfully.",
                Data = invoice
            });
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            var invoice = await _repository.GetById(id);
            if (invoice == null)
            {
                return NotFound(new GeneralResponse<object>
                {
                    Success = false,
                    Message = $"Invoice with ID {id} not found.",
                    Data = null
                });
            }

            await _repository.Delete(id);

            return Ok(new GeneralResponse<object>
            {
                Success = true,
                Message = "Invoice deleted successfully.",
                Data = null
            });
        }
    }
}
