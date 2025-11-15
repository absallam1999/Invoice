using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using invoice.Core.Interfaces.Services;
using System.Security.Claims;
using invoice.Core.DTO.Tax;

namespace invoice.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class TaxController : ControllerBase
    {

        private readonly ITaxService _taxService;

        public TaxController(ITaxService taxService)
        {
            _taxService = taxService;
        }

        private string GetUserId() =>
        User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? string.Empty;


        [HttpPost]
        public async Task<IActionResult> Create(TaxReadDTO dto)
        {
            var response = await _taxService.CreateAsync(dto, GetUserId());
            return Ok(response);
        }

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var response = await _taxService.GetByUserIdAsync(GetUserId());
            return Ok(response);
        }

        [AllowAnonymous]
        [HttpGet("{userid}")]
        public async Task<IActionResult> GetTax(string userid)
        {
            var response = await _taxService.GetByUserIdAsync(userid);
            return Ok(response);
        }

        [HttpPut()]
        public async Task<IActionResult> Update(TaxReadDTO dto)
        {
            var response = await _taxService.UpdateAsync(dto, GetUserId());
            return Ok(response);
        }
    }
}