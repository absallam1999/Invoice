using invoice.Core.DTO.Page;
using invoice.Core.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace invoice.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class PagesController : ControllerBase
    {
        private readonly IPageService _pageService;

        public PagesController(IPageService pageService)
        {
            _pageService = pageService;
        }

        [HttpGet("all")]
        public async Task<IActionResult> GetAll(string storeId)
        {
            var response = await _pageService.GetAllAsync(storeId);
            return Ok(response);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(string id)
        {
            var response = await _pageService.GetByIdAsync(id);
            return Ok(response);
        }

        [HttpGet("search")]
        public async Task<IActionResult> Search(string keyword, [FromQuery] string storeId)
        {
            var response = await _pageService.SearchAsync(keyword, storeId);
            return Ok(response);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromForm] PageCreateDTO dto, [FromForm] PageImageDTO image)
        {
            var response = await _pageService.CreateAsync(dto, image);
            return Ok(response);
        }

        [HttpPost("range")]
        public async Task<IActionResult> CreateRange([FromForm] PageCreateRangeRequest request)
        {
            if (request.Pages == null || !request.Pages.Any())
                return BadRequest(new { Success = false, Message = "No valid DTOs provided." });

            var response = await _pageService.CreateRangeAsync(request);
            return Ok(response);
        }

        [HttpPut]
        public async Task<IActionResult> Update([FromForm] PageUpdateDTO dto, [FromForm] PageImageDTO? image)
        {
            var response = await _pageService.UpdateAsync(dto, image);
            return Ok(response);
        }

        [HttpPut("range")]
        public async Task<IActionResult> UpdateRange([FromForm] PageUpdateRangeRequest request)
        {
            if (request.Pages == null || !request.Pages.Any())
                return BadRequest(new { Success = false, Message = "No valid DTOs provided." });

            var response = await _pageService.UpdateRangeAsync(request);
            return Ok(response);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            var response = await _pageService.DeleteAsync(id);
            return Ok(response);
        }

        [HttpDelete]
        public async Task<IActionResult> DeleteRange([FromBody] IEnumerable<string> ids)
        {
            var response = await _pageService.DeleteRangeAsync(ids);
            return Ok(response);
        }
    }
}
