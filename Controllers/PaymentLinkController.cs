using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using invoice.Core.DTO;
using invoice.Core.DTO.PaymentLink;
using invoice.Core.Entites;
using invoice.Core.Interfaces.Services;
using System.Linq.Expressions;
using Microsoft.AspNetCore.Authorization;

namespace invoice.Controllers
{
    [Authorize]
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

        [HttpPost]
        public async Task<ActionResult<GeneralResponse<PaymentLinkReadDTO>>> Create(PaymentLinkCreateDTO dto)
        {
            var userId = GetUserId();
            var result = await _paymentLinkService.CreateAsync(dto, userId);

            return result.Success
                ? CreatedAtAction(nameof(GetById), new { id = result.Data.Id }, result)
                : BadRequest(result);
        }

        [HttpPost("custom")]
        public async Task<ActionResult<GeneralResponse<PaymentLinkReadDTO>>> CreateCustom(CustomPaymentLinkCreateDTO dto)
        {
            var userId = GetUserId();
            var result = await _paymentLinkService.CreateCustomPaymentLinkAsync(dto, userId);

            return result.Success
                ? CreatedAtAction(nameof(GetById), new { id = result.Data.Id }, result)
                : BadRequest(result);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<GeneralResponse<PaymentLinkReadDTO>>> Update(string id, PaymentLinkUpdateDTO dto)
        {
            var userId = GetUserId();
            var result = await _paymentLinkService.UpdateAsync(id, dto, userId);

            return result.Success ? Ok(result) : NotFound(result);
        }

        [HttpPut("range")]
        public async Task<ActionResult<GeneralResponse<IEnumerable<PaymentLinkReadDTO>>>> UpdateRange(IEnumerable<PaymentLinkUpdateDTO> dtos)
        {
            var userId = GetUserId();
            var result = await _paymentLinkService.UpdateRangeAsync(dtos, userId);

            return Ok(result);
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult<GeneralResponse<bool>>> Delete(string id)
        {
            var userId = GetUserId();
            var result = await _paymentLinkService.DeleteAsync(id, userId);

            return result.Success ? Ok(result) : NotFound(result);
        }

        [HttpDelete("range")]
        public async Task<ActionResult<GeneralResponse<bool>>> DeleteRange([FromBody] IEnumerable<string> ids)
        {
            var userId = GetUserId();
            var result = await _paymentLinkService.DeleteRangeAsync(ids, userId);

            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<GeneralResponse<PaymentLinkReadDTO>>> GetById(string id)
        {
            var userId = GetUserId();
            var result = await _paymentLinkService.GetByIdAsync(id, userId);

            return result.Success ? Ok(result) : NotFound(result);
        }

        [HttpGet]
        public async Task<ActionResult<GeneralResponse<IEnumerable<PaymentLinkReadDTO>>>> GetAll()
        {
            var userId = GetUserId();
            var result = await _paymentLinkService.GetAllAsync(userId);

            return Ok(result);
        }

        [HttpGet("custom")]
        public async Task<ActionResult<GeneralResponse<IEnumerable<PaymentLinkReadDTO>>>> GetCustomLinks()
        {
            var userId = GetUserId();
            var result = await _paymentLinkService.GetCustomPaymentLinksAsync(userId);

            return Ok(result);
        }

        [HttpGet("query")]
        public async Task<ActionResult<GeneralResponse<IEnumerable<PaymentLinkReadDTO>>>> Query(
            [FromQuery] string purpose = null,
            [FromQuery] decimal? minValue = null,
            [FromQuery] decimal? maxValue = null,
            [FromQuery] bool? isActive = null)
        {
            var userId = GetUserId();

            Expression<Func<PaymentLink, bool>> predicate = pl =>
                (string.IsNullOrEmpty(purpose) || pl.Purpose.Contains(purpose)) &&
                (!minValue.HasValue || pl.Value >= minValue.Value) &&
                (!maxValue.HasValue || pl.Value <= maxValue.Value) &&
                (!isActive.HasValue || pl.IsActive == isActive.Value);

            var result = await _paymentLinkService.QueryAsync(predicate, userId);

            return Ok(result);
        }

        [HttpPost("generate/{invoiceId}")]
        public async Task<ActionResult<GeneralResponse<PaymentLinkReadDTO>>> GenerateLink(string invoiceId, [FromBody] decimal value)
        {
            var userId = GetUserId();
            var result = await _paymentLinkService.GenerateLinkAsync(invoiceId, value, userId);

            return result.Success ? Ok(result) : BadRequest(result);
        }

        [HttpPost("refresh/{paymentLinkId}")]
        public async Task<ActionResult<GeneralResponse<PaymentLinkReadDTO>>> RefreshGatewayLink(string paymentLinkId)
        {
            var userId = GetUserId();
            var result = await _paymentLinkService.RefreshGatewayLinkAsync(paymentLinkId, userId);

            return result.Success ? Ok(result) : BadRequest(result);
        }

        [HttpPost("{paymentLinkId}/payments")]
        public async Task<ActionResult<GeneralResponse<bool>>> AttachPayment(string paymentLinkId, [FromBody] Payment payment)
        {
            var userId = GetUserId();
            var result = await _paymentLinkService.AttachPaymentAsync(paymentLinkId, payment, userId);

            return result.Success ? Ok(result) : BadRequest(result);
        }

        [HttpGet("revenue/custom")]
        public async Task<ActionResult<GeneralResponse<decimal>>> GetCustomRevenue()
        {
            var userId = GetUserId();
            var result = await _paymentLinkService.GetTotalRevenueFromCustomLinksAsync(userId);

            return Ok(result);
        }

        [HttpGet("validate/{paymentLinkId}")]
        public async Task<ActionResult<GeneralResponse<bool>>> Validate(string paymentLinkId)
        {
            var userId = GetUserId();
            var result = await _paymentLinkService.ValidatePaymentLinkAsync(paymentLinkId, userId);

            return Ok(result);
        }

        [HttpPost("deactivate-expired")]
        public async Task<ActionResult<GeneralResponse<bool>>> DeactivateExpired()
        {
            var userId = GetUserId();
            var result = await _paymentLinkService.DeactivateExpiredLinksAsync(userId);

            return Ok(result);
        }

        [HttpGet("active")]
        public async Task<ActionResult<GeneralResponse<IEnumerable<PaymentLinkReadDTO>>>> GetActive()
        {
            var userId = GetUserId();
            var result = await _paymentLinkService.GetActiveLinksAsync(userId);

            return Ok(result);
        }

        [HttpGet("expiring-soon/{daysThreshold}")]
        public async Task<ActionResult<GeneralResponse<IEnumerable<PaymentLinkReadDTO>>>> GetExpiringSoon(int daysThreshold)
        {
            var userId = GetUserId();
            var result = await _paymentLinkService.GetExpiringSoonLinksAsync(daysThreshold, userId);

            return Ok(result);
        }

        [HttpGet("exists/{id}")]
        public async Task<ActionResult<bool>> Exists(string id)
        {
            var userId = GetUserId();
            var result = await _paymentLinkService.ExistsAsync(id, userId);

            return Ok(result);
        }

        [HttpGet("count")]
        public async Task<ActionResult<int>> Count()
        {
            var userId = GetUserId();
            var result = await _paymentLinkService.CountAsync(userId);

            return Ok(result);
        }

        [HttpGet("count/active")]
        public async Task<ActionResult<int>> CountActive()
        {
            var userId = GetUserId();
            var result = await _paymentLinkService.CountActiveAsync(userId);

            return Ok(result);
        }

        [HttpGet("count/invoice/{invoiceId}")]
        public async Task<ActionResult<int>> CountByInvoice(string invoiceId)
        {
            var userId = GetUserId();
            var result = await _paymentLinkService.CountByInvoiceAsync(invoiceId, userId);

            return Ok(result);
        }
    }
}