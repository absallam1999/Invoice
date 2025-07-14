using Microsoft.AspNetCore.Mvc;
using invoice.Models;
using invoice.Data;
using invoice.DTO;
using invoice.DTO.Language;
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
            var dtoList = languages.Select(l => new LanguageDetailsDTO
            {
                Id = l.Id,
                Name = l.Name,
                Target = l.Target
            });

            return Ok(new GeneralResponse<IEnumerable<LanguageDetailsDTO>>
            {
                Success = true,
                Message = "Languages retrieved successfully.",
                Data = dtoList
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

            var dto = new LanguageDetailsDTO
            {
                Id = language.Id,
                Name = language.Name,
                Target = language.Target
            };

            return Ok(new GeneralResponse<LanguageDetailsDTO>
            {
                Success = true,
                Message = "Language retrieved successfully.",
                Data = dto
            });
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateLanguageDTO dto)
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

            var language = new Language
            {
                Name = dto.Name,
                Target = dto.Target
            };

            await _repository.Add(language);

            return Ok(new GeneralResponse<LanguageDetailsDTO>
            {
                Success = true,
                Message = "Language created successfully.",
                Data = new LanguageDetailsDTO
                {
                    Id = language.Id,
                    Name = language.Name,
                    Target = language.Target
                }
            });
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(string id, [FromBody] UpdateLanguageDTO dto)
        {
            if (id != dto.Id)
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

            existing.Name = dto.Name;
            existing.Target = dto.Target;

            await _repository.Update(existing);

            return Ok(new GeneralResponse<LanguageDetailsDTO>
            {
                Success = true,
                Message = "Language updated successfully.",
                Data = new LanguageDetailsDTO
                {
                    Id = existing.Id,
                    Name = existing.Name,
                    Target = existing.Target
                }
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

        [HttpGet("target-page")]
        public async Task<IActionResult> GetLanguagesForPages()
        {
            var languages = await _repository.GetAll();
            var filtered = languages
                .Where(l => l.Target == LanguageTarget.Page)
                .Select(l => new LanguageDetailsDTO
                {
                    Id = l.Id,
                    Name = l.Name,
                    Target = l.Target
                });

            return Ok(new GeneralResponse<IEnumerable<LanguageDetailsDTO>>
            {
                Success = true,
                Message = "Page languages retrieved successfully.",
                Data = filtered
            });
        }

        [HttpGet("target-invoice")]
        public async Task<IActionResult> GetLanguagesForInvoices()
        {
            var languages = await _repository.GetAll();
            var filtered = languages
                .Where(l => l.Target == LanguageTarget.Invoice)
                .Select(l => new LanguageDetailsDTO
                {
                    Id = l.Id,
                    Name = l.Name,
                    Target = l.Target
                });

            return Ok(new GeneralResponse<IEnumerable<LanguageDetailsDTO>>
            {
                Success = true,
                Message = "Invoice languages retrieved successfully.",
                Data = filtered
            });
        }
    }
}
