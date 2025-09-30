using invoice.Core.DTO;
using invoice.Core.Entities;
using invoice.Core.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace invoice.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class CommissionController : ControllerBase
    {
        private readonly ICommissionService _commissionService;

        public CommissionController(ICommissionService commissionService)
        {
            _commissionService = commissionService;
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<GeneralResponse<Commission>>> GetById(string id)
        {
            var result = await _commissionService.GetByIdAsync(id);
            return StatusCode(result.Success ? 200 : 404, result);
        }

        [HttpGet("all")]
        public async Task<ActionResult<GeneralResponse<IEnumerable<Commission>>>> GetAll()
        {
            var result = await _commissionService.GetAllAsync();
            return Ok(result);
        }

        [HttpGet("seller/{sellerId}")]
        public async Task<ActionResult<GeneralResponse<IEnumerable<Commission>>>> GetBySellerId(string sellerId)
        {
            var result = await _commissionService.GetBySellerIdAsync(sellerId);
            return Ok(result);
        }

        [HttpPost]
        public async Task<ActionResult<GeneralResponse<Commission>>> Add([FromBody] Commission commission)
        {
            var result = await _commissionService.AddAsync(commission);
            return StatusCode(result.Success ? 201 : 400, result);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<GeneralResponse<Commission>>> Update(string id, [FromBody] Commission commission)
        {
            if (id != commission.Id)
                return BadRequest(new GeneralResponse<Commission>(false, "Commission ID mismatch"));

            var result = await _commissionService.UpdateAsync(commission);
            return StatusCode(result.Success ? 200 : 400, result);
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult<GeneralResponse<Commission>>> Delete(string id)
        {
            var result = await _commissionService.DeleteAsync(id);
            return StatusCode(result.Success ? 200 : 404, result);
        }

        [HttpPost("settle/{commissionId}")]
        public async Task<ActionResult<GeneralResponse<Commission>>> Settle(string commissionId)
        {
            var result = await _commissionService.SettleCommissionAsync(commissionId);
            return StatusCode(result.Success ? 200 : 404, result);
        }

        [HttpGet("totals/seller/{sellerId}/unsettled")]
        public async Task<ActionResult<GeneralResponse<decimal>>> GetUnsettledTotalForSeller(string sellerId)
        {
            var result = await _commissionService.GetUnsettledTotalForSellerAsync(sellerId);
            return Ok(result);
        }

        [HttpGet("totals/seller/{sellerId}")]
        public async Task<ActionResult<GeneralResponse<decimal>>> GetTotalForSeller(string sellerId)
        {
            var result = await _commissionService.GetTotalForSellerAsync(sellerId);
            return Ok(result);
        }

        [HttpGet("totals/seller/{sellerId}/settled")]
        public async Task<ActionResult<GeneralResponse<decimal>>> GetTotalSettledForSeller(string sellerId)
        {
            var result = await _commissionService.GetTotalSettledForSellerAsync(sellerId);
            return Ok(result);
        }

        [HttpGet("totals/all")]
        public async Task<ActionResult<GeneralResponse<decimal>>> GetTotalCommissions()
        {
            var result = await _commissionService.GetTotalCommissionsAsync();
            return Ok(result);
        }

        [HttpGet("totals/settled")]
        public async Task<ActionResult<GeneralResponse<decimal>>> GetTotalSettledCommissions()
        {
            var result = await _commissionService.GetTotalSettledCommissionsAsync();
            return Ok(result);
        }

        [HttpGet("totals/unsettled")]
        public async Task<ActionResult<GeneralResponse<decimal>>> GetTotalUnsettledCommissions()
        {
            var result = await _commissionService.GetTotalUnsettledCommissionsAsync();
            return Ok(result);
        }

        [HttpGet("invoice/{invoiceId}")]
        public async Task<ActionResult<GeneralResponse<Commission>>> GetByInvoiceId(string invoiceId)
        {
            var result = await _commissionService.GetByInvoiceIdAsync(invoiceId);
            return StatusCode(result.Success ? 200 : 404, result);
        }

        [HttpGet("count")]
        public async Task<ActionResult<GeneralResponse<int>>> Count([FromQuery] string? sellerId = null)
        {
            var result = await _commissionService.CountAsync(sellerId);
            return Ok(result);
        }

        [HttpGet("exists/{commissionId}")]
        public async Task<ActionResult<GeneralResponse<bool>>> Exists(string commissionId)
        {
            var result = await _commissionService.ExistsAsync(commissionId);
            return Ok(result);
        }
    }
}
