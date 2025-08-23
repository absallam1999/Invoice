using invoice.Core.DTO.Store;
using invoice.Core.DTO.StoreSettings;
using invoice.Core.Entites;
using invoice.Core.Enums;
using invoice.Core.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Linq.Expressions;

namespace invoice.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class StoreController : ControllerBase
    {
        private readonly IStoreService _storeService;

        public StoreController(IStoreService storeService)
        {
            _storeService = storeService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] string userId)
        {
            var result = await _storeService.GetAllAsync(userId);
            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(string id, [FromQuery] string userId)
        {
            var result = await _storeService.GetByIdAsync(id, userId);
            return Ok(result);
        }

        [HttpGet("by-user")]
        public async Task<IActionResult> GetByUser([FromQuery] string userId)
        {
            var result = await _storeService.GetByUserAsync(userId);
            return Ok(result);
        }

        [HttpGet("by-language/{languageId}")]
        public async Task<IActionResult> GetByLanguage(string languageId, [FromQuery] string userId)
        {
            var result = await _storeService.GetByLanguageAsync(languageId, userId);
            return Ok(result);
        }

        [HttpGet("active")]
        public async Task<IActionResult> GetActiveStores([FromQuery] string userId)
        {
            var result = await _storeService.GetActiveStoresAsync(userId);
            return Ok(result);
        }

        [HttpGet("inactive")]
        public async Task<IActionResult> GetInactiveStores([FromQuery] string userId)
        {
            var result = await _storeService.GetInactiveStoresAsync(userId);
            return Ok(result);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] StoreCreateDTO dto, [FromQuery] string userId)
        {
            var result = await _storeService.CreateAsync(dto, userId);
            return Ok(result);
        }

        [HttpPost("range")]
        public async Task<IActionResult> AddRange([FromBody] IEnumerable<StoreCreateDTO> dtos, [FromQuery] string userId)
        {
            var result = await _storeService.AddRangeAsync(dtos, userId);
            return Ok(result);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(string id, [FromBody] StoreUpdateDTO dto, [FromQuery] string userId)
        {
            var result = await _storeService.UpdateAsync(id, dto, userId);
            return Ok(result);
        }

        [HttpPut("range")]
        public async Task<IActionResult> UpdateRange([FromBody] IEnumerable<StoreUpdateDTO> dtos, [FromQuery] string userId)
        {
            var result = await _storeService.UpdateRangeAsync(dtos, userId);
            return Ok(result);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string id, [FromQuery] string userId)
        {
            var result = await _storeService.DeleteAsync(id, userId);
            return Ok(result);
        }

        [HttpDelete("range")]
        public async Task<IActionResult> DeleteRange([FromBody] IEnumerable<string> ids, [FromQuery] string userId)
        {
            var result = await _storeService.DeleteRangeAsync(ids, userId);
            return Ok(result);
        }

        [HttpPatch("{id}/activate")]
        public async Task<IActionResult> ActivateStore(string id, [FromQuery] string userId)
        {
            var result = await _storeService.ActivateStoreAsync(id, userId);
            return Ok(result);
        }

        [HttpPatch("{id}/deactivate")]
        public async Task<IActionResult> DeactivateStore(string id, [FromQuery] string userId)
        {
            var result = await _storeService.DeactivateStoreAsync(id, userId);
            return Ok(result);
        }

        [HttpGet("{storeId}/settings")]
        public async Task<IActionResult> GetSettings(string storeId, [FromQuery] string userId)
        {
            var result = await _storeService.GetSettingsAsync(storeId, userId);
            return Ok(result);
        }

        [HttpPut("{storeId}/settings")]
        public async Task<IActionResult> UpdateSettings(string storeId, [FromBody] StoreSettingsReadDTO settingsDto, [FromQuery] string userId)
        {
            var result = await _storeService.UpdateSettingsAsync(storeId, settingsDto, userId);
            return Ok(result);
        }

        [HttpPatch("{storeId}/payment-method")]
        public async Task<IActionResult> UpdatePaymentMethods(string storeId, [FromQuery] PaymentType paymentType, [FromQuery] string userId)
        {
            var result = await _storeService.UpdatePaymentMethodsAsync(storeId, paymentType, userId);
            return Ok(result);
        }

        [HttpPost("query")]
        public async Task<IActionResult> Query([FromBody] Expression<Func<Store, bool>> predicate, [FromQuery] string userId)
        {
            var result = await _storeService.QueryAsync(predicate, userId);
            return Ok(result);
        }
    }
}
