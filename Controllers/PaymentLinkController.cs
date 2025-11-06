using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using invoice.Core.DTO;
using invoice.Core.DTO.PaymentLink;
using invoice.Core.Entities;
using invoice.Core.Interfaces.Services;
using System.Linq.Expressions;
using Microsoft.AspNetCore.Authorization;
using invoice.Services;
using invoice.Core.DTO.Store;

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
            var result = await _paymentLinkService.CreateAsync(dto, GetUserId());

            return Ok(result);

        }

        [HttpPut("{id}")]
        public async Task<ActionResult<GeneralResponse<PaymentLinkReadDTO>>> Update(string id, PaymentLinkUpdateDTO dto)
        {
            var result = await _paymentLinkService.UpdateAsync(id, dto, GetUserId());

            return result.Success ? Ok(result) : NotFound(result);
        }

        [HttpPut("range")]
        public async Task<ActionResult<GeneralResponse<IEnumerable<PaymentLinkReadDTO>>>> UpdateRange(IEnumerable<PaymentLinkUpdateDTO> dtos)
        {
            var result = await _paymentLinkService.UpdateRangeAsync(dtos, GetUserId());

            return Ok(result);
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult<GeneralResponse<bool>>> Delete(string id)
        {
            var result = await _paymentLinkService.DeleteAsync(id, GetUserId());

            return result.Success ? Ok(result) : NotFound(result);
        }

        [HttpDelete("range")]
        public async Task<ActionResult<GeneralResponse<bool>>> DeleteRange([FromBody] IEnumerable<string> ids)
        {
            var result = await _paymentLinkService.DeleteRangeAsync(ids, GetUserId());

            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<GeneralResponse<PaymentLinkReadDTO>>> GetById(string id)
        {
            var result = await _paymentLinkService.GetByIdAsync(id, GetUserId());

            return result.Success ? Ok(result) : NotFound(result);
        }

        [HttpGet]
        public async Task<ActionResult<GeneralResponse<IEnumerable<GetAllPaymentLinkDTO>>>> GetAll()
        {
            var result = await _paymentLinkService.GetAllAsync(GetUserId());

            return Ok(result);
        }


        [HttpPut("activate/{id}")]
        public async Task<IActionResult> ActivateStore(string id)
        {
            var result = await _paymentLinkService.ActivatePaymentLinkAsync(id, GetUserId());
            return Ok(result);
        }


        [AllowAnonymous]
        [HttpGet("slug/{slug}")]
        public async Task<IActionResult> GetBySlug(string slug)
        {
            var result = await _paymentLinkService.GetBySlug(slug);
            return Ok(result);
        }


        //[HttpGet("query")]
        //public async Task<ActionResult<GeneralResponse<IEnumerable<PaymentLinkReadDTO>>>> Query(
        //    [FromQuery] string purpose = null,
        //    [FromQuery] decimal? minValue = null,
        //    [FromQuery] decimal? maxValue = null,
        //    [FromQuery] bool? isActive = null)
        //{
        //    Expression<Func<PaymentLink, bool>> predicate = pl =>
        //        (string.IsNullOrEmpty(purpose) || pl.Purpose.Contains(purpose)) &&
        //        (!minValue.HasValue || pl.Value >= minValue.Value) &&
        //        (!maxValue.HasValue || pl.Value <= maxValue.Value) &&
        //        (!isActive.HasValue || pl.IsActive == isActive.Value);

        //    var result = await _paymentLinkService.QueryAsync(predicate, GetUserId());

        //    return Ok(result);
        //}

        [HttpGet("exists/{id}")]
        public async Task<ActionResult<bool>> Exists(string id)
        {
            var result = await _paymentLinkService.ExistsAsync(id, GetUserId());

            return Ok(new { Success = true, Exists = result });
        }

        [HttpGet("count")]
        public async Task<IActionResult> Count()
        {
            var count = await _paymentLinkService.CountAsync(GetUserId());

            return Ok(new { Count = count });
        }


        [AllowAnonymous]
        [HttpPost("CreatePayment/{linkid}/{userid}")]
        public async Task<IActionResult> CreateOrder(string linkid ,string userid, [FromBody] CreatePaymentDTO dto)
        {
            var result = await _paymentLinkService.CreatePaymentAsync(dto,linkid, userid);
            return Ok(result);
        }

    }
}