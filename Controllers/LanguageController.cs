using invoice.Core.Interfaces.Services;
using invoice.Core.Entities;
using invoice.Core.Enums;
using Microsoft.AspNetCore.Mvc;
using invoice.Core.DTO.Language;

namespace invoice.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class LanguageController : ControllerBase
    {
        private readonly ILanguageService _languageService;

        public LanguageController(ILanguageService languageService)
        {
            _languageService = languageService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var result = await _languageService.GetAllAsync();
            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(string id)
        {
            var result = await _languageService.GetByIdAsync(id);
            if (!result.Success) return NotFound(result);
            return Ok(result);
        }

        [HttpGet("name/{name}")]
        public async Task<IActionResult> GetByName(LanguageName name)
        {
            var result = await _languageService.GetByNameAsync(name);
            if (!result.Success) return NotFound(result);
            return Ok(result);
        }

        [HttpGet("target/{target}")]
        public async Task<IActionResult> GetByTarget(LanguageTarget target)
        {
            var result = await _languageService.GetByTargetAsync(target);
            if (!result.Success) return NotFound(result);
            return Ok(result);
        }

        [HttpGet("search")]
        public async Task<IActionResult> Search([FromQuery] string keyword)
        {
            var result = await _languageService.SearchAsync(keyword);
            return Ok(result);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateLanguageDTO language)
        {
            var result = await _languageService.CreateAsync(language);
            if (!result.Success) return BadRequest(result);
            return Ok(result);
        }

        [HttpPost("range")]
        public async Task<IActionResult> CreateRange([FromBody] IEnumerable<CreateLanguageDTO> languages)
        {
            var result = await _languageService.CreateRangeAsync(languages);
            if (!result.Success) return BadRequest(result);
            return Ok(result);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(string id, [FromBody] UpdateLanguageDTO language)
        {
            var result = await _languageService.UpdateAsync(id, language);
            if (!result.Success) return BadRequest(result);
            return Ok(result);
        }

        [HttpPut("range")]
        public async Task<IActionResult> UpdateRange([FromBody] IEnumerable<UpdateLanguageDTO> languages)
        {
            var result = await _languageService.UpdateRangeAsync(languages);
            if (!result.Success) return BadRequest(result);
            return Ok(result);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            var result = await _languageService.DeleteAsync(id);
            if (!result.Success) return NotFound(result);
            return Ok(result);
        }

        [HttpDelete("range")]
        public async Task<IActionResult> DeleteRange([FromBody] IEnumerable<string> ids)
        {
            var result = await _languageService.DeleteRangeAsync(ids);
            if (!result.Success) return NotFound(result);
            return Ok(result);
        }

        [HttpGet("exists/{id}")]
        public async Task<IActionResult> Exists(string id)
        {
            var exists = await _languageService.ExistsAsync(id);
            return Ok(new { Exists = exists });
        }

        [HttpGet("count")]
        public async Task<IActionResult> Count()
        {
            var count = await _languageService.CountAsync();
            return Ok(new { Count = count });
        }
    }
}