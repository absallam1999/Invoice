using invoice.Core.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using invoice.Core.Entities;
using invoice.Core.DTO;

namespace invoice.Controllers
{
    //[Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class InvoiceItemController : ControllerBase
    {
        private readonly IInvoiceItemsService _invoiceItemService;

        public InvoiceItemController(IInvoiceItemsService invoiceItemService)
        {
            _invoiceItemService = invoiceItemService;
        }

        [HttpGet("invoice/{invoiceId}")]
        public async Task<IActionResult> GetByInvoiceId(string invoiceId)
        {
            var response = await _invoiceItemService.GetByInvoiceIdAsync(invoiceId);
            return StatusCode(response.Success ? 200 : 404, response);
        }

        [HttpGet("product/{productId}")]
        public async Task<IActionResult> GetByProductId(string productId)
        {
            var response = await _invoiceItemService.GetByProductIdAsync(productId);
            return StatusCode(response.Success ? 200 : 404, response);
        }

        [HttpGet("invoice/{invoiceId}/total")]
        public async Task<IActionResult> GetInvoiceTotal(string invoiceId)
        {
            var response = await _invoiceItemService.GetInvoiceTotalAsync(invoiceId);
            return StatusCode(response.Success ? 200 : 404, response);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] InvoiceItem item)
        {
            var response = await _invoiceItemService.CreateAsync(item);
            return StatusCode(response.Success ? 201 : 400, response);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(string id, [FromBody] InvoiceItem item)
        {
            if (id != item.Id)
                return BadRequest(new GeneralResponse<bool> { Success = false, Message = "ID mismatch" });

            var response = await _invoiceItemService.UpdateAsync(item);
            return StatusCode(response.Success ? 200 : 400, response);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            var response = await _invoiceItemService.DeleteAsync(id);
            return StatusCode(response.Success ? 200 : 404, response);
        }

        [HttpDelete("invoice/{invoiceId}")]
        public async Task<IActionResult> DeleteByInvoiceId(string invoiceId)
        {
            var response = await _invoiceItemService.DeleteByInvoiceIdAsync(invoiceId);
            return StatusCode(response.Success ? 200 : 404, response);
        }
    }
}