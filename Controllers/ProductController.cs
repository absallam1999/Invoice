using invoice.Core.DTO.Product;
using invoice.Core.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace invoice.API.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class ProductController : ControllerBase
    {
        private readonly IProductService _productService;

        public ProductController(IProductService productService)
        {
            _productService = productService;
        }

        private string GetUserId() =>
            User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? string.Empty;

        [HttpPost("create")]
        public async Task<IActionResult> Create([FromForm] ProductCreateRequest request)
        {
            var response = await _productService.CreateAsync(request, GetUserId());
            return response.Success ? Ok(response) : BadRequest(response);
        }

        [HttpPut("update/{id}")]
        public async Task<IActionResult> Update(string id, [FromForm] ProductUpdateRequest request)
        {
            var response = await _productService.UpdateAsync(id, request, GetUserId());
            return response.Success ? Ok(response) : BadRequest(response);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            var response = await _productService.DeleteAsync(id, GetUserId());
            return response.Success ? Ok(response) : BadRequest(response);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(string id)
        {
            var response = await _productService.GetByIdAsync(id, GetUserId());
            return response.Success ? Ok(response) : BadRequest(response);
        }

        [HttpGet("all")]
        public async Task<IActionResult> GetAll()
        {
            var response = await _productService.GetAllAsync(GetUserId());
            return response.Success ? Ok(response) : BadRequest(response);
        }

        [HttpGet("StoreAvailable")]
        public async Task<IActionResult> StoreAvailable()
        {
            var response = await _productService.GetAvailableForStoreAsync(GetUserId());
            return response.Success ? Ok(response) : BadRequest(response);
        }

        [HttpGet("POSAvailable")]
        public async Task<IActionResult> POSAvailable()
        {
            var response = await _productService.GetAvailableForPOSAsync(GetUserId());
            return response.Success ? Ok(response) : BadRequest(response);
        }

        [HttpGet("InProductList")]
        public async Task<IActionResult> GetProductList()
        {
            var response = await _productService.GetProductListAsync(GetUserId());
            return response.Success ? Ok(response) : BadRequest(response);
        }

        [HttpGet("category/{categoryId}")]
        public async Task<IActionResult> GetByCategory(string categoryId)
        {
            var response = await _productService.GetByCategoryAsync(categoryId, GetUserId());
            return response.Success ? Ok(response) : BadRequest(response);
        }

        [HttpPut("{id}/images")]
        public async Task<IActionResult> UpdateImages(string id, [FromForm] ProductImagesDTO request)
        {
            var response = await _productService.UpdateImageAsync(id, request, GetUserId());
            return response.Success ? Ok(response) : BadRequest(response);
        }

        [HttpPut("{id}/quantity/{quantity}")]
        public async Task<IActionResult> UpdateQuantity(string id, int quantity)
        {
            var response = await _productService.UpdateQuantityAsync(id, quantity, GetUserId());
            return response.Success ? Ok(response) : BadRequest(response);
        }

        [HttpPut("{id}/increment/{amount}")]
        public async Task<IActionResult> IncrementQuantity(string id, int amount)
        {
            var response = await _productService.IncrementQuantityAsync(id, amount, GetUserId());
            return response.Success ? Ok(response) : BadRequest(response);
        }

        [HttpPut("{id}/decrement/{amount}")]
        public async Task<IActionResult> DecrementQuantity(string id, int amount)
        {
            var response = await _productService.DecrementQuantityAsync(id, amount, GetUserId());
            return response.Success ? Ok(response) : BadRequest(response);
        }

        [HttpPost("range")]
        public async Task<IActionResult> AddRange([FromForm] ProductCreateRangeDTO request)
        {
            var response = await _productService.AddRangeAsync(request, GetUserId());
            return response.Success ? Ok(response) : BadRequest(response);
        }

        [HttpPut("range")]
        public async Task<IActionResult> UpdateRange([FromForm] ProductUpdateRangeDTO request)
        {
            var response = await _productService.UpdateRangeAsync(request, GetUserId());
            return response.Success ? Ok(response) : BadRequest(response);
        }

        [HttpDelete("range")]
        public async Task<IActionResult> DeleteRange([FromBody] IEnumerable<string> ids)
        {
            var response = await _productService.DeleteRangeAsync(ids, GetUserId());
            return response.Success ? Ok(response) : BadRequest(response);
        }

        [HttpGet("exists")]
        public async Task<IActionResult> Exists([FromQuery] string name)
        {
            var response = await _productService.ExistsAsync(p => p.Name == name, GetUserId());
            return Ok(new { Success = true, Exists = response });
        }

        [HttpGet("count")]
        public async Task<IActionResult> Count([FromQuery] string categoryId)
        {
            var response = await _productService.CountAsync(p => p.CategoryId == categoryId, GetUserId());
            return Ok(new { Success = true, Count = response });
        }
    }
}
