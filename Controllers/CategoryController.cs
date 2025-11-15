using invoice.Core.DTO.Category;
using invoice.Core.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace invoice.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class CategoryController : ControllerBase
    {
        private readonly ICategoryService _categoryService;

        public CategoryController(ICategoryService categoryService)
        {
            _categoryService = categoryService;
        }

        private string GetUserId() =>
     User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? string.Empty;


        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var response = await _categoryService.GetAllAsync(GetUserId());

            return response.Success ? Ok(response) : BadRequest(response);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(string id)
        {
            var response = await _categoryService.GetByIdAsync(id, GetUserId());

            return response.Success ? Ok(response) : NotFound(response);
        }

        [HttpGet("by-user")]
        public async Task<IActionResult> GetByUserId()
        {
            var response = await _categoryService.GetByUserIdAsync(GetUserId());

            return response.Success ? Ok(response) : NotFound(response);
        }

        [HttpGet("search")]
        public async Task<IActionResult> Search([FromQuery] string keyword)
        {
            var response = await _categoryService.QueryAsync(GetUserId(), keyword);

            return response.Success ? Ok(response) : NotFound(response);
        }

        [HttpGet("count")]
        public async Task<IActionResult> Count()
        {
            var count = await _categoryService.CountAsync(GetUserId());

            return Ok(new { Count = count });
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CategoryCreateDTO dto)
        {
            var response = await _categoryService.CreateAsync(dto, GetUserId());
            return response.Success ? Ok(response) : BadRequest(response);
        }

        [HttpPost("range")]
        public async Task<IActionResult> CreateRange([FromBody] IEnumerable<CategoryCreateDTO> dtos)
        {
            var response = await _categoryService.CreateRangeAsync(dtos, GetUserId());

            return response.Success ? Ok(response) : BadRequest(response);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(string id, [FromBody] CategoryUpdateDTO dto)
        {
            var response = await _categoryService.UpdateAsync(id, dto, GetUserId());

            return response.Success ? Ok(response) : BadRequest(response);
        }

        //[HttpPut("range")]
        //public async Task<IActionResult> UpdateRange([FromBody] IEnumerable<CategoryUpdateDTO> dtos)
        //{
        //    var response = await _categoryService.UpdateRangeAsync(dtos, GetUserId());

        //    return response.Success ? Ok(response) : BadRequest(response);
        //}

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            var response = await _categoryService.DeleteAsync(id, GetUserId());

            return response.Success ? Ok(response) : BadRequest(response);
        }

        [HttpDelete("range")]
        public async Task<IActionResult> DeleteRange([FromBody] IEnumerable<string> ids)
        {
            var response = await _categoryService.DeleteRangeAsync(ids, GetUserId());

            return response.Success ? Ok(response) : BadRequest(response);
        }
    }
}