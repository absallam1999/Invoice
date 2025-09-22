using invoice.Core.DTO.Client;
using invoice.Core.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace invoice.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class ClientController : ControllerBase
    {
        private readonly IClientService _clientService;

        public ClientController(IClientService clientService)
        {
            _clientService = clientService;
        }

        private string GetUserId() =>
            User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? string.Empty;


        [HttpGet("all")]
        public async Task<IActionResult> GetAll()
        {
            var response = await _clientService.GetAllAsync(GetUserId());
            return Ok(response);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(string id)
        {
            var response = await _clientService.GetByIdAsync(id, GetUserId());
            if (!response.Success) return NotFound(response);
            return Ok(response);
        }

        [HttpGet("by-name/{name}")]
        public async Task<IActionResult> GetByName(string name)
        {
            var response = await _clientService.GetByNameAsync(name, GetUserId());
            if (!response.Success) return NotFound(response);
            return Ok(response);
        }

        [HttpGet("search")]
        public async Task<IActionResult> Search([FromQuery] string keyword)
        {
            var response = await _clientService.SearchAsync(keyword, GetUserId());
            return Ok(response);
        }

        [HttpGet("exists/{id}")]
        public async Task<IActionResult> Exists(string id)
        {
            var exists = await _clientService.ExistsAsync(id, GetUserId());
            return Ok(new { Exists = exists });
        }

        [HttpGet("count")]
        public async Task<IActionResult> Count()
        {
            var count = await _clientService.CountAsync(GetUserId());
            return Ok(new { Count = count });
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] ClientCreateDTO dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var response = await _clientService.CreateAsync(dto, GetUserId());
            if (!response.Success) return BadRequest(response);
            return CreatedAtAction(nameof(GetById), new { id = response.Data.Id }, response);
        }

        [HttpPost("range")]
        public async Task<IActionResult> CreateRange([FromBody] IEnumerable<ClientCreateDTO> dtos)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var response = await _clientService.CreateRangeAsync(dtos, GetUserId());
            if (!response.Success) return BadRequest(response);
            return Ok(response);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update([FromRoute] string id, [FromBody] ClientUpdateDTO dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);


            var response = await _clientService.UpdateAsync(id, dto, GetUserId());
            if (!response.Success) return BadRequest(response);

            return Ok(response);
        }

        [HttpPut("range")]
        //public async Task<IActionResult> UpdateRange([FromBody] IEnumerable<ClientUpdateDTO> dtos)
        //{
        //    if (!ModelState.IsValid) return BadRequest(ModelState);

        //    var response = await _clientService.UpdateRangeAsync(dtos, GetUserId());
        //    if (!response.Success) return BadRequest(response);
        //    return Ok(response);
        //}

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            var response = await _clientService.DeleteAsync(id, GetUserId());
            if (!response.Success) return BadRequest(response);
            return Ok(response);
        }

        [HttpDelete("range")]
        public async Task<IActionResult> DeleteRange([FromBody] IEnumerable<string> ids)
        {
            var response = await _clientService.DeleteRangeAsync(ids, GetUserId());
            if (!response.Success) return BadRequest(response);
            return Ok(response);
        }
    }
}