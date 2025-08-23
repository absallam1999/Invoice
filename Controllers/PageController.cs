using invoice.Core.DTO.Page;
using invoice.Core.Entites;
using invoice.Core.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace invoice.Controllers
{
    //[Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class PagesController : ControllerBase
    {
        private readonly IPageService _pageService;

        public PagesController(IPageService pageService)
        {
            _pageService = pageService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] string storeId = null, [FromQuery] string languageId = null)
        {
            var response = await _pageService.GetAllAsync(storeId, languageId);
            return Ok(response);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(string id)
        {
            var response = await _pageService.GetByIdAsync(id);
            return Ok(response);
        }

        [HttpGet("search")]
        public async Task<IActionResult> Search([FromQuery] string keyword, [FromQuery] string storeId = null, [FromQuery] string languageId = null)
        {
            var response = await _pageService.SearchAsync(keyword, storeId, languageId);
            return Ok(response);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] PageCreateDTO dto)
        {
            var entity = new Page
            {
                Title = dto.Title,
                Content = dto.Content,
                Image = dto.Image,
                InFooter = dto.InFooter,
                InHeader = dto.InHeader,
                StoreId = dto.StoreId,
                LanguageId = dto.LanguageId
            };

            var response = await _pageService.CreateAsync(entity);
            return Ok(response);
        }

        [HttpPost("range")]
        public async Task<IActionResult> CreateRange([FromBody] IEnumerable<PageCreateDTO> dtos)
        {
            var entities = dtos.Select(dto => new Page
            {
                Title = dto.Title,
                Content = dto.Content,
                Image = dto.Image,
                InFooter = dto.InFooter,
                InHeader = dto.InHeader,
                StoreId = dto.StoreId,
                LanguageId = dto.LanguageId
            });

            var response = await _pageService.CreateRangeAsync(entities);
            return Ok(response);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(string id, [FromBody] PageUpdateDTO dto)
        {
            if (id != dto.Id)
                return BadRequest(new { Success = false, Message = "ID mismatch" });

            var entity = new Page
            {
                Id = dto.Id,
                Title = dto.Title,
                Content = dto.Content,
                Image = dto.Image,
                InFooter = dto.InFooter,
                InHeader = dto.InHeader,
                StoreId = dto.StoreId,
                LanguageId = dto.LanguageId
            };

            var response = await _pageService.UpdateAsync(id, entity);
            return Ok(response);
        }

        [HttpPut("range")]
        public async Task<IActionResult> UpdateRange([FromBody] IEnumerable<PageUpdateDTO> dtos)
        {
            var entities = dtos.Select(dto => new Page
            {
                Id = dto.Id,
                Title = dto.Title,
                Content = dto.Content,
                Image = dto.Image,
                InFooter = dto.InFooter,
                InHeader = dto.InHeader,
                StoreId = dto.StoreId,
                LanguageId = dto.LanguageId
            });

            var response = await _pageService.UpdateRangeAsync(entities);
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
