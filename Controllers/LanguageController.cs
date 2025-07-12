using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;
using invoice.Models;
using invoice.Data;
using invoice.DTO;
using Microsoft.AspNetCore.Authorization;
using invoice.Models.Enums;

namespace invoice.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class LanguageController : ControllerBase
    {
        private readonly IRepository<Language> _repository;

        public LanguageController(IRepository<Language> repository)
        {
            _repository = repository;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var languages = await _repository.GetAll();
            return Ok(new GeneralResponse<IEnumerable<Language>>
            {
                Success = true,
                Message = "Languages retrieved successfully.",
                Data = languages
            });
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(string id)
        {
            var language = await _repository.GetById(id);
            if (language == null)
            {
                return NotFound(new GeneralResponse<object>
                {
                    Success = false,
                    Message = $"Language with ID {id} not found.",
                    Data = null
                });
            }

            return Ok(new GeneralResponse<Language>
            {
                Success = true,
                Message = "Language retrieved successfully.",
                Data = language
            });
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] Language language)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new GeneralResponse<object>
                {
                    Success = false,
                    Message = "Validation failed.",
                    Data = ModelState
                });
            }

            await _repository.Add(language);

            return Ok(new GeneralResponse<Language>
            {
                Success = true,
                Message = "Language created successfully.",
                Data = language
            });
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(string id, [FromBody] Language updated)
        {
            if (id != updated.Id)
            {
                return BadRequest(new GeneralResponse<object>
                {
                    Success = false,
                    Message = "ID mismatch.",
                    Data = null
                });
            }

            var existing = await _repository.GetById(id);
            if (existing == null)
            {
                return NotFound(new GeneralResponse<object>
                {
                    Success = false,
                    Message = $"Language with ID {id} not found.",
                    Data = null
                });
            }

            existing.Name = updated.Name;
            await _repository.Update(existing);

            return Ok(new GeneralResponse<Language>
            {
                Success = true,
                Message = "Language updated successfully.",
                Data = existing
            });
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            var language = await _repository.GetById(id);
            if (language == null)
            {
                return NotFound(new GeneralResponse<object>
                {
                    Success = false,
                    Message = $"Language with ID {id} not found.",
                    Data = null
                });
            }

            await _repository.Delete(id);

            return Ok(new GeneralResponse<object>
            {
                Success = true,
                Message = "Language deleted successfully.",
                Data = null
            });
        }

        [HttpGet("Target-page")]
        public async Task<IActionResult> GetLanguagesForPages()
        {
            var all = await _repository.GetAll();
            var filtered = all.Where(l => l.Target == LanguageTarget.Page);
            return Ok(filtered);
        }

        [HttpGet("Target-invoice")]
        public async Task<IActionResult> GetLanguagesForInvoices()
        {
            var all = await _repository.GetAll();
            var filtered = all.Where(l => l.Target == LanguageTarget.Invoice);
            return Ok(filtered);
        }
    }
}
