using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using invoice.Data;
using invoice.Models;
using invoice.DTO;
using invoice.DTO.Product;
using System.Security.Claims;
using invoice.Models.Interfaces;
using invoice.DTO.Client;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;
using Microsoft.IdentityModel.Tokens;
using Microsoft.EntityFrameworkCore;


namespace invoice.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class ProductController : ControllerBase
    {
        private readonly IRepository<Product> _productrepository;

        public ProductController(IRepository<Product> productrepository)
        {
            _productrepository = productrepository;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
                return Unauthorized();

            var product = await _productrepository.GetAll(userId);
            var result = product.Select(p => new GetAllProductDTO
            {
                ProductId = p.Id,
                Name = p.Name,
                Image = p.Image,
                Price = p.Price,
                Quantity = p.Quantity,
                Category = p.Category?.Name  ?? null
            });
            //if (result == null)
            //    return NotFound(new GeneralResponse<object>
            //    {
            //        Success = false,
            //        Message = "Product not found.",
            //        Data = null
            //    });
            return Ok(new GeneralResponse<IEnumerable<GetAllProductDTO>>
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

            ///var product = await _productrepository.GetById(id, userId, c => c.InvoiceItems);
            var product = await _productrepository.GetById(id, userId, q=> q 
            .Include(p=>p. InvoiceItems));
               
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
                NumberOfSales= product.InvoiceItems?.Count ?? 0,
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
        public async Task<IActionResult> Create([FromForm] ProductInfoDTO dto)
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
                InStore = dto.InStore,
                UserId = userId,
            };

            var result = await _productrepository.Add(product);

            if (!result.Success)
            {
                return StatusCode(500, new GeneralResponse<object>
                {
                    Success = false,
                    Message = result.Message ?? "Failed to create product.",
                    Data = null
                });
            }

            var productDTO = new ProductDetailsDTO
            {
                ProductId = product.Id,
                Name = product.Name,
                Image = product.Image,
                Price = product.Price,
                Quantity = product.Quantity,
                NumberOfSales= product.InvoiceItems?.Count ?? 0,
               // CategoryId = product.CategoryId,
                //CategoryName = product.Category?.Name,
                //StoreId = product.StoreId,
                //StoreName = product.Store?.Name
            };
            return Ok(new GeneralResponse<ProductDetailsDTO>
            {
                Success = true,
                Message = "Product created successfully.",
                Data = productDTO
            });
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(string id, [FromForm] ProductInfoDTO dto)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
                return Unauthorized();

            var product = await _productrepository.GetById(id, userId);
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

            if (!string.IsNullOrEmpty(dto.Name))
                product.Name = dto.Name;

            if (!string.IsNullOrEmpty(imageFromReq))
                product.Image = imageFromReq;
  
                product.Price = dto.Price;
            if (dto.Quantity.HasValue)
                product.Quantity = dto.Quantity.Value;
            if (!string.IsNullOrEmpty(dto.CategoryId))
                product.CategoryId = dto.CategoryId;

            if (dto.InPOS!=null)
                product.InPOS = dto.InPOS;

            if (dto.InStore!=null)
                product.InStore = dto.InStore;

            var result = await _productrepository.Update(product);

            if (!result.Success)
            {
                return StatusCode(500, new GeneralResponse<object>
                {
                    Success = false,
                    Message = result.Message,
                    Data = null
                });
            }

            var updated = new ProductDetailsDTO
            {

                ProductId = product.Id,
                Name = product.Name,
                Image = product.Image,
                Price = product.Price,
                Quantity = product.Quantity,
                NumberOfSales = product.InvoiceItems?.Count ?? 0,
                // CategoryId = product.CategoryId,
                //CategoryName = product.Category?.Name,
                //StoreId = product.StoreId,
                //StoreName = product.Store?.Name
            };
            return Ok(new GeneralResponse<ProductDetailsDTO>
            {
                Success = true,
                Message = " Product updated successfully.",
                Data = updated
            });

        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
                return Unauthorized();

            var product = await _productrepository.GetById(id, userId);
            if (product == null)
                return NotFound(new GeneralResponse<object>
                {
                    Success = false,
                    Message = "Product not found.",
                    Data = null
                });

            var result = await _productrepository.Delete(id);


            if (!result.Success)
                return NotFound(new { result.Message });

            return Ok(new { result.Message });
           
        }

        [HttpGet("ProductsInPOS")]
        public async Task<IActionResult> ProductsInPOS()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
                return Unauthorized();

            var products = await _productrepository.Query(
                p => (p.Quantity == null || p.Quantity > 0) && p.InPOS == true
                                && p.UserId== userId

               // ,p => p.Category 
            );

            var result = products.Select(p => new GetAllProductDTO
            {
                ProductId = p.Id,
                Name = p.Name,
                Image = p.Image,
                Price = p.Price,
                Quantity = p.Quantity,
                Category = p.Category?.Name ?? null
            });
            return Ok(new GeneralResponse<IEnumerable<GetAllProductDTO>>
            {
                Success = true,
                Message = "Available products retrieved successfully.",
                Data = result
            });
        } 

        [HttpGet("ProductsInStor")]
        public async Task<IActionResult> ProductsInStor()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
                return Unauthorized();

            var products = await _productrepository.Query(
                p => (p.Quantity == null || p.Quantity > 0) && p.InStore == true
                                && p.UserId == userId

            // ,p => p.Category 
            );

            var result = products.Select(p => new GetAllProductDTO
            {
                ProductId = p.Id,
                Name = p.Name,
                Image = p.Image,
                Price = p.Price,
                Quantity = p.Quantity,
                Category = p.Category?.Name ?? null
            });
            return Ok(new GeneralResponse<IEnumerable<GetAllProductDTO>>
            {
                Success = true,
                Message = "Available products retrieved successfully.",
                Data = result
            });
        }
        [HttpGet("Productsavailable")]
        public async Task<IActionResult> Productsavailable()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
                return Unauthorized();

            var products = await _productrepository.Query(
                p => (p.Quantity == null || p.Quantity > 0 )&& p.UserId == userId
               // ,p => p.Category 
            );
          var result = products.Select(p => new GetAllProductDTO
            {
                ProductId = p.Id,
                Name = p.Name,
                Image = p.Image,
                Price = p.Price,
                Quantity = p.Quantity,
                Category = p.Category?.Name ?? null
            });
            return Ok(new GeneralResponse<IEnumerable<GetAllProductDTO>>
            {
                Success = true,
                Message = "Available products retrieved successfully.",
                Data = result
            });
        }

        [HttpPost("AddProduct")]
        public async Task<IActionResult> AddProduct([FromForm] AddProductDTO dto)
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

            var product = new Product
            {
                Name = dto.Name,
                Price = dto.Price,
                //InProductList = dto.InProductList ?? true,
                UserId = userId,
            };

            var result = await _productrepository.Add(product);

            if (!result.Success)
            {
                return StatusCode(500, new GeneralResponse<object>
                {
                    Success = false,
                    Message = result.Message ?? "Failed to add product.",
                    Data = null
                });
            }

            return Ok(new GeneralResponse<Product>
            {
                Success = true,
                Message = "Product added successfully.",
                Data = product
            });
        }

        #region imageFunc
        [NonAction]
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