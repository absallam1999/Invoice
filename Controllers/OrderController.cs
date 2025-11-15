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

        private string? GetUserId() => User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        //private IActionResult UnauthorizedIfNoUserId(out string userId)
        //{
        //    userId = GetUserId() ?? string.Empty;
        //    if (string.IsNullOrWhiteSpace(userId))
        //        return Unauthorized(new { Success = false, Message = "User not authorized" });

        //    return null!;
        //}

        //[HttpPost]
        //public async Task<IActionResult> Create([FromBody] OrderCreateDTO dto)
        //{
        //    var unauth = UnauthorizedIfNoUserId(out var userId);
        //    if (unauth != null) return unauth;

        //    var result = await _orderService.CreateAsync(dto, userId);
        //    return result.Success ? Ok(result) : BadRequest(result);
        //}

        //[HttpPost("range")]
        //public async Task<IActionResult> CreateRange([FromBody] IEnumerable<OrderCreateDTO> dtos)
        //{
        //    var unauth = UnauthorizedIfNoUserId(out var userId);
        //    if (unauth != null) return unauth;

        //    var result = await _orderService.CreateRangeAsync(dtos, userId);
        //    return result.Success ? Ok(result) : BadRequest(result);
        //}

        //[HttpPut("{id}")]
        //public async Task<IActionResult> Update(string id, [FromBody] OrderUpdateDTO dto)
        //{
        //    var unauth = UnauthorizedIfNoUserId(out var userId);
        //    if (unauth != null) return unauth;

        //    var result = await _orderService.UpdateAsync(id, dto, userId);
        //    return result.Success ? Ok(result) : BadRequest(result);
        //}

        //[HttpPut("range")]
        //public async Task<IActionResult> UpdateRange([FromBody] IEnumerable<OrderUpdateDTO> dtos)
        //{
        //    var unauth = UnauthorizedIfNoUserId(out var userId);
        //    if (unauth != null) return unauth;

        //    var result = await _orderService.UpdateRangeAsync(dtos, userId);
        //    return result.Success ? Ok(result) : BadRequest(result);
        //}

        //[HttpDelete("{id}")]
        //public async Task<IActionResult> Delete(string id)
        //{
        //    var unauth = UnauthorizedIfNoUserId(out var userId);
        //    if (unauth != null) return unauth;

        //    var result = await _orderService.DeleteAsync(id, userId);
        //    return result.Success ? Ok(result) : BadRequest(result);
        //}

        //[HttpDelete("range")]
        //public async Task<IActionResult> DeleteRange([FromBody] IEnumerable<string> ids)
        //{
        //    var unauth = UnauthorizedIfNoUserId(out var userId);
        //    if (unauth != null) return unauth;

        //    var result = await _orderService.DeleteRangeAsync(ids, userId);
        //    return result.Success ? Ok(result) : BadRequest(result);
        //}

        //[HttpGet("{id}")]
        //public async Task<IActionResult> GetById(string id)
        //{
        //    var unauth = UnauthorizedIfNoUserId(out var userId);
        //    if (unauth != null) return unauth;

        //    var result = await _orderService.GetByIdAsync(id, userId);
        //    return result.Success ? Ok(result) : NotFound(result);
        //}

        //[HttpGet("all")]
        //public async Task<IActionResult> GetAll()
        //{
        //    var unauth = UnauthorizedIfNoUserId(out var userId);
        //    if (unauth != null) return unauth;

        //    var result = await _orderService.GetAllAsync(userId);
        //    return result.Success ? Ok(result) : NotFound(result);
        //}

        //[HttpGet("client/{clientId}")]
        //public async Task<IActionResult> GetByClient(string clientId)
        //{
        //    var unauth = UnauthorizedIfNoUserId(out var userId);
        //    if (unauth != null) return unauth;

        //    var result = await _orderService.GetByClientAsync(clientId, userId);
        //    return result.Success ? Ok(result) : NotFound(result);
        //}

        //[HttpGet("status/{status}")]
        //public async Task<IActionResult> GetByStatus(OrderStatus status)
        //{
        //    var unauth = UnauthorizedIfNoUserId(out var userId);
        //    if (unauth != null) return unauth;

        //    var result = await _orderService.GetByStatusAsync(status, userId);
        //    return result.Success ? Ok(result) : NotFound(result);
        //}

        //[HttpGet("revenue")]
        //public async Task<IActionResult> GetTotalRevenue()
        //{
        //    var unauth = UnauthorizedIfNoUserId(out var userId);
        //    if (unauth != null) return unauth;

        //    var result = await _orderService.GetTotalRevenueAsync(userId);
        //    return result.Success ? Ok(result) : BadRequest(result);
        //}

        //[HttpGet("count/status/{status}")]
        //public async Task<IActionResult> CountByStatus(OrderStatus status)
        //{
        //    var unauth = UnauthorizedIfNoUserId(out var userId);
        //    if (unauth != null) return unauth;

        //    var count = await _orderService.CountByStatusAsync(status, userId);
        //    return Ok(new { Success = true, Count = count });
        //}

        //[HttpGet("{id}/exists")]
        //public async Task<IActionResult> Exists(string id)
        //{
        //    var unauth = UnauthorizedIfNoUserId(out var userId);
        //    if (unauth != null) return unauth;

        //    var exists = await _orderService.ExistsAsync(id, userId);
        //    return Ok(new { Success = exists, Exists = exists });
        //}

        //[HttpGet("count")]
        //public async Task<IActionResult> Count()
        //{
        //    var unauth = UnauthorizedIfNoUserId(out var userId);
        //    if (unauth != null) return unauth;

        //    var count = await _orderService.CountAsync(userId);
        //    return Ok(new { Success = true, Count = count });
        //}
    }
}
