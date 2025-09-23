using Core.Interfaces.Services;
using invoice.Core.DTO.Product;
using invoice.Core.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace invoice.Controllers
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


        [HttpPost]
        public async Task<IActionResult> Create([FromForm] ProductCreateDTO dto)
        {
            var response = await _productService.CreateAsync(dto, GetUserId());
            return Ok(response);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(string id, [FromForm] ProductUpdateDTO dto)
        {
            var response = await _productService.UpdateAsync(id, dto, GetUserId());
            return Ok(response);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            var response = await _productService.DeleteAsync(id, GetUserId());
            return Ok(response);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(string id)
        {
            var response = await _productService.GetByIdWithInvoicesAsync(id, GetUserId());
            return Ok(response);
        }

        [AllowAnonymous]
        [HttpGet("storeproduct/{productid}/{userid}")]
        public async Task<IActionResult> GetByIdForStore(string productid, string userid)
        {
            var response = await _productService.GetByIdAsync(productid, userid);
            return Ok(response);
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var response = await _productService.GetAllAsync(GetUserId());
            return Ok(response);
        }


        [HttpGet("category/{categoryId}")]
        public async Task<IActionResult> GetByCategory(string categoryId)
        {
            var response = await _productService.GetByCategoryAsync(categoryId, GetUserId());
            return Ok(response);
        }



        [HttpGet("available/pos")]
        public async Task<IActionResult> GetAvailableForPOS()
        {
            var response = await _productService.GetAvailableForPOSAsync(GetUserId());
            return Ok(response);
        }

        [HttpGet("available/store")]
        public async Task<IActionResult> GetAvailableForStore()
        {
            var response = await _productService.GetAvailableForStoreAsync(GetUserId());
            return Ok(response);
        }

        [AllowAnonymous]
        [HttpGet("storeproducts/{userid}")]
        public async Task<IActionResult> GetStoreProdect(string userid)
        {
            var response = await _productService.GetAvailableForStoreAsync(userid);
            return Ok(response);
        }

        [HttpGet("list")]
        public async Task<IActionResult> GetProductList()
        {
            var response = await _productService.ProductsavailableAsync(GetUserId());
            return Ok(response);
        }




        [HttpGet("store/{storeId}")]
        public async Task<IActionResult> GetByStore(string storeId)
        {
            var response = await _productService.GetByStoreAsync(storeId, GetUserId());
            return Ok(response);
        }
        [HttpPut("{id}/quantity/{quantity}")]
        public async Task<IActionResult> UpdateQuantity(string id, int quantity)
        {
            var response = await _productService.UpdateQuantityAsync(id, quantity, GetUserId());
            return Ok(response);
        }

        [HttpPut("{id}/increment/{amount}")]
        public async Task<IActionResult> IncrementQuantity(string id, int amount)
        {
            var response = await _productService.IncrementQuantityAsync(id, amount, GetUserId());
            return Ok(response);
        }

        [HttpPut("{id}/decrement/{amount}")]
        public async Task<IActionResult> DecrementQuantity(string id, int amount)
        {
            var response = await _productService.DecrementQuantityAsync(id, amount, GetUserId());
            return Ok(response);
        }


        [HttpPost("range")]
        public async Task<IActionResult> AddRange([FromBody] IEnumerable<ProductCreateDTO> dtos)
        {
            var response = await _productService.AddRangeAsync(dtos, GetUserId());
            return Ok(response);
        }

        [HttpPut("range")]
        public async Task<IActionResult> UpdateRange([FromBody] IEnumerable<ProductUpdateDTO> dtos)
        {
            var response = await _productService.UpdateRangeAsync(dtos, GetUserId());
            return Ok(response);
        }

        [HttpDelete("range")]
        public async Task<IActionResult> DeleteRange([FromBody] IEnumerable<string> ids)
        {
            var response = await _productService.DeleteRangeAsync(ids, GetUserId());
            return Ok(response);
        }


        [HttpGet("exists")]
        public async Task<IActionResult> Exists([FromQuery] string name)
        {
            var response = await _productService.ExistsAsync(p => p.Name == name, GetUserId());
            return Ok(response);
        }

        [HttpGet("count")]
        public async Task<IActionResult> Count( )
        {
            var count = await _productService.CountAsync(GetUserId());
            return Ok(new { Count = count });
        }
    }
}