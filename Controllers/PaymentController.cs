using invoice.Core.DTO;
using invoice.Core.DTO.Payment;
using invoice.Core.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using invoice.Core.DTO.PaymentResponse.TapPayments;
using invoice.Services.Payments.TabPayments;
using invoice.Services;
using Microsoft.AspNetCore.Identity;
using invoice.Core.Entities;



namespace invoice.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class PaymentsController : ControllerBase
    {
        private readonly IPaymentService _paymentService;
        private readonly IPaymentGateway _paymentGateway;
        private readonly IAuthService _authService;
        private readonly UserManager<ApplicationUser> _userManager;



        public PaymentsController(IPaymentService paymentService , IPaymentGateway paymentGateway, UserManager<ApplicationUser> userManager, IAuthService authService)
        {
            _paymentService = paymentService;
            _paymentGateway = paymentGateway;
            _authService = authService;
            _userManager = userManager;


        }

        private string GetUserId() => User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? string.Empty;


        #region Tap_Onboardin

        [HttpPost("create-merchant")]
        public async Task<IActionResult> CreateMerchant([FromBody] TapUserDto dto)
        {
                 if (!ModelState.IsValid)
                   return BadRequest(ModelState);


                try
                {
                var redirectUrl = await _paymentGateway.CreateMerchantAccountAsync(dto,GetUserId());

                if (string.IsNullOrEmpty(redirectUrl))
                    return BadRequest(new { message = "Failed to create merchant" });

                return Ok(new { redirectUrl });
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }


        [AllowAnonymous]
        [HttpPost("onboarding-success")]
        public async Task <IActionResult> OnboardingSuccess([FromQuery] string userId, [FromQuery] string account_id, [FromQuery] string status)
        {
            if (string.IsNullOrEmpty(account_id))
                return BadRequest("Missing account id");


            // update status

            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
                return NotFound("User not found");

            user.TabAccountId = account_id;
            await _userManager.UpdateAsync(user);


            return Ok(new
            {
                message = "Onboarding success",
                userId = userId,
                tapAccountId = account_id,
            });


        }


        #endregion

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var response = await _paymentService.GetAllAsync(GetUserId());
            return Ok(response);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(string id)
        {
            var response = await _paymentService.GetByIdAsync(id, GetUserId());
            if (!response.Success) return NotFound(response);
            return Ok(response);
        }

        [HttpGet("invoice/{invoiceId}")]
        public async Task<IActionResult> GetByInvoiceId(string invoiceId)
        {
            var response = await _paymentService.GetByInvoiceIdAsync(invoiceId, GetUserId());
            return Ok(response);
        }

        [HttpGet("method/{paymentMethodId}")]
        public async Task<IActionResult> GetByPaymentMethodId(string paymentMethodId)
        {
            var response = await _paymentService.GetByPaymentMethodIdAsync(paymentMethodId, GetUserId());
            return Ok(response);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] PaymentCreateDTO dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(new GeneralResponse<string>(false, "Invalid payment data"));

            var response = await _paymentService.CreateAsync(dto, GetUserId());

            return Created($"api/payments/{response.Data?.Id}", response);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(string id, [FromBody] PaymentUpdateDTO dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(new GeneralResponse<string>(false, "Invalid update data"));

            var response = await _paymentService.UpdateAsync(id, dto, GetUserId());
            if (!response.Success) return NotFound(response);
            return Ok(response);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            var response = await _paymentService.DeleteAsync(id, GetUserId());
            if (!response.Success) return NotFound(response);
            return Ok(response);
        }

        [HttpPost("{id}/cancel")]
        public async Task<IActionResult> Cancel(string id)
        {
            var response = await _paymentService.CancelPaymentAsync(id, GetUserId());
            return Ok(response);
        }

        [HttpPost("{id}/complete")]
        public async Task<IActionResult> MarkAsCompleted(string id)
        {
            var response = await _paymentService.MarkAsCompletedAsync(id, GetUserId());
            return Ok(response);
        }

        [HttpPost("{id}/fail")]
        public async Task<IActionResult> MarkAsFailed(string id, [FromQuery] string reason)
        {
            var response = await _paymentService.MarkAsFailedAsync(id, reason, GetUserId());
            return Ok(response);
        }

        [HttpPost("{id}/expire")]
        public async Task<IActionResult> Expire(string id)
        {
            var response = await _paymentService.ExpirePaymentAsync(id, GetUserId());
            return Ok(response);
        }

        [HttpPost("{id}/reactivate")]
        public async Task<IActionResult> Reactivate(string id)
        {
            var response = await _paymentService.ReactivatePaymentAsync(id, GetUserId());
            return Ok(response);
        }

        [HttpGet("count")]
        public async Task<IActionResult> Count()
        {
            var count = await _paymentService.CountAsync(GetUserId());
            return Ok(new { count });
        }

        [HttpGet("count/invoice/{invoiceId}")]
        public async Task<IActionResult> CountByInvoice(string invoiceId)
        {
            var count = await _paymentService.CountByInvoiceAsync(invoiceId, GetUserId());
            return Ok(new { count });
        }

        [HttpGet("count/method/{paymentMethodId}")]
        public async Task<IActionResult> CountByPaymentMethod(string paymentMethodId)
        {
            var count = await _paymentService.CountByPaymentMethodAsync(paymentMethodId, GetUserId());
            return Ok(new { count });
        }

        [HttpGet("total/invoice/{invoiceId}")]
        public async Task<IActionResult> GetTotalPaidByInvoice(string invoiceId)
        {
            var response = await _paymentService.GetTotalPaidByInvoiceAsync(invoiceId, GetUserId());
            return Ok(response);
        }

        [HttpGet("total/method/{paymentMethodId}")]
        public async Task<IActionResult> GetTotalPaidByPaymentMethod(string paymentMethodId)
        {
            var response = await _paymentService.GetTotalPaidByPaymentMethodAsync(paymentMethodId, GetUserId());
            return Ok(response);
        }

        [HttpGet("total/user")]
        public async Task<IActionResult> GetTotalPaidByUser()
        {
            var response = await _paymentService.GetTotalPaidByUserAsync(GetUserId());
            return Ok(response);
        }

        [HttpGet("revenue/monthly/{year}")]
        public async Task<IActionResult> GetMonthlyRevenue(int year)
        {
            var response = await _paymentService.GetMonthlyRevenueAsync(year, GetUserId());
            return Ok(response);
        }

        [HttpGet("revenue/methods")]
        public async Task<IActionResult> GetRevenueByPaymentMethod()
        {
            var response = await _paymentService.GetRevenueByPaymentMethodAsync(GetUserId());
            return Ok(response);
        }
    }
}
