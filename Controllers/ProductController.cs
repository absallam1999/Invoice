using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using invoice.Data;
using invoice.Models;
using invoice.DTO;
using invoice.DTO.Product;
using System.Security.Claims;


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
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
                return Unauthorized();

            var product = await _repository.GetAll(userId);
            var result = product.Select(p => new GetAllProductDTO
            {
                    ProductId=p.Id,
                    Image=p.Image,
                    Price=p.Price,
                    Quantity=p.Quantity
            });
            //if (result == null)
            //    return NotFound(new GeneralResponse<object>
            //    {
            //        Success = false,
            //        Message = "Product not found.",
            //        Data = null
            //    });
            return Ok(new GeneralResponse<IEnumerable<ProductDetailsDTO>>
            {
                Success = true,
                Message = "Products retrieved successfully.",
                Data = result
            });
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(string id)
        {

            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
                return Unauthorized();

            var product = await _repository.GetById(id, userId, c => c.InvoiceItems);
               
            if (product == null)
                return NotFound(new GeneralResponse<object>
                {
                    Success = false,
                    Message = "Product not found.",
                    Data = null
                });

            var productDTO = new ProductDetailsDTO
            {
                ProductId = product.Id,
                Name = product.Name,
                Image = product.Image,
                Price = product.Price,
                Quantity = product.Quantity,
               // NumberOfSales= product.InvoiceItems?.Count ?? 0,
                //CategoryId = product.CategoryId,
                //CategoryName = product.Category?.Name,
                //StoreId = product.StoreId,
                //StoreName = product.Store?.Name
            };

            return Ok(new GeneralResponse<ProductDetailsDTO>
            {
                Success = true,
                Message = "Product retrieved successfully.",
                Data = productDTO
            });
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] ProductInfoDTO dto)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
                return Unauthorized();

            if (!ModelState.IsValid)
            {
                return BadRequest(new GeneralResponse<object>
                {
                    Success = false,
                    Message = "Validation failed.",
                    Data = ModelState
                });
            }

            string? imageFromReq = await GetImageNameFn(dto.Image);

            var product = new Product
            {
                Name = dto.Name,
                Image = imageFromReq,
                Price = dto.Price,
                Quantity = dto.Quantity,
                CategoryId = dto.CategoryId,
                InPOS = dto.InPOS,
                InStoe = dto.InStoe,
                UserId = userId,
            };

            var result = await _repository.Add(product);

            if (!result.Success)
            {
                return StatusCode(500, new GeneralResponse<object>
                {
                    Success = false,
                    Message = result.Message ?? "Failed to create product.",
                    Data = null
                });
            }

            return Ok(new GeneralResponse<Product>
            {
                Success = true,
                Message = "Product created successfully.",
                Data = product
            });
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(string id, [FromBody] ProductInfoDTO dto)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
                return Unauthorized();

            var product = await _repository.GetById(id, userId);
            if (product == null)
                return NotFound(new GeneralResponse<object>
                {
                    Success = false,
                    Message = "Product not found.",
                    Data = null
                });

            if (!ModelState.IsValid)
                return BadRequest(new GeneralResponse<object>
                {
                    Success = false,
                    Message = "Validation failed.",
                    Data = ModelState
                });
            string? imageFromReq = await GetImageNameFn(dto.Image);

             product.Name = dto.Name;
            product.Image = imageFromReq;
            product.Price = dto.Price;
            product.Quantity = dto.Quantity;
            product.CategoryId = dto.CategoryId;
            product.InPOS = dto.InPOS;
            product.InStoe = dto.InStoe;
            product.UserId = userId;


            var result = await _repository.Update(product);

            if (!result.Success)
                return BadRequest(result.Message);

            return Ok(new
            {
                result.Success,
                result.Message,
                result.Data
            });

        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string id)
        {


            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
                return Unauthorized();

            var product = await _repository.GetById(id, userId);
            if (product == null)
                return NotFound(new GeneralResponse<object>
                {
                    Success = false,
                    Message = "Product not found.",
                    Data = null
                });

            var result = await _repository.Delete(id);


            if (!result.Success)
                return NotFound(new { result.Message });

            return Ok(new { result.Message });
           
        }

        #region imageFunc
        public  async Task<string> GetImageNameFn(IFormFile image)

        {
            if (image != null && image.Length > 0)
            {
                var fileName = Guid.NewGuid().ToString() + Path.GetExtension(image.FileName);
                var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/Images", fileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await image.CopyToAsync(stream);
                }
                return fileName;

            }
            return null;
        }
        #endregion
    }
}