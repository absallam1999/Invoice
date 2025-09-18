using AutoMapper;
using invoice.Core.DTO.PayInvoice;
using invoice.Core.Entites;
using invoice.Core.Enums;
using invoice.Core.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace invoice.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class PayInvoiceController : ControllerBase
    {
        private readonly IPayInvoiceService _payInvoiceService;
        private readonly IMapper _mapper;

        public PayInvoiceController(IPayInvoiceService payInvoiceService, IMapper mapper)
        {
            _payInvoiceService = payInvoiceService;
            _mapper = mapper;
        }

        private string GetUserId()
        {
            return User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? string.Empty;
        }

        [HttpPost("session")]
        public async Task<IActionResult> CreateSession([FromQuery] string invoiceId, [FromQuery] PaymentType paymentType)
        {
            var response = await _payInvoiceService.CreatePaymentSessionAsync(invoiceId, paymentType, GetUserId());
            return Ok(response);
        }

        [HttpPost("callback")]
        public async Task<IActionResult> ProcessCallback([FromQuery] string sessionId, [FromQuery] PaymentType paymentType,
                                                         [FromQuery] bool isSuccess, [FromBody] string? callbackData)
        {
            var response = await _payInvoiceService.ProcessPaymentCallbackAsync(sessionId, paymentType, isSuccess, callbackData);
            return Ok(response);
        }

        [HttpPost("refund")]
        public async Task<IActionResult> Refund([FromQuery] string paymentId, [FromQuery] PaymentType paymentType,
                                                [FromQuery] decimal? amount = null)
        {
            var response = await _payInvoiceService.RefundPaymentAsync(paymentId, paymentType, amount);
            return Ok(response);
        }

        [HttpPost("cancel/{paymentId}")]
        public async Task<IActionResult> Cancel(string paymentId)
        {
            var response = await _payInvoiceService.CancelPaymentAsync(paymentId);
            return Ok(response);
        }

        [HttpPost("retry/{paymentId}")]
        public async Task<IActionResult> RetryFailed(string paymentId)
        {
            var response = await _payInvoiceService.RetryFailedPaymentAsync(paymentId);
            return Ok(response);
        }

        [HttpGet("status/{paymentId}")]
        public async Task<IActionResult> GetStatus(string paymentId)
        {
            var response = await _payInvoiceService.GetPaymentStatusAsync(paymentId);
            return Ok(response);
        }

        [HttpGet("status/session/{sessionId}")]
        public async Task<IActionResult> GetStatusBySession(string sessionId, [FromQuery] PaymentType paymentType)
        {
            var response = await _payInvoiceService.GetPaymentStatusBySessionAsync(sessionId, paymentType);
            return Ok(response);
        }

        [HttpGet("status/invoice/{invoiceId}")]
        public async Task<IActionResult> GetStatusesByInvoice(string invoiceId)
        {
            var response = await _payInvoiceService.GetPaymentStatusesByInvoiceAsync(invoiceId);
            return Ok(response);
        }

        [HttpGet("status")]
        public async Task<IActionResult> GetByStatus([FromQuery] PaymentStatus status)
        {
            var response = await _payInvoiceService.GetByStatusAsync(status, GetUserId());
            return Ok(response);
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var response = await _payInvoiceService.GetAllAsync(GetUserId());
            return Ok(response);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(string id)
        {
            var response = await _payInvoiceService.GetByIdAsync(id, GetUserId());
            return Ok(response);
        }

        [HttpGet("session/{sessionId}")]
        public async Task<IActionResult> GetBySessionId(string sessionId)
        {
            var response = await _payInvoiceService.GetBySessionIdAsync(sessionId, GetUserId());
            return Ok(response);
        }

        [HttpGet("invoice/{invoiceId}")]
        public async Task<IActionResult> GetByInvoiceId(string invoiceId)
        {
            var response = await _payInvoiceService.GetByInvoiceIdAsync(invoiceId, GetUserId());
            return Ok(response);
        }

        [HttpGet("paymentmethod/{paymentMethodId}")]
        public async Task<IActionResult> GetByPaymentMethodId(string paymentMethodId)
        {
            var response = await _payInvoiceService.GetByPaymentMethodIdAsync(paymentMethodId, GetUserId());
            return Ok(response);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] PayInvoiceCreateDTO dto)
        {
            var response = await _payInvoiceService.CreateAsync(_mapper.Map<PayInvoice>(dto));
            return Ok(response);
        }

        [HttpPost("range")]
        public async Task<IActionResult> CreateRange([FromBody] IEnumerable<PayInvoiceCreateDTO> dtos)
        {
            var response = await _payInvoiceService.CreateRangeAsync(_mapper.Map<IEnumerable<PayInvoice>>(dtos));
            return Ok(response);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(string id, [FromBody] PayInvoiceUpdateDTO dto)
        {
            var response = await _payInvoiceService.UpdateAsync(id, _mapper.Map<PayInvoice>(dto));
            return Ok(response);
        }

        [HttpPut("{id}/status")]
        public async Task<IActionResult> UpdateStatus(string id, [FromQuery] PaymentStatus status, [FromBody] string? callbackData)
        {
            var response = await _payInvoiceService.UpdateStatusAsync(id, status, callbackData);
            return Ok(response);
        }

        [HttpPut("range")]
        public async Task<IActionResult> UpdateRange([FromBody] IEnumerable<PayInvoiceUpdateDTO> dtos)
        {
            var response = await _payInvoiceService.UpdateRangeAsync(_mapper.Map<IEnumerable<PayInvoice>>(dtos));
            return Ok(response);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            var response = await _payInvoiceService.DeleteAsync(id);
            return Ok(response);
        }

        [HttpDelete("session/{sessionId}")]
        public async Task<IActionResult> DeleteBySession(string sessionId)
        {
            var response = await _payInvoiceService.DeleteBySessionIdAsync(sessionId);
            return Ok(response);
        }

        [HttpDelete("range")]
        public async Task<IActionResult> DeleteRange([FromBody] IEnumerable<string> ids)
        {
            var response = await _payInvoiceService.DeleteRangeAsync(ids);
            return Ok(response);
        }


        [HttpGet("exists/{id}")]
        public async Task<IActionResult> Exists(string id)
        {
            var response = await _payInvoiceService.ExistsAsync(id);
            return Ok(response);
        }

        [HttpGet("exists/session/{sessionId}")]
        public async Task<IActionResult> ExistsBySession(string sessionId)
        {
            var response = await _payInvoiceService.ExistsBySessionAsync(sessionId);
            return Ok(response);
        }

        [HttpGet("count")]
        public async Task<IActionResult> Count([FromQuery] string? invoiceId = null)
        {
            var response = await _payInvoiceService.CountAsync(invoiceId);
            return Ok(response);
        }

        [HttpGet("count/status")]
        public async Task<IActionResult> CountByStatus([FromQuery] PaymentStatus status, [FromQuery] string? invoiceId = null)
        {
            var response = await _payInvoiceService.CountByStatusAsync(status, invoiceId);
            return Ok(response);
        }

        [HttpGet("totalpaid/{invoiceId}")]
        public async Task<IActionResult> GetTotalPaidAmount(string invoiceId)
        {
            var response = await _payInvoiceService.GetTotalPaidAmountAsync(invoiceId);
            return Ok(response);
        }
    }
}
