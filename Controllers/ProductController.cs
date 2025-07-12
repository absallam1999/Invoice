using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using invoice.Data;
using invoice.Models;
using invoice.DTO;
using invoice.DTO.Product;

namespace invoice.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class ProductController : ControllerBase
    {
        private readonly IRepository<Product> _repository;

        public ProductController(IRepository<Product> repository)
        {
            _repository = repository;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var products = await _repository.GetAll();
            var result = products.Select(p => new ProductDetailsDTO
            {
                Id = p.Id,
                Name = p.Name,
                Image = p.Image,
                Price = p.Price,
                Quantity = p.Quantity,
                CategoryId = p.CategoryId,
                CategoryName = p.Category?.Name,
                StoreId = p.StoreId,
                StoreName = p.Store?.Name
            });
            return Ok(new GeneralResponse<object>
            {
                Success = true,
                Message = "Products retrieved successfully.",
                Data = products
            });
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(string id)
        {
            var product = await _repository.GetById(id);
            if (product == null)
                return NotFound(new GeneralResponse<object>
                {
                    Success = false,
                    Message = "Product not found.",
                    Data = null
                });

            return Ok(new GeneralResponse<object>
            {
                Success = true,
                Message = "Product retrieved successfully.",
                Data = product
            });
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateProductDTO dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(new GeneralResponse<object>
                {
                    Success = false,
                    Message = "Validation failed.",
                    Data = ModelState
                });

            var product = new Product
            {
                Name = dto.Name,
                Image = dto.Image,
                Price = dto.Price,
                Quantity = dto.Quantity,
                CategoryId = dto.CategoryId,
                StoreId = dto.StoreId
            };

            await _repository.Add(product);

            return Ok(new GeneralResponse<object>
            {
                Success = true,
                Message = "Product created successfully.",
                Data = product
            });
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(string id, [FromBody] UpdateProductDTO dto)
        {
            if (id != dto.Id)
                return BadRequest(new GeneralResponse<object>
                {
                    Success = false,
                    Message = "ID mismatch.",
                    Data = null
                });

            var product = await _repository.GetById(id);
            if (product == null)
                return NotFound(new GeneralResponse<object>
                {
                    Success = false,
                    Message = "Product not found.",
                    Data = null
                });

            product.Name = dto.Name;
            product.Image = dto.Image;
            product.Price = dto.Price;
            product.Quantity = dto.Quantity;
            product.CategoryId = dto.CategoryId;
            product.StoreId = dto.StoreId;

            await _repository.Update(product);

            return Ok(new GeneralResponse<object>
            {
                Success = true,
                Message = "Product updated successfully.",
                Data = product
            });
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            var product = await _repository.GetById(id);
            if (product == null)
                return NotFound(new GeneralResponse<object>
                {
                    Success = false,
                    Message = "Product not found.",
                    Data = null
                });

            await _repository.Delete(id);

            return Ok(new GeneralResponse<object>
            {
                Success = true,
                Message = "Product deleted successfully.",
                Data = null
            });
        }
    }
}