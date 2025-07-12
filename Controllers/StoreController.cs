using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using invoice.Models;
using invoice.Data;
using invoice.DTO.Store;
using invoice.DTO;
using Microsoft.AspNetCore.Authorization;

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

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var stores = await _repository.GetAll();
            return Ok(new GeneralResponse<IEnumerable<Store>>
            {
                Success = true,
                Message = "Stores retrieved successfully.",
                Data = stores
            });
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(string id)
        {
            var store = await _repository.GetById(id);
            if (store == null)
            {
                return NotFound(new GeneralResponse<Store>
                {
                    Success = false,
                    Message = $"Store with ID {id} not found.",
                    Data = null
                });
            }

            return Ok(new GeneralResponse<Store>
            {
                Success = true,
                Message = "Store retrieved successfully.",
                Data = store
            });
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

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            var store = await _repository.GetById(id);
            if (store == null)
            {
                return NotFound(new GeneralResponse<object>
                {
                    Success = false,
                    Message = $"Store with ID {id} not found.",
                    Data = null
                });
            }

            await _repository.Delete(id);
            return Ok(new GeneralResponse<object>
            {
                Success = true,
                Message = "Store deleted successfully.",
                Data = null
            });
        }
    }
}