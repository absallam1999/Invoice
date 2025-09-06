using invoice.Core.DTO;
using invoice.Core.DTO.Order;
using invoice.Core.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace invoice.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class OrderController : ControllerBase
    {

        private readonly IOrderService _orderService;

        public OrderController(IOrderService orderService)
        {
            _orderService = orderService;
        }

        private string GetUserId()
        {
            return User.FindFirstValue(ClaimTypes.NameIdentifier) ?? string.Empty;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var response = await _orderService.GetAllAsync(GetUserId());
            return StatusCode(response.Success ? 200 : 400, response);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(string id)
        {
            var response = await _orderService.GetByIdAsync(id, GetUserId());
            return StatusCode(response.Success ? 200 : 404, response);
        }

        [HttpPost]
        public async Task<IActionResult> Add([FromBody] OrderCreateDTO dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(new GeneralResponse<OrderReadDTO>(false, "Invalid order data."));

            var response = await _orderService.AddAsync(dto, GetUserId());
            return StatusCode(response.Success ? 201 : 400, response);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(string id, [FromBody] OrderUpdateDTO dto)
        {
            if (id != dto.Id)
                return BadRequest(new GeneralResponse<OrderReadDTO>(false, "Order ID mismatch."));

            var response = await _orderService.UpdateAsync(dto, GetUserId());
            return StatusCode(response.Success ? 200 : 400, response);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            var response = await _orderService.DeleteAsync(id, GetUserId());
            return StatusCode(response.Success ? 200 : 404, response);
        }

        [HttpPost("range")]
        public async Task<IActionResult> AddRange([FromBody] IEnumerable<OrderCreateDTO> dtos)
        {
            if (!ModelState.IsValid)
                return BadRequest(new GeneralResponse<IEnumerable<OrderReadDTO>>(false, "Invalid orders data."));

            var response = await _orderService.AddRangeAsync(dtos, GetUserId());
            return StatusCode(response.Success ? 201 : 400, response);
        }

        [HttpPut("range")]
        public async Task<IActionResult> UpdateRange([FromBody] IEnumerable<OrderUpdateDTO> dtos)
        {
            var response = await _orderService.UpdateRangeAsync(dtos, GetUserId());
            return StatusCode(response.Success ? 200 : 400, response);
        }

        [HttpDelete("range")]
        public async Task<IActionResult> DeleteRange([FromBody] IEnumerable<string> ids)
        {
            var response = await _orderService.DeleteRangeAsync(ids, GetUserId());
            return StatusCode(response.Success ? 200 : 404, response);
        }
    }
}
