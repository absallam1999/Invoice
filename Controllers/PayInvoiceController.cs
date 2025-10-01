using AutoMapper;
using invoice.Core.DTO.PayInvoice;
using invoice.Core.Entities;
using invoice.Core.Interfaces.Services;
using Microsoft.AspNetCore.Mvc;

namespace invoice.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PayInvoiceController : ControllerBase
    {
        private readonly IPayInvoiceService _payInvoiceService;
        private readonly IMapper _mapper;

        public PayInvoiceController(IPayInvoiceService payInvoiceService, IMapper mapper)
        {
            _payInvoiceService = payInvoiceService;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var response = await _payInvoiceService.GetAllAsync();
            return Ok(response);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(string id)
        {
            var response = await _payInvoiceService.GetByIdAsync(id);
            return Ok(response);
        }

        [HttpGet("invoice/{invoiceId}")]
        public async Task<IActionResult> GetByInvoiceId(string invoiceId)
        {
            var response = await _payInvoiceService.GetByInvoiceIdAsync(invoiceId);
            return Ok(response);
        }

        [HttpGet("paymentmethod/{paymentMethodId}")]
        public async Task<IActionResult> GetByPaymentMethodId(string paymentMethodId)
        {
            var response = await _payInvoiceService.GetByPaymentMethodIdAsync(paymentMethodId);
            return Ok(response);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] PayInvoiceCreateDTO dto)
        {
            var entity = _mapper.Map<PayInvoice>(dto);
            var response = await _payInvoiceService.CreateAsync(entity);
            return Ok(response);
        }

        [HttpPost("range")]
        public async Task<IActionResult> CreateRange([FromBody] IEnumerable<PayInvoiceCreateDTO> dtos)
        {
            var entities = _mapper.Map<IEnumerable<PayInvoice>>(dtos);
            var response = await _payInvoiceService.CreateRangeAsync(entities);
            return Ok(response);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(string id, [FromBody] PayInvoiceUpdateDTO dto)
        {
            var entity = _mapper.Map<PayInvoice>(dto);
            var response = await _payInvoiceService.UpdateAsync(id, entity);
            return Ok(response);
        }

        [HttpPut("range")]
        public async Task<IActionResult> UpdateRange([FromBody] IEnumerable<PayInvoiceUpdateDTO> dtos)
        {
            var entities = _mapper.Map<IEnumerable<PayInvoice>>(dtos);
            var response = await _payInvoiceService.UpdateRangeAsync(entities);
            return Ok(response);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            var response = await _payInvoiceService.DeleteAsync(id);
            return Ok(response);
        }

        [HttpDelete("range")]
        public async Task<IActionResult> DeleteRange([FromBody] IEnumerable<string> ids)
        {
            var response = await _payInvoiceService.DeleteRangeAsync(ids);
            return Ok(response);
        }
    }
}