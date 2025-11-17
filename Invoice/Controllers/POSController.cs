using invoice.Core.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace invoice.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class POSController : ControllerBase
    {
        private readonly IPOSService _posService;

        public POSController(IPOSService posService)
        {
            _posService = posService;
        }

        private string GetUserId() =>
            User.FindFirstValue(ClaimTypes.NameIdentifier) ?? string.Empty;

        [HttpGet("product/{name}")]
        public async Task<IActionResult> GetProductByName(string name)
        {
            var response = await _posService.GetProductByNameAsync(name, GetUserId());
            return response.Success ? Ok(response) : BadRequest(response);
        }

        [HttpGet("products/search")]
        public async Task<IActionResult> SearchProducts([FromQuery] string keyword)
        {
            var response = await _posService.SearchProductsAsync(keyword, GetUserId());
            return response.Success ? Ok(response) : BadRequest(response);
        }

        [HttpPost("invoice/draft")]
        public async Task<IActionResult> CreateDraftInvoice([FromQuery] string storeId)
        {
            var response = await _posService.CreateDraftInvoiceAsync(storeId, GetUserId());
            return response.Success ? Ok(response) : BadRequest(response);
        }

        [HttpPost("invoice/{invoiceId}/items")]
        public async Task<IActionResult> AddItemToInvoice(string invoiceId, [FromQuery] string productId, [FromQuery] int quantity)
        {
            var response = await _posService.AddItemToInvoiceAsync(invoiceId, productId, quantity, GetUserId());
            return response.Success ? Ok(response) : BadRequest(response);
        }

        [HttpPut("invoice/items/{invoiceItemId}")]
        public async Task<IActionResult> UpdateInvoiceItem(string invoiceItemId, [FromQuery] int newQuantity)
        {
            var response = await _posService.UpdateInvoiceItemAsync(invoiceItemId, newQuantity, GetUserId());
            return response.Success ? Ok(response) : BadRequest(response);
        }

        [HttpDelete("invoice/items/{invoiceItemId}")]
        public async Task<IActionResult> RemoveInvoiceItem(string invoiceItemId)
        {
            var response = await _posService.RemoveInvoiceItemAsync(invoiceItemId, GetUserId());
            return response.Success ? Ok(response) : BadRequest(response);
        }

        [HttpPost("invoice/{invoiceId}/finalize")]
        public async Task<IActionResult> FinalizeSale(
            string invoiceId,
            [FromQuery] string paymentMethodId,
            [FromQuery] decimal paidAmount)
        {
            var response = await _posService.FinalizeSaleAsync(invoiceId, paymentMethodId, paidAmount, GetUserId());
            return response.Success ? Ok(response) : BadRequest(response);
        }

        [HttpPost("invoice/{invoiceId}/cancel")]
        public async Task<IActionResult> CancelSale(string invoiceId)
        {
            var response = await _posService.CancelSaleAsync(invoiceId, GetUserId());
            return response.Success ? Ok(response) : BadRequest(response);
        }

        [HttpPost("invoice/{invoiceId}/refund")]
        public async Task<IActionResult> RefundInvoice(string invoiceId)
        {
            var response = await _posService.RefundInvoiceAsync(invoiceId, GetUserId());
            return response.Success ? Ok(response) : BadRequest(response);
        }

        [HttpGet("invoices/{storeId}")]
        public async Task<IActionResult> GetPOSInvoices(string storeId, [FromQuery] DateTime? date)
        {
            var response = await _posService.GetPOSInvoicesAsync(storeId, date, GetUserId());
            return response.Success ? Ok(response) : BadRequest(response);
        }

        [HttpGet("sales/{storeId}/daily-total")]
        public async Task<IActionResult> GetDailySalesTotal(string storeId, [FromQuery] DateTime date)
        {
            var response = await _posService.GetDailySalesTotalAsync(storeId, date, GetUserId());
            return response.Success ? Ok(response) : BadRequest(response);
        }
    }
}
