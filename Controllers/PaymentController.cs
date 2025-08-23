using invoice.Core.DTO;
using invoice.Core.DTO.Payment;
using invoice.Core.Interfaces.Services;
using Microsoft.AspNetCore.Mvc;

namespace invoice.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PaymentController : ControllerBase
    {
        private readonly IPaymentService _service;

        public PaymentController(IPaymentService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] string userId = null)
        {
            var response = await _service.GetAllAsync(userId);
            return Ok(response);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(string id, [FromQuery] string userId = null)
        {
            var response = await _service.GetByIdAsync(id, userId);
            return Ok(response);
        }

        [HttpPost("process/{paymentMethodId}")]
        public async Task<IActionResult> ProcessPayment([FromRoute] string paymentMethodId, [FromBody] PaymentCreateDTO dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(new GeneralResponse<object>
                {
                    Success = false,
                    Message = "Invalid data submitted",
                    Data = ModelState
                });

            var userId = HttpContext.User?.Identity?.Name;
            var result = await _service.ProcessPaymentAsync(paymentMethodId, dto, userId);
            return Ok(result);
        }

        [HttpGet("invoice/{invoiceId}")]
        public async Task<IActionResult> GetByInvoice(string invoiceId, [FromQuery] string userId = null)
        {
            var response = await _service.GetByInvoiceIdAsync(invoiceId, userId);
            return Ok(response);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] PaymentCreateDTO dto, [FromQuery] string userId)
        {
            var response = await _service.CreateAsync(dto, userId);
            return Ok(response);
        }

        [HttpPost("range")]
        public async Task<IActionResult> CreateRange([FromBody] IEnumerable<PaymentCreateDTO> dtos, [FromQuery] string userId)
        {
            var response = await _service.CreateRangeAsync(dtos, userId);
            return Ok(response);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(string id, [FromBody] PaymentUpdateDTO dto, [FromQuery] string userId)
        {
            var response = await _service.UpdateAsync(id, dto, userId);
            return Ok(response);
        }

        [HttpPut("range")]
        public async Task<IActionResult> UpdateRange([FromBody] IEnumerable<PaymentUpdateDTO> dtos, [FromQuery] string userId)
        {
            var response = await _service.UpdateRangeAsync(dtos, userId);
            return Ok(response);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string id, [FromQuery] string userId)
        {
            var response = await _service.DeleteAsync(id, userId);
            return Ok(response);
        }

        [HttpDelete("range")]
        public async Task<IActionResult> DeleteRange([FromBody] IEnumerable<string> ids, [FromQuery] string userId)
        {
            var response = await _service.DeleteRangeAsync(ids, userId);
            return Ok(response);
        }
    }
}

//    [HttpPost("stripePayment")]
//    public async Task<IActionResult> StripePayment([FromBody] StripePaymentDTO dto)
//    {
//        if (!ModelState.IsValid)
//            return BadRequest(new GeneralResponse<object>
//            {
//                Success = false,
//                Message = "Invalid data submitted.",
//                Data = ModelState
//            });

//        // Retrieve the PaymentMethod
//        var paymentMethod = await _paymentMethodService.GetByIdAsync(dto.PaymentMethodId);
//        if (paymentMethod == null)
//            return NotFound(new GeneralResponse<object>
//            {
//                Success = false,
//                Message = "Payment method not found."
//            });

//        // Prepare invoice for Stripe
//        var invoice = await _invoiceService.GetByIdAsync(dto.InvoiceId, null);
//        if (invoice == null || !invoice.Success)
//            return NotFound(new GeneralResponse<object>
//            {
//                Success = false,
//                Message = "Invoice not found."
//            });

//        var domain = "http://localhost:7230/";
//        var options = new Stripe.Checkout.SessionCreateOptions
//        {
//            SuccessUrl = $"{domain}Invoices/OrderConfirm?session_id={{CHECKOUT_SESSION_ID}}",
//            CancelUrl = $"{domain}Invoices/CancelOrder",
//            LineItems = new List<Stripe.Checkout.SessionLineItemOptions>(),
//            Mode = "payment",
//            CustomerEmail = dto.CustomerEmail
//        };

//        options.LineItems.Add(new Stripe.Checkout.SessionLineItemOptions
//        {
//            PriceData = new Stripe.Checkout.SessionLineItemPriceDataOptions
//            {
//                UnitAmount = (long)(dto.Cost * 100),
//                Currency = "usd",
//                ProductData = new Stripe.Checkout.SessionLineItemPriceDataProductDataOptions
//                {
//                    Name = dto.Name,
//                    TaxCode = invoice.Data.TaxNumber
//                }
//            },
//            Quantity = 1
//        });

//        var service = new Stripe.Checkout.SessionService();
//        var session = service.Create(options);

//        // Record payment in database (with PaymentMethod)
//        var payment = new Payment
//        {
//            Name = dto.Name,
//            Cost = dto.Cost,
//            UserId = "stripe", // Or actual user id
//            InvoiceId = dto.InvoiceId,
//            PaymentMethodId = dto.PaymentMethodId,
//            Date = DateTime.UtcNow
//        };
//        await _paymentService.CreateAsync(payment);

//        return Ok(new GeneralResponse<string>
//        {
//            Success = true,
//            Message = "Stripe session created successfully.",
//            Data = session.Url
//        });
//    }

