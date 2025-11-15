using System.Security.Claims;
using System.Text;
using invoice.Core.DTO;
using invoice.Core.DTO.Payment;
using invoice.Core.DTO.PaymentResponse;
using invoice.Core.DTO.PaymentResponse.TapPayments;
using invoice.Core.Entities;
using invoice.Core.Enums;
using invoice.Core.Interfaces.Services;
using invoice.Services;
using invoice.Services.Payments.TabPayments;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Stripe;
using static invoice.Core.DTO.PaymentResponse.TapPayments.CreateLeadDto;

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



        public PaymentsController(IPaymentService paymentService, IPaymentGateway paymentGateway, UserManager<ApplicationUser> userManager, IAuthService authService)
        {
            _paymentService = paymentService;
            _paymentGateway = paymentGateway;
            _authService = authService;
            _userManager = userManager;


        }

      

        private string GetUserId() =>
            User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? string.Empty;
        #region Tap_Onboarding

        //[HttpPost("create-merchant")]
        //public async Task<IActionResult> CreateMerchant([FromBody] TapUserDto dto)
        //{
        //    if (!ModelState.IsValid)
        //        return BadRequest(ModelState);


        //    try
        //    {
        //        var redirectUrl = await _paymentGateway.CreateMerchantAccountAsync(dto, GetUserId());

        //        if (string.IsNullOrEmpty(redirectUrl))
        //            return BadRequest(new { message = "Failed to create merchant" });

        //        return Ok(new { redirectUrl });
        //    }
        //    catch (Exception ex)
        //    {
        //        return BadRequest(new { error = ex.Message });
        //    }
        //}


        [HttpPost("lead-to-connect")]
        public async Task<IActionResult> LeadToConnect([FromBody] CreateLeadDto dto)
        {
            var leadResponse = await _paymentGateway.CreateLeadAsync(dto, GetUserId());

            if (!leadResponse.Success || string.IsNullOrEmpty(leadResponse.Data))
            {
                return BadRequest("Failed to create lead: " + leadResponse.Message);
            }



            var leadId = leadResponse.Data;

            var connectUrl = await _paymentGateway.CreateConnectUrlAsync(leadId, GetUserId());

            if (connectUrl == null)
                return BadRequest("Lead created, but failed to create connect URL");

            return Ok(new
            {
                leadId,
                connectUrl
            });
        }

            [AllowAnonymous]
        [HttpPost("onboarding-success")]
        public async Task<IActionResult> OnboardingSuccess([FromQuery] string userId, [FromQuery] string accountId, [FromQuery] string status)
        {
            if (string.IsNullOrEmpty(accountId))
                return BadRequest("Missing account id");

            if (string.IsNullOrEmpty(userId))
                return BadRequest("Missing user id");

            if (status?.ToLower() != "success")
            {
                return BadRequest("Onboarding not successful");
            }

            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
                return NotFound("User not found");

            user.TabAccountId = accountId;
            await _userManager.UpdateAsync(user);

            return Ok(new
            {
                message = "Onboarding success",
                userId = userId,
                tapAccountId = accountId
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
            if (!response.Success)
                return NotFound(response);
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
            var response = await _paymentService.GetByPaymentMethodIdAsync(
                paymentMethodId,
                GetUserId()
            );
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
            if (!response.Success)
                return NotFound(response);
            return Ok(response);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            var response = await _paymentService.DeleteAsync(id, GetUserId());
            if (!response.Success)
                return NotFound(response);
            return Ok(response);
        }

        [HttpPost("{id}/cancel")]
        public async Task<IActionResult> Cancel(string id)
        {
            var response = await _paymentService.CancelPaymentAsync(id, GetUserId());
            return Ok(response);
        }

        [HttpPost("tab-WebHook")]
        public async Task<IActionResult> HandleTabWebhook()
        {
            using var reader = new StreamReader(Request.Body, Encoding.UTF8);
            var rawPayload = await reader.ReadToEndAsync();

            var headers = new Dictionary<string, string>();
            foreach (var header in Request.Headers)
            {
                headers[header.Key] = header.Value.ToString();
            }

            var webhookPayload = new WebhookPayloadDTO
            {
                RawPayload = rawPayload,
                Headers = headers,
                Signature = Request.Headers["X-Tap-Signature"].FirstOrDefault() ?? string.Empty,
                SourceIp =
                    Request.HttpContext.Connection.RemoteIpAddress?.ToString() ?? string.Empty,
                UserAgent = Request.Headers["User-Agent"].ToString(),
            };

            var result = await _paymentGateway.HandleWebhookAsync(webhookPayload);

            if (result.Success)
            {
                return Ok("Webhook processed successfully");
            }
            else
            {
                return BadRequest(result.Message);
            }
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
            var count = await _paymentService.CountByPaymentMethodAsync(
                paymentMethodId,
                GetUserId()
            );
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
            var response = await _paymentService.GetTotalPaidByPaymentMethodAsync(
                paymentMethodId,
                GetUserId()
            );
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