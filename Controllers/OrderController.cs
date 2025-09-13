using invoice.Core.DTO;
using invoice.Core.DTO.Order;
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
    public class OrdersController : ControllerBase
    {
        private readonly IOrderService _orderService;

        public OrdersController(IOrderService orderService)
        {
            _orderService = orderService;
        }

        private string GetUserId() => User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? string.Empty;

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] OrderCreateDTO dto)
        {
            if (string.IsNullOrWhiteSpace(GetUserId()))
                return Unauthorized();

            var result = await _orderService.CreateAsync(dto, GetUserId());
            return result.Success ? Ok(result) : BadRequest(result);
        }

        [HttpPost("range")]
        public async Task<IActionResult> CreateRange([FromBody] IEnumerable<OrderCreateDTO> dtos)
        {
            if (string.IsNullOrWhiteSpace(GetUserId()))
                return Unauthorized();

            var result = await _orderService.CreateRangeAsync(dtos, GetUserId());
            return result.Success ? Ok(result) : BadRequest(result);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(string id, [FromBody] OrderUpdateDTO dto)
        {
            if (string.IsNullOrWhiteSpace(GetUserId()))
                return Unauthorized();

            var result = await _orderService.UpdateAsync(id, dto, GetUserId());
            return result.Success ? Ok(result) : BadRequest(result);
        }

        [HttpPut("range")]
        public async Task<IActionResult> UpdateRange([FromBody] IEnumerable<OrderUpdateDTO> dtos)
        {
            if (string.IsNullOrWhiteSpace(GetUserId()))
                return Unauthorized();

            var result = await _orderService.UpdateRangeAsync(dtos, GetUserId());
            return result.Success ? Ok(result) : BadRequest(result);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            if (string.IsNullOrWhiteSpace(GetUserId()))
                return Unauthorized();

            var result = await _orderService.DeleteAsync(id, GetUserId());
            return result.Success ? Ok(result) : BadRequest(result);
        }

        [HttpDelete("range")]
        public async Task<IActionResult> DeleteRange([FromBody] IEnumerable<string> ids)
        {
            if (string.IsNullOrWhiteSpace(GetUserId()))
                return Unauthorized();

            var result = await _orderService.DeleteRangeAsync(ids, GetUserId());
            return result.Success ? Ok(result) : BadRequest(result);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(string id)
        {
            if (string.IsNullOrWhiteSpace(GetUserId()))
                return Unauthorized();

            var result = await _orderService.GetByIdAsync(id, GetUserId());
            return result.Success ? Ok(result) : NotFound(result);
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            if (string.IsNullOrWhiteSpace(GetUserId()))
                return Unauthorized();

            var result = await _orderService.GetAllAsync(GetUserId());
            return result.Success ? Ok(result) : NotFound(result);
        }

        [HttpGet("client/{clientId}")]
        public async Task<IActionResult> GetByClient(string clientId)
        {
            if (string.IsNullOrWhiteSpace(GetUserId()))
                return Unauthorized();

            var result = await _orderService.GetByClientAsync(clientId, GetUserId());
            return result.Success ? Ok(result) : NotFound(result);
        }

        [HttpGet("status/{status}")]
        public async Task<IActionResult> GetByStatus(OrderStatus status)
        {
            if (string.IsNullOrWhiteSpace(GetUserId()))
                return Unauthorized();

            var result = await _orderService.GetByStatusAsync(status, GetUserId());
            return result.Success ? Ok(result) : NotFound(result);
        }

        [HttpGet("revenue")]
        public async Task<IActionResult> GetTotalRevenue()
        {
            if (string.IsNullOrWhiteSpace(GetUserId()))
                return Unauthorized();

            var result = await _orderService.GetTotalRevenueAsync(GetUserId());
            return result.Success ? Ok(result) : BadRequest(result);
        }

        [HttpGet("count")]
        public async Task<IActionResult> Count()
        {
            if (string.IsNullOrWhiteSpace(GetUserId()))
                return Unauthorized();

            var count = await _orderService.CountAsync(GetUserId());
            return Ok(new { Success = true, Count = count });
        }

        [HttpGet("count/status/{status}")]
        public async Task<IActionResult> CountByStatus(OrderStatus status)
        {
            if (string.IsNullOrWhiteSpace(GetUserId()))
                return Unauthorized();

            var count = await _orderService.CountByStatusAsync(status, GetUserId());
            return Ok(new { Success = true, Count = count });
        }

        [HttpHead("{id}")]
        public async Task<IActionResult> Exists(string id)
        {
            if (string.IsNullOrWhiteSpace(GetUserId()))
                return Unauthorized();

            var exists = await _orderService.ExistsAsync(id, GetUserId());
            return exists ? Ok(new { Success = true, Exists = true }) : NotFound(new { Success = false, Exists = false });
        }
    }
}
