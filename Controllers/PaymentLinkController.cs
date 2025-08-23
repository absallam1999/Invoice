using invoice.Core.DTO.PaymentLink;
using invoice.Core.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace invoice.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PaymentLinkController : ControllerBase
    {
        private readonly IPaymentLinkService _paymentLinkService;

        public PaymentLinkController(IPaymentLinkService paymentLinkService)
        {
            _paymentLinkService = paymentLinkService;
        }

        private string GetUserId() =>
        User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? string.Empty;

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(string id)
        {
            var result = await _paymentLinkService.GetByIdAsync(id, GetUserId());
            return result.Success ? Ok(result) : NotFound(result);
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var result = await _paymentLinkService.GetAllAsync(GetUserId());
            return Ok(result);
        }

        [HttpPost]
        public async Task<IActionResult> Create(PaymentLinkCreateDTO dto)
        {
            var result = await _paymentLinkService.CreateAsync(dto, GetUserId());
            return result.Success ? Ok(result) : BadRequest(result);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(string id, PaymentLinkUpdateDTO dto)
        {
            var result = await _paymentLinkService.UpdateAsync(id, dto, GetUserId());
            return result.Success ? Ok(result) : BadRequest(result);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            var result = await _paymentLinkService.DeleteAsync(id, GetUserId());
            return result.Success ? Ok(result) : BadRequest(result);
        }

        [HttpPost("{invoiceId}/generate")]
        public async Task<IActionResult> GenerateLink(string invoiceId, [FromQuery] decimal value)
        {
            var result = await _paymentLinkService.GenerateLinkAsync(invoiceId, value, GetUserId());
            return result.Success ? Ok(result) : BadRequest(result);
        }

        [HttpPost("{invoiceId}/recalculate")]
        public async Task<IActionResult> RecalculateInvoiceTotals(string invoiceId)
        {
            var result = await _paymentLinkService.RecalculateInvoiceTotalsAsync(invoiceId, GetUserId());
            return result.Success ? Ok(result) : BadRequest(result);
        }

        [HttpGet("count")]
        public async Task<IActionResult> Count()
        {
            var count = await _paymentLinkService.CountAsync(GetUserId());
            return Ok(new { Count = count });
        }
    }
}
