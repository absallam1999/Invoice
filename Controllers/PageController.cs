//using Microsoft.AspNetCore.Mvc;
//using invoice.Models;
//using invoice.Data;
//using invoice.DTO.Page;
//using Microsoft.AspNetCore.Authorization;
//using invoice.DTO;

//namespace invoice.Controllers
//{
//    [Route("api/[controller]")]
//    [ApiController]
//    [Authorize]
//    public class PageController : ControllerBase
//    {
//        private readonly IRepository<Page> _repository;

//        public PageController(IRepository<Page> repository)
//        {
//            _repository = repository;
//        }

//        [HttpGet]
//        public async Task<IActionResult> GetAll()
//        {
//            var pages = await _repository.GetAll(p => p.Store, p => p.Language);
//            var result = pages.Select(p => new PageDetailsDTO
//            {
//                Id = p.Id,
//                Title = p.Title,
//                Content = p.Content,
//                Image = p.Image,
//                Infooter = p.Infooter,
//                Inheader = p.Inheader,
//                StoreId = p.StoreId,
//                StoreName = p.Store.Name,
//                LanguageId = p.LanguageId,
//                LanguageName = p.Language.Name
//            });

//            return Ok(new GeneralResponse<IEnumerable<PageDetailsDTO>>
//            {
//                Success = true,
//                Message = "Pages retrieved successfully.",
//                Data = result
//            });
//        }

//        [HttpGet("{id}")]
//        public async Task<IActionResult> GetById(string id)
//        {
//            var page = await _repository.GetById(id, p => p.Store, p => p.Language);
//            if (page == null)
//            {
//                return NotFound(new GeneralResponse<object>
//                {
//                    Success = false,
//                    Message = $"Page with ID {id} not found.",
//                    Data = null
//                });
//            }

//            var dto = new PageDetailsDTO
//            {
//                Id = page.Id,
//                Title = page.Title,
//                Content = page.Content,
//                Image = page.Image,
//                Infooter = page.Infooter,
//                Inheader = page.Inheader,
//                StoreId = page.StoreId,
//                StoreName = page.Store?.Name,
//                LanguageId = page.LanguageId,
//                LanguageName = page.Language?.Name
//            };

//            return Ok(new GeneralResponse<PageDetailsDTO>
//            {
//                Success = true,
//                Message = "Page retrieved successfully.",
//                Data = dto
//            });
//        }

//        [HttpPost]
//        public async Task<IActionResult> Create([FromBody] CreatePageDTO dto)
//        {
//            if (!ModelState.IsValid)
//            {
//                return BadRequest(new GeneralResponse<object>
//                {
//                    Success = false,
//                    Message = "Validation failed.",
//                    Data = ModelState
//                });
//            }

//            var page = new Page
//            {
//                Title = dto.Title,
//                Content = dto.Content,
//                Image = dto.Image,
//                Infooter = dto.Infooter,
//                Inheader = dto.Inheader,
//                StoreId = dto.StoreId,
//                LanguageId = dto.LanguageId
//            };

//            await _repository.Add(page);

//            return Ok(new GeneralResponse<Page>
//            {
//                Success = true,
//                Message = "Page created successfully.",
//                Data = page
//            });
//        }

//        [HttpPut("{id}")]
//        public async Task<IActionResult> Update(string id, [FromBody] UpdatePageDTO dto)
//        {
//            if (id != dto.Id)
//            {
//                return BadRequest(new GeneralResponse<object>
//                {
//                    Success = false,
//                    Message = "ID mismatch.",
//                    Data = null
//                });
//            }

//            var page = await _repository.GetById(id);
//            if (page == null)
//            {
//                return NotFound(new GeneralResponse<object>
//                {
//                    Success = false,
//                    Message = $"Page with ID {id} not found.",
//                    Data = null
//                });
//            }

//            page.Title = dto.Title;
//            page.Content = dto.Content;
//            page.Image = dto.Image;
//            page.Infooter = dto.Infooter;
//            page.Inheader = dto.Inheader;
//            page.StoreId = dto.StoreId;
//            page.LanguageId = dto.LanguageId;

//            await _repository.Update(page);

//            return Ok(new GeneralResponse<Page>
//            {
//                Success = true,
//                Message = "Page updated successfully.",
//                Data = page
//            });
//        }

//        [HttpDelete("{id}")]
//        public async Task<IActionResult> Delete(string id)
//        {
//            var page = await _repository.GetById(id);
//            if (page == null)
//            {
//                return NotFound(new GeneralResponse<object>
//                {
//                    Success = false,
//                    Message = $"Page with ID {id} not found.",
//                    Data = null
//                });
//            }

//            await _repository.Delete(id);

//            return Ok(new GeneralResponse<object>
//            {
//                Success = true,
//                Message = "Page deleted successfully.",
//                Data = null
//            });
//        }
//    }
//}