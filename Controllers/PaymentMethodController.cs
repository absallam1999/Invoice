using invoice.Core.Enums;
using invoice.Core.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace invoice.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class PaymentMethodController : ControllerBase
    {
        private readonly IPaymentMethodService _paymentMethodService;

        public PaymentMethodController(IPaymentMethodService paymentMethodService)
        {
            _paymentMethodService = paymentMethodService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var response = await _paymentMethodService.GetAllAsync();
            return Ok(response);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(string id)
        {
            var response = await _paymentMethodService.GetByIdAsync(id);
            return Ok(response);
        }

        [HttpGet("get-id-from-type/{paymentType}")]
        public async Task<IActionResult> GetIdFromType(PaymentType paymentType)
        {
            var response = await _paymentMethodService.GetIdFromTypeAsync(paymentType);
            return Ok(response);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromQuery] PaymentType type)
        {
            var response = await _paymentMethodService.CreateAsync(type);
            return Ok(response);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(string id, [FromQuery] PaymentType type)
        {
            var response = await _paymentMethodService.UpdateAsync(id, type);
            return Ok(response);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            var response = await _paymentMethodService.DeleteAsync(id);
            return Ok(response);
        }
    }
}
