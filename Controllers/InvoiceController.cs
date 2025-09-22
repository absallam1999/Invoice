using invoice.Core.DTO.Invoice;
using invoice.Core.DTO.PayInvoice;
using invoice.Core.DTO.Payment;
using invoice.Core.DTO.PaymentLink;
using invoice.Core.DTO.Store;
using invoice.Core.Enums;
using invoice.Core.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace invoice.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class InvoiceController : ControllerBase
    {
        private readonly IInvoiceService _invoiceService;

        public InvoiceController(IInvoiceService invoiceService)
        {
            _invoiceService = invoiceService;
        }

        private string GetUserId() =>
        User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? string.Empty;

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var response = await _invoiceService.GetAllAsync(GetUserId());
            return Ok(response);
        }

        [HttpGet("StoreInvoice")]
        public async Task<IActionResult> StoreInvoice()
        {
            var response = await _invoiceService.GetAllForStoreAsync(GetUserId());
            return Ok(response);
        }


        [HttpGet("POSInvoice")]
        public async Task<IActionResult> POSInvoice()
        {
            var response = await _invoiceService.GetForPOSAsync(InvoiceType.Cashier, GetUserId());
            return Ok(response);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(string id)
        {
            var response = await _invoiceService.GetByIdAsync(id, GetUserId());
            return Ok(response);
        }

        [AllowAnonymous]
        [HttpGet("GetWithUser/{id}")]
        public async Task<IActionResult> GetByIdWithUser(string id)
        {
            var response = await _invoiceService.GetByIdWithUserAsync(id);
            return Ok(response);
        }

        [HttpGet("code/{code}")]
        public async Task<IActionResult> GetByCode(string code)
        {
            var response = await _invoiceService.GetByCodeAsync(code, GetUserId());
            return Ok(response);
        }

        [HttpGet("search")]
        public async Task<IActionResult> Search([FromQuery] string keyword)
        {
            var response = await _invoiceService.SearchAsync(keyword, GetUserId());
            return Ok(response);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] InvoiceCreateDTO dto)
        {
            var response = await _invoiceService.CreateAsync(dto, GetUserId());
            return Ok(response);
        }


        [HttpPost("CreateByPOS")]
        public async Task<IActionResult> CreateByPOS([FromBody] InvoiceCreateDTO dto)
        {
            var response = await _invoiceService.CreateAsync(dto, GetUserId());
            if (response?.Data != null)
            {
                var response2 = await _invoiceService.PayAsync(response.Data.Id, GetUserId());
                var response3 = await _invoiceService.GetByIdAsync(response.Data.Id, GetUserId());
                return Ok(response3);


            }
            return Ok(response);
        }

        [HttpPost("range")]
        public async Task<IActionResult> CreateRange([FromBody] IEnumerable<InvoiceCreateDTO> dtos)
        {
            var response = await _invoiceService.CreateRangeAsync(dtos, GetUserId());
            return Ok(response);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(string id, [FromBody] InvoiceUpdateDTO dto)
        {
            var response = await _invoiceService.UpdateAsync(id, dto, GetUserId());
            return Ok(response);
        }

        [HttpPut("pay/{id}")]
        public async Task<IActionResult> Pay(string id, [FromBody] PayInvoiceCreateDTO dto)
        {
            var response = await _invoiceService.PayAsync(id, GetUserId(), dto);
            return Ok(response);
        }

        [HttpPut("Refund/{id}")]
        public async Task<IActionResult> Refund(string id)
        {
            var response = await _invoiceService.RefundAsync(id, GetUserId());
            return Ok(response);
        }

        [HttpPut("ChangeOrderStatus/{id}")]
        public async Task<IActionResult> ChangeOrderStatus(string id, [FromBody] ChangeOrderStatusDTO dto)
        {
            var response = await _invoiceService.ChangeOrderStatus(id, dto, GetUserId());
            return Ok(response);
        }

        [HttpPut("range")]
        public async Task<IActionResult> UpdateRange([FromBody] IEnumerable<InvoiceUpdateDTO> dtos)
        {
            var response = await _invoiceService.UpdateRangeAsync(dtos, GetUserId());
            return Ok(response);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            var response = await _invoiceService.DeleteAsync(id, GetUserId());
            return Ok(response);
        }

        [HttpDelete("range")]
        public async Task<IActionResult> DeleteRange([FromBody] IEnumerable<string> ids)
        {
            var response = await _invoiceService.DeleteRangeAsync(ids, GetUserId());
            return Ok(response);
        }

        [HttpGet("client/{clientId}")]
        public async Task<IActionResult> GetByClient(string clientId)
        {
            var response = await _invoiceService.GetByClientAsync(clientId, GetUserId());
            return Ok(response);
        }



        [HttpGet("status/{status}")]
        public async Task<IActionResult> GetByStatus(InvoiceStatus status)
        {
            var response = await _invoiceService.GetByStatusAsync(status, GetUserId());
            return Ok(response);
        }



        [HttpGet("count")]
        public async Task<IActionResult> Count()
        {
            var count = await _invoiceService.CountAsync(GetUserId());
            return Ok(new { count });
        }

        [HttpGet("total-value")]
        public async Task<IActionResult> GetTotalValue()
        {
            var response = await _invoiceService.GetTotalValueAsync(GetUserId());
            return Ok(response);
        }

        [HttpGet("total-final-value")]
        public async Task<IActionResult> GetTotalFinalValue()
        {
            var response = await _invoiceService.GetTotalFinalValueAsync(GetUserId());
            return Ok(response);
        }

        [HttpPost("{invoiceId}/payments")]
        public async Task<IActionResult> AddPayment(string invoiceId, [FromBody] PaymentCreateDTO dto)
        {
            var response = await _invoiceService.AddPaymentAsync(invoiceId, dto, GetUserId());
            return Ok(response);
        }

        [HttpPost("payment-links")]
        public async Task<IActionResult> GeneratePaymentLink([FromBody] PaymentLinkCreateDTO dto)
        {
            var response = await _invoiceService.GeneratePaymentLinkAsync(dto, GetUserId());
            return Ok(response);
        }

        [HttpPost("{invoiceId}/mark-paid")]
        public async Task<IActionResult> MarkAsPaid(string invoiceId)
        {
            var response = await _invoiceService.MarkAsPaidAsync(invoiceId, GetUserId());
            return Ok(response);
        }

        [HttpPost("{invoiceId}/cancel")]
        public async Task<IActionResult> Cancel(string invoiceId)
        {
            var response = await _invoiceService.CancelAsync(invoiceId, GetUserId());
            return Ok(response);
        }
    }
}