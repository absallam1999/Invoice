using invoice.Core.DTO.Currency;
using invoice.Core.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace invoice.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CurrencyController : ControllerBase
    {

        private readonly ICurrencyService _currencyService;

        public CurrencyController(ICurrencyService currencyService)
        {
            _currencyService = currencyService;
        }


        private string GetUserId() =>
       User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? string.Empty;


        [HttpPost]
        public async Task<IActionResult> Create(CurrencyReadDTO dto)
        {
            var response = await _currencyService.CreateAsync(dto, GetUserId());
            return Ok(response);
        }

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var response = await _currencyService.GetByUserIdAsync(GetUserId());
            return Ok(response);
        }



        [AllowAnonymous]
        [HttpGet("{userid}")]
        public async Task<IActionResult> GetCurrency(string userid)
        {
            var response = await _currencyService.GetByUserIdAsync(userid);
            return Ok(response);
        }


        [HttpPut()]
        public async Task<IActionResult> Update(CurrencyReadDTO dto)
        {
            var response = await _currencyService.UpdateAsync(dto, GetUserId());
            return Ok(response);
        }

    }
}