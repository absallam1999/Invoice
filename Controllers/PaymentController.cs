using invoice.Core.DTO;
using invoice.Core.DTO.Payment;
using invoice.Core.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace invoice.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class PaymentController : ControllerBase
    {
        private readonly IPaymentService _service;

        public PaymentController(IPaymentService service)
        {
            _service = service;
        }

        private string GetUserId() => User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? string.Empty;

        [HttpGet("all")]
        public async Task<IActionResult> GetAll()
        {
            var response = await _service.GetAllAsync(GetUserId());
            if (!response.Success) return BadRequest(response);
            return Ok(response);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(string id)
        {
            var response = await _service.GetByIdAsync(id, GetUserId());
            if (!response.Success) return NotFound(response);
            return Ok(response);
        }

        [HttpGet("invoice/{invoiceId}")]
        public async Task<IActionResult> GetByInvoice(string invoiceId)
        {
            var response = await _service.GetByInvoiceIdAsync(invoiceId, GetUserId());
            if (!response.Success) return NotFound(response);
            return Ok(response);
        }

        [HttpGet("method/{paymentMethodId}")]
        public async Task<IActionResult> GetByPaymentMethod(string paymentMethodId)
        {
            var response = await _service.GetByPaymentMethodIdAsync(paymentMethodId, GetUserId());
            if (!response.Success) return NotFound(response);
            return Ok(response);
        }

        [HttpGet("count/invoice/{invoiceId}")]
        public async Task<IActionResult> CountByInvoice(string invoiceId)
        {
            var count = await _service.CountByInvoiceAsync(invoiceId, GetUserId());
            return Ok(new GeneralResponse<int>(true, "Payments count by invoice retrieved successfully", count));
        }

        [HttpGet("count/method/{paymentMethodId}")]
        public async Task<IActionResult> CountByPaymentMethod(string paymentMethodId)
        {
            var count = await _service.CountByPaymentMethodAsync(paymentMethodId, GetUserId());
            return Ok(new GeneralResponse<int>(true, "Payments count by payment method retrieved successfully", count));
        }

        [HttpGet("total-paid/invoice/{invoiceId}")]
        public async Task<IActionResult> GetTotalPaidByInvoice(string invoiceId)
        {
            var response = await _service.GetTotalPaidByInvoiceAsync(invoiceId, GetUserId());
            if (!response.Success) return BadRequest(response);
            return Ok(response);
        }

        [HttpGet("total-paid/user")]
        public async Task<IActionResult> GetTotalPaidByUser()
        {
            var response = await _service.GetTotalPaidByUserAsync(GetUserId());
            if (!response.Success) return BadRequest(response);
            return Ok(response);
        }

        [HttpGet("total-paid/method/{paymentMethodId}")]
        public async Task<IActionResult> GetTotalPaidByPaymentMethod(string paymentMethodId)
        {
            var response = await _service.GetTotalPaidByPaymentMethodAsync(paymentMethodId, GetUserId());
            if (!response.Success) return BadRequest(response);
            return Ok(response);
        }

        [HttpGet("revenue/monthly/{year}")]
        public async Task<IActionResult> GetMonthlyRevenue(int year)
        {
            var response = await _service.GetMonthlyRevenueAsync(year, GetUserId());
            if (!response.Success) return BadRequest(response);
            return Ok(response);
        }

        [HttpGet("revenue/by-method")]
        public async Task<IActionResult> GetRevenueByPaymentMethod()
        {
            var response = await _service.GetRevenueByPaymentMethodAsync(GetUserId());
            if (!response.Success) return BadRequest(response);
            return Ok(response);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] PaymentCreateDTO dto)
        {
            var response = await _service.CreateAsync(dto, GetUserId());
            if (!response.Success) return BadRequest(response);

            return Ok(response);
        }


        [HttpPost("range")]
        public async Task<IActionResult> CreateRange([FromBody] IEnumerable<PaymentCreateDTO> dtos)
        {
            var response = await _service.CreateRangeAsync(dtos, GetUserId());
            if (!response.Success) return BadRequest(response);
            return Ok(response);
        }

        [HttpPost("process/{paymentMethodId}")]
        public async Task<IActionResult> ProcessPayment(string paymentMethodId, [FromBody] PaymentCreateDTO dto)
        {
            var response = await _service.ProcessPaymentAsync(paymentMethodId, dto, GetUserId());
            if (!response.Success) return BadRequest(response);
            return Ok(response);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(string id, [FromBody] PaymentUpdateDTO dto)
        {
            var response = await _service.UpdateAsync(id, dto, GetUserId());
            if (!response.Success) return NotFound(response);
            return Ok(response);
        }

        [HttpPut("range")]
        public async Task<IActionResult> UpdateRange([FromBody] IEnumerable<PaymentUpdateDTO> dtos)
        {
            var response = await _service.UpdateRangeAsync(dtos, GetUserId());
            if (!response.Success) return BadRequest(response);
            return Ok(response);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            var response = await _service.DeleteAsync(id, GetUserId());
            if (!response.Success) return NotFound(response);
            return Ok(response);
        }

        [HttpDelete("range")]
        public async Task<IActionResult> DeleteRange([FromBody] IEnumerable<string> ids)
        {
            var response = await _service.DeleteRangeAsync(ids, GetUserId());
            if (!response.Success) return BadRequest(response);
            return Ok(response);
        }

        [HttpPost("refund/{paymentId}")]
        public async Task<IActionResult> Refund(string paymentId)
        {
            var response = await _service.RefundPaymentAsync(paymentId, GetUserId());
            if (!response.Success) return BadRequest(response);
            return Ok(response);
        }

        [HttpPost("cancel/{paymentId}")]
        public async Task<IActionResult> Cancel(string paymentId)
        {
            var response = await _service.CancelPaymentAsync(paymentId, GetUserId());
            if (!response.Success) return BadRequest(response);
            return Ok(response);
        }

        [HttpPost("complete/{paymentId}")]
        public async Task<IActionResult> MarkAsCompleted(string paymentId)
        {
            var response = await _service.MarkAsCompletedAsync(paymentId, GetUserId());
            if (!response.Success) return BadRequest(response);
            return Ok(response);
        }

        [HttpPost("fail/{paymentId}")]
        public async Task<IActionResult> MarkAsFailed(string paymentId, [FromQuery] string reason)
        {
            var response = await _service.MarkAsFailedAsync(paymentId, reason, GetUserId());
            if (!response.Success) return BadRequest(response);
            return Ok(response);
        }

        [HttpPost("expire/{paymentId}")]
        public async Task<IActionResult> Expire(string paymentId)
        {
            var response = await _service.ExpirePaymentAsync(paymentId, GetUserId());
            if (!response.Success) return BadRequest(response);
            return Ok(response);
        }

        [HttpPost("reactivate/{paymentId}")]
        public async Task<IActionResult> Reactivate(string paymentId)
        {
            var response = await _service.ReactivatePaymentAsync(paymentId, GetUserId());
            if (!response.Success) return BadRequest(response);
            return Ok(response);
        }

        [HttpGet("exists/{id}")]
        public async Task<IActionResult> Exists(string id)
        {
            var exists = await _service.ExistsAsync(id, GetUserId());
            return Ok(new GeneralResponse<bool>(true, exists ? "Payment exists" : "Payment not found", exists));
        }

        [HttpGet("count")]
        public async Task<IActionResult> Count()
        {
            var count = await _service.CountAsync(GetUserId());
            return Ok(new GeneralResponse<int>(true, "Total payments retrieved successfully", count));
        }
    }
}