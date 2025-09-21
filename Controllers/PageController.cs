using invoice.Core.DTO.Page;
using invoice.Core.Entities;
using invoice.Core.Interfaces.Services;
using invoice.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace invoice.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class PagesController : ControllerBase
    {
        private readonly IPageService _pageService;
        private readonly IStoreService _storeService;

        public PagesController(IPageService pageService,IStoreService storeService)
        {
            _pageService = pageService;
            _storeService = storeService;
        }
        private string GetUserId() =>
        User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? string.Empty;

        private async Task<string> GetStoreIdAsync()
        {
            var store = await _storeService.GetByUserAsync(GetUserId());
            return store.Data.Id;
        }


        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var response = await _pageService.GetAllAsync(GetUserId());
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
        public async Task<IActionResult> Create([FromForm] PageCreateDTO dto )
        {
            var storeId = await GetStoreIdAsync();

            var response = await _pageService.CreateAsync(dto, storeId);
            return Ok(response);
        }

        [HttpPost("range")]
        public async Task<IActionResult> CreateRange([FromBody] IEnumerable<Page> dtos)
        {
            var entities = dtos.Select(dto => new Page
            {
                Title = dto.Title,
                Content = dto.Content,
                Image = dto.Image,
                InFooter = dto.InFooter,
                InHeader = dto.InHeader,
            
            });
            var Store = await _storeService.GetByUserAsync(GetUserId());
            var storeId = Store.Data.Id;

            var response = await _pageService.CreateRangeAsync(entities);
            return Ok(response);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(string id, [FromForm] PageUpdateDTO dto)
        {
            var response = await _pageService.UpdateAsync(id, dto);
            return Ok(response);
        }

        [HttpPut("range")]
        public async Task<IActionResult> UpdateRange([FromForm] IEnumerable<PageUpdateDTO> dtos)
        {
            var entities = dtos.Select(dto => new Page
            {
                
                Title = dto.Title,
                Content = dto.Content,
               // Image = dto.Image,
                InFooter = dto.InFooter,
                InHeader = dto.InHeader,
               
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
