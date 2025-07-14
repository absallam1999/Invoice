using invoice.Data;
using invoice.DTO.Payment;
using invoice.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using invoice.DTO;

namespace invoice.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class PaymentsController : ControllerBase
    {
        private readonly IRepository<Payment> _repository;

        public PaymentsController(IRepository<Payment> repository)
        {
            _repository = repository;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var payments = await _repository.GetAll(p => p.Invoice, p => p.PaymentMethod, p => p.User);
            var result = payments.Select(p => new PaymentDetailsDTO
            {
                Id = p.Id,
                Name = p.Name,
                Cost = p.Cost,
                Date = p.Date,
                InvoiceId = p.InvoiceId,
                InvoiceNumber = p.Invoice.Number,
                PaymentMethodId = p.PaymentMethodId,
                PaymentMethodName = p.PaymentMethod.Name,
                UserId = p.UserId,
                UserName = p.User.UserName
            });

            return Ok(new GeneralResponse<IEnumerable<PaymentDetailsDTO>>
            {
                Success = true,
                Message = "Payments retrieved successfully.",
                Data = result
            });
        }

        [HttpGet("{id}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetById(string id)
        {
            var payment = await _repository.GetById(id, p => p.Invoice, p => p.PaymentMethod, p => p.User);
            if (payment == null)
            {
                return NotFound(new GeneralResponse<object>
                {
                    Success = false,
                    Message = $"Payment with ID {id} not found.",
                    Data = null
                });
            }

            var dto = new PaymentDetailsDTO
            {
                Id = payment.Id,
                Name = payment.Name,
                Cost = payment.Cost,
                Date = payment.Date,
                InvoiceId = payment.InvoiceId,
                InvoiceNumber = payment.Invoice?.Number,
                PaymentMethodId = payment.PaymentMethodId,
                PaymentMethodName = payment.PaymentMethod?.Name,
                UserId = payment.UserId,
                UserName = payment.User?.UserName
            };

            return Ok(new GeneralResponse<PaymentDetailsDTO>
            {
                Success = true,
                Message = "Payment retrieved successfully.",
                Data = dto
            });
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreatePaymentDTO dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new GeneralResponse<object>
                {
                    Success = false,
                    Message = "Invalid data submitted.",
                    Data = ModelState
                });
            }

            var payment = new Payment
            {
                Name = dto.Name,
                Cost = dto.Cost,
                Date = dto.Date,
                InvoiceId = dto.InvoiceId,
                PaymentMethodId = dto.PaymentMethodId,
                UserId = dto.UserId
            };

            await _repository.Add(payment);

            return Ok(new GeneralResponse<Payment>
            {
                Success = true,
                Message = "Payment created successfully.",
                Data = payment
            });
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(string id, [FromBody] UpdatePaymentDTO dto)
        {
            if (id != dto.Id)
            {
                return BadRequest(new GeneralResponse<object>
                {
                    Success = false,
                    Message = "ID mismatch.",
                    Data = null
                });
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(new GeneralResponse<object>
                {
                    Success = false,
                    Message = "Invalid data submitted.",
                    Data = ModelState
                });
            }

            var existing = await _repository.GetById(id);
            if (existing == null)
            {
                return NotFound(new GeneralResponse<object>
                {
                    Success = false,
                    Message = $"Payment with ID {id} not found.",
                    Data = null
                });
            }

            existing.Name = dto.Name;
            existing.Cost = dto.Cost;
            existing.Date = dto.Date;
            existing.InvoiceId = dto.InvoiceId;
            existing.PaymentMethodId = dto.PaymentMethodId;
            existing.UserId = dto.UserId;

            await _repository.Update(existing);

            return Ok(new GeneralResponse<Payment>
            {
                Success = true,
                Message = "Payment updated successfully.",
                Data = existing
            });
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            var payment = await _repository.GetById(id);
            if (payment == null)
            {
                return NotFound(new GeneralResponse<object>
                {
                    Success = false,
                    Message = $"Payment with ID {id} not found.",
                    Data = null
                });
            }

            await _repository.Delete(id);

            return Ok(new GeneralResponse<object>
            {
                Success = true,
                Message = "Payment deleted successfully.",
                Data = null
            });
        }
    }
}
