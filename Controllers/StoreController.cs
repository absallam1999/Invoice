using Microsoft.AspNetCore.Mvc;
using invoice.Models;
using invoice.Data;
using invoice.DTO.Store;
using invoice.DTO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using invoice.DTO.Product;
using System.Security.Claims;
using System.Data;

namespace invoice.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class StoreController : ControllerBase
    {
        private readonly IRepository<Store> _storerepository;
        private readonly IRepository<Product> _productrepository;

        public StoreController(IRepository<Store> storerepository , IRepository<Product> productrepository)
        {
            _storerepository = storerepository;
            _productrepository= productrepository;
        }
       
        [HttpGet]
        public async Task<IActionResult> Get()
        {  
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
                return Unauthorized();
            
            var store = await _storerepository.GetByUserId( userId, q => q
          .Include(p => p.ContactInfo)
          .Include(p => p.PurchaseCompletionOptions)
          .Include(p => p.Shipping)
          .Include(p => p.Pages)
          
          );

            if (store == null)
                return NotFound(new GeneralResponse<object>
                {
                    Success = false,
                    Message = "You don't have a store yet",
                    Data = null
                });
            var products = await _productrepository.Query(
                p => (p.Quantity == null || p.Quantity > 0) && p.InStore == true
                                && p.UserId == userId);


            var StoreDTO = new StoreDetailsDTO
            {
                StoreName = store.Name,
                StoreId = store.Id,
                Description = store.Description,
                Url = store.Url,
                Logo = store.Logo,
                CoverImage = store.CoverImage,
                Color = store.Color,
                IsActivated = store.IsActivated,
                StoreEmail = store.ContactInfo.Email,
                StorePhone = store.ContactInfo.Phone,
                Storelocation = store.ContactInfo.location,
                StoreFacebook = store.ContactInfo.Facebook,
                StoreInstagram = store.ContactInfo.Instagram,
                StoreWhatsApp = store.ContactInfo.WhatsApp,
                Currency = store.Currency,
                Arabic = store.Arabic,
                English = store.English,
                Cash = store.Cash,
                BankTransfer = store.BankTransfer,
                PayPal = store.PayPal,
                Tax = store.Tax,
                FromStore = store.Shipping.FromStore,
                Delivery = store.Shipping.Delivery,
                //cost
                ClientName = store.PurchaseCompletionOptions.Name,
                ClientEmail = store.PurchaseCompletionOptions.Email,
                Clientphone = store.PurchaseCompletionOptions.phone,
                TermsAndConditions = store.PurchaseCompletionOptions.TermsAndConditions,
                Products = products.Select(p => new ProductStore
                {
                    ProductId = p.Id,
                    ProductName = p.Name,
                    ProductImage = p.Image
                } ).ToList(),
                //category

            };

            return Ok(new GeneralResponse<StoreDetailsDTO>
            {
                Success = true,
                Message = "Store retrieved successfully.",
                Data = StoreDTO

            });
            }

        [HttpPost]
        public async Task<IActionResult> Add([FromBody] CreateStoreDTO dto)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
                return Unauthorized();

            var storeDB = await _storerepository.GetByUserId(userId);

            if (storeDB != null)
                return NotFound(new GeneralResponse<object>
                {
                    Success = false,
                    Message = "You have store already.",
                    Data = storeDB.Id
                });



            if (!ModelState.IsValid)
            {
                return BadRequest(new GeneralResponse<object>
                {
                    Success = false,
                    Message = "Invalid data submitted.",
                    Data = ModelState
                });
            }

            var store = new Store
            {
                Name = dto.Name,
                Description = dto.Description,
                Url = dto.Url,
                Logo = dto.Logo,
                CoverImage = dto.CoverImage,
                Color = dto.Color,
                Currency = dto.Currency,
                IsActivated = dto.IsActivated,
                UserId = dto.UserId
            };

            await _storerepository.Add(store);
            return Ok(new GeneralResponse<Store>
            {
                Success = true,
                Message = "Store created successfully.",
                Data = store
            });
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(string id, [FromBody] UpdateStoreDTO dto)
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

            if (!ModelState.IsValid)
            {
                return BadRequest(new GeneralResponse<object>
                {
                    Success = false,
                    Message = "Invalid data submitted.",
                    Data = ModelState
                });
            }

            var existingStore = await _storerepository.GetById(id);
            if (existingStore == null)
            {
                return NotFound(new GeneralResponse<object>
                {
                    Success = false,
                    Message = $"No store found with ID {id}.",
                    Data = null
                });
            }

            existingStore.Name = dto.Name;
            existingStore.Description = dto.Description;
            existingStore.Url = dto.Url;
            existingStore.Logo = dto.Logo;
            existingStore.CoverImage = dto.CoverImage;
            existingStore.Color = dto.Color;
            existingStore.Currency = dto.Currency;
            existingStore.IsActivated = dto.IsActivated;
            existingStore.UserId = dto.UserId;

            await _storerepository.Update(existingStore);

            return Ok(new GeneralResponse<Store>
            {
                Success = true,
                Message = "Store updated successfully.",
                Data = existingStore
            });
        }
        

        [HttpPut("activation")]
        public async Task<IActionResult> Activation()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
                return Unauthorized();

            var store = await _storerepository.GetByUserId(userId);
             
            if (store == null)
                return NotFound(new GeneralResponse<object>
                {
                    Success = false,
                    Message = "Store not found.",
                    Data = null
                });

            store.IsActivated = !store.IsActivated;

            var result = await _storerepository.Update(store);

            if (!result.Success)
            {
                return StatusCode(500, new GeneralResponse<object>
                {
                    Success = false,
                    Message = result.Message,
                    Data = null
                });
            }

            var updated = store.IsActivated ? "active" : "unactive";
           
            return Ok(new GeneralResponse<object>
            {
                Success = true,
                Message = $"Store is now {updated}.",
                Data = new { store.Id, store.IsActivated }
            });

        }
    }
}