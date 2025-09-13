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

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var response = await _service.GetAllAsync(GetUserId());
            return Ok(response);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(string id)
        {
            var response = await _service.GetByIdAsync(id, GetUserId());
            return Ok(response);
        }

        [HttpGet("invoice/{invoiceId}")]
        public async Task<IActionResult> GetByInvoice(string invoiceId)
        {
            var response = await _service.GetByInvoiceIdAsync(invoiceId, GetUserId());
            return Ok(response);
        }

        [HttpGet("method/{paymentMethodId}")]
        public async Task<IActionResult> GetByPaymentMethod(string paymentMethodId)
        {
            var response = await _service.GetByPaymentMethodIdAsync(paymentMethodId, GetUserId());
            return Ok(response);
        }

        [HttpGet("exists/{id}")]
        public async Task<IActionResult> Exists(string id)
        {
            var exists = await _service.ExistsAsync(id, GetUserId());
            return Ok(new { exists });
        }

        [HttpGet("count")]
        public async Task<IActionResult> Count()
        {
            var count = await _service.CountAsync(GetUserId());
            return Ok(new { count });
        }

        [HttpGet("total-paid/{invoiceId}")]
        public async Task<IActionResult> GetTotalPaidByInvoice(string invoiceId)
        {
            var response = await _service.GetTotalPaidByInvoiceAsync(invoiceId, GetUserId());
            return Ok(response);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] PaymentCreateDTO dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(new GeneralResponse<object>
                {
                    Success = false,
                    Message = "Invalid data submitted",
                    Data = ModelState
                });

            var response = await _service.CreateAsync(dto, GetUserId());
            return Ok(response);
        }

        [HttpPost("range")]
        public async Task<IActionResult> CreateRange([FromBody] IEnumerable<PaymentCreateDTO> dtos)
        {
            var response = await _service.CreateRangeAsync(dtos, GetUserId());
            return Ok(response);
        }

        [HttpPost("process/{paymentMethodId}")]
        public async Task<IActionResult> ProcessPayment(string paymentMethodId, [FromBody] PaymentCreateDTO dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(new GeneralResponse<object>
                {
                    Success = false,
                    Message = "Invalid data submitted",
                    Data = ModelState
                });

            var response = await _service.ProcessPaymentAsync(paymentMethodId, dto, GetUserId());
            return Ok(response);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(string id, [FromBody] PaymentUpdateDTO dto)
        {
            var response = await _service.UpdateAsync(id, dto, GetUserId());
            return Ok(response);
        }

        [HttpPut("range")]
        public async Task<IActionResult> UpdateRange([FromBody] IEnumerable<PaymentUpdateDTO> dtos)
        {
            var response = await _service.UpdateRangeAsync(dtos, GetUserId());
            return Ok(response);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            var response = await _service.DeleteAsync(id, GetUserId());
            return Ok(response);
        }

        [HttpDelete("range")]
        public async Task<IActionResult> DeleteRange([FromBody] IEnumerable<string> ids)
        {
            var response = await _service.DeleteRangeAsync(ids, GetUserId());
            return Ok(response);
        }
    }
}

