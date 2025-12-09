using invoice.Core.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace invoice.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class PaymentMethodController : ControllerBase
    {
        private readonly IPaymentMethodService _paymentMethodservice;

        public PaymentMethodController(IPaymentMethodService paymentMethodservice)
        {
            _paymentMethodservice = paymentMethodservice;
        }

        private string GetUserId() =>
     User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? string.Empty;


        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var response = await _paymentMethodservice.GetAllAsync();

            return response.Success ? Ok(response) : BadRequest(response);
        }
    }
}