using System.Security.Claims;
using invoice.Core.DTO.Store;
using invoice.Core.DTO.StoreSettings;
using invoice.Core.Enums;
using invoice.Core.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

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

        private string GetUserId() =>
            User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? string.Empty;

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var result = await _storeService.GetAllAsync(GetUserId());
            return result.Success ? Ok(result) : BadRequest(result);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(string id)
        {
            var result = await _storeService.GetByIdAsync(id, GetUserId());
            return result.Success ? Ok(result) : BadRequest(result);
        }

        [HttpGet("by-user")]
        public async Task<IActionResult> GetByUser()
        {
            var result = await _storeService.GetByUserAsync(GetUserId());
            return result.Success ? Ok(result) : BadRequest(result);
        }

        [HttpGet("by-language/{languageId}")]
        public async Task<IActionResult> GetByLanguage(string languageId)
        {
            var result = await _storeService.GetByLanguageAsync(languageId, GetUserId());
            return result.Success ? Ok(result) : BadRequest(result);
        }

        [HttpGet("active")]
        public async Task<IActionResult> GetActiveStores()
        {
            var result = await _storeService.GetActiveStoresAsync(GetUserId());
            return result.Success ? Ok(result) : BadRequest(result);
        }

        [HttpGet("inactive")]
        public async Task<IActionResult> GetInactiveStores()
        {
            var result = await _storeService.GetInactiveStoresAsync(GetUserId());
            return result.Success ? Ok(result) : BadRequest(result);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] StoreCreateDTO dto)
        {
            var result = await _storeService.CreateAsync(dto, GetUserId());
            return result.Success ? Ok(result) : BadRequest(result);
        }

        [HttpPost("range")]
        public async Task<IActionResult> AddRange([FromBody] IEnumerable<StoreCreateDTO> dtos)
        {
            var result = await _storeService.AddRangeAsync(dtos, GetUserId());
            return result.Success ? Ok(result) : BadRequest(result);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(string id, [FromBody] StoreUpdateDTO dto)
        {
            var result = await _storeService.UpdateAsync(id, dto, GetUserId());
            return result.Success ? Ok(result) : BadRequest(result);
        }

        [HttpPut("range")]
        public async Task<IActionResult> UpdateRange([FromBody] IEnumerable<StoreUpdateDTO> dtos)
        {
            var result = await _storeService.UpdateRangeAsync(dtos, GetUserId());
            return result.Success ? Ok(result) : BadRequest(result);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            var result = await _storeService.DeleteAsync(id, GetUserId());
            return result.Success ? Ok(result) : BadRequest(result);
        }

        [HttpDelete("range")]
        public async Task<IActionResult> DeleteRange([FromBody] IEnumerable<string> ids)
        {
            var result = await _storeService.DeleteRangeAsync(ids, GetUserId());
            return result.Success ? Ok(result) : BadRequest(result);
        }

        [HttpPatch("{id}/activate")]
        public async Task<IActionResult> ActivateStore(string id)
        {
            var result = await _storeService.ActivateStoreAsync(id, GetUserId());
            return result.Success ? Ok(result) : BadRequest(result);
        }

        [HttpPatch("{id}/deactivate")]
        public async Task<IActionResult> DeactivateStore(string id)
        {
            var result = await _storeService.DeactivateStoreAsync(id, GetUserId());
            return result.Success ? Ok(result) : BadRequest(result);
        }

        [HttpGet("{storeId}/settings")]
        public async Task<IActionResult> GetSettings(string storeId)
        {
            var result = await _storeService.GetSettingsAsync(storeId, GetUserId());
            return result.Success ? Ok(result) : BadRequest(result);
        }

        [HttpPut("{storeId}/settings")]
        public async Task<IActionResult> UpdateSettings(
            string storeId,
            [FromForm] StoreSettingsUpdateDTO request)
        {
            var result = await _storeService.UpdateSettingsAsync(storeId, request, GetUserId());
            return result.Success ? Ok(result) : BadRequest(result);
        }

        [HttpPatch("{storeId}/payment-method")]
        public async Task<IActionResult> UpdatePaymentMethods(string storeId, [FromQuery] PaymentType paymentType)
        {
            var result = await _storeService.UpdatePaymentMethodsAsync(storeId, paymentType, GetUserId());
            return result.Success ? Ok(result) : BadRequest(result);
        }
    }
}
