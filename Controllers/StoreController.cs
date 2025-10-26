using invoice.Core.DTO.Store;
using invoice.Core.DTO.StoreSettings;
using invoice.Core.Entities;
using invoice.Core.Enums;
using invoice.Core.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Linq.Expressions;
using System.Security.Claims;

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


        [HttpPost]
        public async Task<IActionResult> Create([FromForm] StoreCreateDTO dto)
        {
            var result = await _storeService.CreateAsync(dto, GetUserId());
            return Ok(result);
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var result = await _storeService.GetAsync(GetUserId());
            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(string id)
        {
            var result = await _storeService.GetByIdAsync(id, GetUserId());
            return Ok(result);
        }


        [HttpPut()]
        public async Task<IActionResult> Update( [FromForm] StoreUpdateDTO dto)
        {
            var result = await _storeService.UpdateAsync( dto, GetUserId ());
            return Ok(result);
        }


        [AllowAnonymous]
        [HttpGet("slug/{slug}")]
        public async Task<IActionResult> GetBySlug(string slug)
        {
            var result = await _storeService.GetBySlug(slug);
            return Ok(result);
        }


        [AllowAnonymous]
        [HttpPost("CreateOrder/{userid}/{storeId}")]
        public async Task<IActionResult> CreateOrder(string userid,string storeId, [FromBody] CreateOrderDTO dto)
        {
            var result = await _storeService.CreateOrderAsync(dto, userid, storeId);
            return Ok(result);
        }



        [HttpGet("by-user")]
        public async Task<IActionResult> GetByUser([FromQuery] string userId)
        {
            var result = await _storeService.GetByUserAsync(userId);
            return Ok(result);
        }




     



        [HttpPost("range")]
        public async Task<IActionResult> AddRange([FromBody] IEnumerable<StoreCreateDTO> dtos, [FromQuery] string userId)
        {
            var result = await _storeService.AddRangeAsync(dtos, userId);
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

        [HttpPut("activate")]
        public async Task<IActionResult> ActivateStore()
        {
            var result = await _storeService.ActivateStoreAsync(GetUserId());
            return Ok(result);
        }

        //[HttpPatch("{id}/deactivate")]
        //public async Task<IActionResult> DeactivateStore(string id, [FromQuery] string userId)
        //{
        //    var result = await _storeService.DeactivateStoreAsync(id, userId);
        //    return Ok(result);
        //}

        [HttpGet("{storeId}/settings")]
        public async Task<IActionResult> GetSettings(string storeId, [FromQuery] string userId)
        {
            var result = await _storeService.GetSettingsAsync(storeId, userId);
            return Ok(result);
        }

        [HttpPut("{storeId}/settings")]
        public async Task<IActionResult> UpdateSettings(string storeId, [FromBody] StoreSettingsUpdateDTO settingsDto, [FromQuery] string userId)
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