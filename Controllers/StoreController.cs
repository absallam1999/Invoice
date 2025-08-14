using Microsoft.AspNetCore.Mvc;
using invoice.Models;
using invoice.Data;
using invoice.DTO.Store;
using invoice.DTO;
using Microsoft.AspNetCore.Authorization;
using invoice.DTO.Invoice;
using invoice.DTO.Product;
using invoice.DTO.PurchaseCompletionOptions;

namespace invoice.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class StoreController : ControllerBase
    {
        private readonly IRepository<Store> _repository;

        public StoreController(IRepository<Store> repository)
        {
            _repository = repository;
        }
        //  [HttpGet]
        //public async Task<IActionResult> GetAll()
        //{
        //    var stores = await _repository.GetAll(
        //        s => s.Products,
        //        s => s.Invoices,
        //        s => s.PurchaseCompletionOptions,
        //        s => s.ContactInformations
        //    );

        //    var result = stores.Select(store => new
        //    {
        //        Id = store.Id,
        //        Name = store.Name,
        //        Description = store.Description,
        //        Url = store.Url,
        //        Logo = store.Logo,
        //        UserId = store.UserId,
        //        IsActivated = store.IsActivated,

        //        ContactInformations = store.ContactInformations?.Select(ci => new ContactInformationDetailsDTO
        //        {
        //            Id = ci.Id,
        //            Location = ci.location,
        //            Facebook = ci.Facebook,
        //            WhatsApp = ci.WhatsApp,
        //            Instagram = ci.Instagram,
        //            StoreId = ci.StoreId
        //        }).ToList() ?? new List<ContactInformationDetailsDTO>(),

        //        PurchaseCompletionOption = store.PurchaseCompletionOptions == null
        //            ? null
        //            : new PurchaseCompletionOptionsDTO
        //            {
        //                Id = store.PurchaseCompletionOptions.Id,
        //                SendEmail = store.PurchaseCompletionOptions.SendEmail,
        //                StoreId = store.PurchaseCompletionOptions.StoreId
        //            }
        //    });

        //    return Ok(new GeneralResponse<object>
        //    {
        //        Success = true,
        //        Message = "Stores retrieved successfully.",
        //        Data = result
        //    });
        //}

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(string id)
        {
            try
            {
                var store = await _repository.GetById(id,
                    s => s.Products,
                    //s => s.Invoices,
                    s => s.User,
                    s => s.ContactInformations,
                    s => s.PurchaseCompletionOptions
                );

                if (store == null)
                {
                    return NotFound(new GeneralResponse<object>
                    {
                        Success = false,
                        Message = $"Store with ID {id} not found.",
                        Data = null
                    });
                }

                var result = new
                {
                    Id = store.Id,
                    Name = store.Name,
                    UserId = store.UserId,

                    ContactInformations = store.ContactInformations?.Select(ci => new ContactInformationDetailsDTO
                    {
                        Id = ci.Id,
                        Location = ci.location,
                        Facebook = ci.Facebook,
                        WhatsApp = ci.WhatsApp,
                        Instagram = ci.Instagram,
                        StoreId = ci.StoreId
                    }).ToList() ?? new List<ContactInformationDetailsDTO>(),

                    Products = store.Products?.Select(p => new ProductDetailsDTO
                    {
                        //Id = p.Id,
                        //Name = p.Name,
                        //Image = p.Image,
                        //Price = p.Price,
                        //Quantity = p.Quantity,
                        //CategoryId = p.CategoryId,
                        //CategoryName = p.Category?.Name,
                        //StoreId = p.StoreId,
                        //StoreName = p.Store?.Name
                    }).ToList() ?? new List<ProductDetailsDTO>(),

                    Invoices = store.Invoices?.Select(i => new InvoiceDetailsDTO
                    {
                        //Id = i.Id,
                        //Number = i.Number,
                        //CreateAt = i.CreateAt,
                        //TaxNumber = i.TaxNumber,
                        //Value = i.Value,
                        //Description = i.Description,
                        //InvoiceStatus = i.InvoiceStatus,
                        //InvoiceType = i.InvoiceType,
                        //UserId = i.UserId,
                        //StoreId = i.StoreId,
                        //ClientId = i.ClientId,
                        //LanguageId = i.LanguageId
                    }).ToList() ?? new List<InvoiceDetailsDTO>(),

                    PurchaseCompletionOption = store.PurchaseCompletionOptions == null
                    ? null
                    : new PurchaseCompletionOptionsDTO
                    {
                        Id = store.PurchaseCompletionOptions.Id,
                        SendEmail = store.PurchaseCompletionOptions.SendEmail,
                        StoreId = store.PurchaseCompletionOptions.StoreId
                    }
                };

                return Ok(new GeneralResponse<object>
                {
                    Success = true,
                    Message = "Store retrieved successfully.",
                    Data = result
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new GeneralResponse<object>
                {
                    Success = false,
                    Message = $"Internal server error: {ex.Message}",
                    Data = null
                });
            }
        }

        [HttpPost]
        public async Task<IActionResult> Add([FromBody] CreateStoreDTO dto)
        {
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

            await _repository.Add(store);
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

            var existingStore = await _repository.GetById(id);
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

            await _repository.Update(existingStore);

            return Ok(new GeneralResponse<Store>
            {
                Success = true,
                Message = "Store updated successfully.",
                Data = existingStore
            });
        }
         //+unactive

        //[HttpDelete("{id}")]
        //public async Task<IActionResult> Delete(string id)
        //{
        //    var store = await _repository.GetById(id);
        //    if (store == null)
        //    {
        //        return NotFound(new GeneralResponse<object>
        //        {
        //            Success = false,
        //            Message = $"Store with ID {id} not found.",
        //            Data = null
        //        });
        //    }

        //    await _repository.Delete(id);
        //    return Ok(new GeneralResponse<object>
        //    {
        //        Success = true,
        //        Message = "Store deleted successfully.",
        //        Data = null
        //    });
        //}
    }
}