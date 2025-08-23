using invoice.Core.DTO.ContactInformation;
using invoice.Core.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace invoice.Controllers
{
    //[Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class ContactInfoController : ControllerBase
    {
        private readonly IContactInfoService _contactInfoService;

        public ContactInfoController(IContactInfoService contactInfoService)
        {
            _contactInfoService = contactInfoService;
        }

        private string GetUserId() =>
        User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? string.Empty;

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var response = await _contactInfoService.GetAllAsync(GetUserId());
            return Ok(response);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(string id)
        {
            var response = await _contactInfoService.GetByIdAsync(id, GetUserId());
            return response.Success ? Ok(response) : NotFound(response);
        }

        [HttpGet("store/{storeId}")]
        public async Task<IActionResult> GetByStoreId(string storeId)
        {
            var response = await _contactInfoService.GetByStoreIdAsync(storeId, GetUserId());
            return Ok(response);
        }

        [HttpGet("email/{email}")]
        public async Task<IActionResult> GetByEmail(string email)
        {
            var response = await _contactInfoService.GetByEmailAsync(email, GetUserId());
            return response.Success ? Ok(response) : NotFound(response);
        }

        [HttpGet("search")]
        public async Task<IActionResult> Search([FromQuery] string keyword)
        {
            var response = await _contactInfoService.SearchAsync(keyword, GetUserId());
            return Ok(response);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] ContactInfoCreateDTO dto)
        {
            var response = await _contactInfoService.CreateAsync(dto, GetUserId());
            return response.Success ? Ok(response) : BadRequest(response);
        }

        [HttpPost("range")]
        public async Task<IActionResult> CreateRange([FromBody] IEnumerable<ContactInfoCreateDTO> dtos)
        {
            var response = await _contactInfoService.CreateRangeAsync(dtos, GetUserId());
            return response.Success ? Ok(response) : BadRequest(response);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(string id, [FromBody] ContactInfoUpdateDTO dto)
        {
            var response = await _contactInfoService.UpdateAsync(id, dto, GetUserId());
            return response.Success ? Ok(response) : NotFound(response);
        }

        [HttpPut("range")]
        public async Task<IActionResult> UpdateRange([FromBody] IEnumerable<ContactInfoUpdateDTO> dtos)
        {
            var response = await _contactInfoService.UpdateRangeAsync(dtos, GetUserId());
            return response.Success ? Ok(response) : BadRequest(response);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            var response = await _contactInfoService.DeleteAsync(id, GetUserId());
            return response.Success ? Ok(response) : NotFound(response);
        }

        [HttpDelete("range")]
        public async Task<IActionResult> DeleteRange([FromBody] IEnumerable<string> ids)
        {
            var response = await _contactInfoService.DeleteRangeAsync(ids, GetUserId());
            return response.Success ? Ok(response) : BadRequest(response);
        }

        [HttpGet("exists/{id}")]
        public async Task<IActionResult> Exists(string id)
        {
            var exists = await _contactInfoService.ExistsAsync(id, GetUserId());
            return Ok(new { Exists = exists });
        }

        [HttpGet("count")]
        public async Task<IActionResult> Count()
        {
            var count = await _contactInfoService.CountAsync(GetUserId());
            return Ok(new { Count = count });
        }
    }
}
