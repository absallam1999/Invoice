using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using invoice.Data;
using invoice.Models;
using invoice.DTO.Product;
using invoice.DTO;

//namespace invoice.Controllers
//{
//    [Route("api/[controller]")]
//    [ApiController]
//    [Authorize]
//    public class CategoryController : ControllerBase
//    {
//        private readonly IRepository<Category> _repository;

//        public CategoryController(IRepository<Category> repository)
//        {
//            _repository = repository;
//        }

//        [HttpGet]
//        public async Task<IActionResult> GetAll()
//        {
//            var categories = await _repository.GetAll(c => c.Products);

//            var result = categories.Select(c => new CategoryDetailsDTO
//            {
//                Id = c.Id,
//                Name = c.Name,
//                Products = c.Products.Select(p => new ProductDetailsDTO
//                {
//                    Id = p.Id,
//                    Name = p.Name,
//                    Image = p.Image,
//                    Price = p.Price,
//                    Quantity = p.Quantity,
//                    StoreId = p.StoreId,
//                    CategoryId = p.CategoryId
//                }).ToList() ?? new List<ProductDetailsDTO>()
//            });

//            return Ok(new GeneralResponse<IEnumerable<CategoryDetailsDTO>>
//            {
//                Success = true,
//                Message = "Categories retrieved successfully.",
//                Data = result
//            });
//        }

//        [HttpGet("{id}")]
//        public async Task<IActionResult> GetById(string id)
//        {
//            var category = await _repository.GetById(id, c => c.Products);
//            if (category == null)
//            {
//                return NotFound(new GeneralResponse<object>
//                {
//                    Success = false,
//                    Message = $"Category with ID {id} not found.",
//                    Data = null
//                });
//            }

//            var result = new CategoryDetailsDTO
//            {
//                Id = category.Id,
//                Name = category.Name,
//                Products = category.Products.Select(p => new ProductDetailsDTO
//                {
//                    Id = p.Id,
//                    Name = p.Name,
//                    Image = p.Image,
//                    Price = p.Price,
//                    Quantity = p.Quantity,
//                    StoreId = p.StoreId,
//                    CategoryId = p.CategoryId
//                }).ToList() ?? new List<ProductDetailsDTO>()
//            };

//            return Ok(new GeneralResponse<CategoryDetailsDTO>
//            {
//                Success = true,
//                Message = "Category retrieved successfully.",
//                Data = result
//            });
//        }

//        [HttpPost]
//        public async Task<IActionResult> Create([FromBody] CreateCategoryDTO dto)
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

//            var category = new Category
//            {
//                Name = dto.Name
//            };

//            await _repository.Add(category);

//            var result = new CategoryDetailsDTO
//            {
//                Id = category.Id,
//                Name = category.Name,
//                Products = new List<ProductDetailsDTO>()
//            };

//            return Ok(new GeneralResponse<CategoryDetailsDTO>
//            {
//                Success = true,
//                Message = "Category created successfully.",
//                Data = result
//            });
//        }

//        [HttpPut("{id}")]
//        public async Task<IActionResult> Update(string id, [FromBody] UpdateCategoryDTO dto)
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

//            if (!ModelState.IsValid)
//            {
//                return BadRequest(new GeneralResponse<object>
//                {
//                    Success = false,
//                    Message = "Validation failed.",
//                    Data = ModelState
//                });
//            }

//            var existing = await _repository.GetById(id, c => c.Products);
//            if (existing == null)
//            {
//                return NotFound(new GeneralResponse<object>
//                {
//                    Success = false,
//                    Message = $"Category with ID {id} not found.",
//                    Data = null
//                });
//            }

//            existing.Name = dto.Name;
//            await _repository.Update(existing);

//            var result = new CategoryDetailsDTO
//            {
//                Id = existing.Id,
//                Name = existing.Name,
//                Products = existing.Products.Select(p => new ProductDetailsDTO
//                {
//                    Id = p.Id,
//                    Name = p.Name,
//                    Image = p.Image,
//                    Price = p.Price,
//                    Quantity = p.Quantity,
//                    StoreId = p.StoreId,
//                    CategoryId = p.CategoryId
//                }).ToList() ?? new List<ProductDetailsDTO>()
//            };

//            return Ok(new GeneralResponse<CategoryDetailsDTO>
//            {
//                Success = true,
//                Message = "Category updated successfully.",
//                Data = result
//            });
//        }

//        [HttpDelete("{id}")]
//        public async Task<IActionResult> Delete(string id)
//        {
//            var category = await _repository.GetById(id);
//            if (category == null)
//            {
//                return NotFound(new GeneralResponse<object>
//                {
//                    Success = false,
//                    Message = $"Category with ID {id} not found.",
//                    Data = null
//                });
//            }

//            await _repository.Delete(id);

//            return Ok(new GeneralResponse<object>
//            {
//                Success = true,
//                Message = "Category deleted successfully.",
//                Data = null
//            });
//        }
//    }
//}
