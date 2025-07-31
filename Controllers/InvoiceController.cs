using invoice.Data;
using invoice.DTO;
using invoice.DTO.Client;
using invoice.DTO.Invoice;
using invoice.Models;
using invoice.Models.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace invoice.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class InvoiceController : ControllerBase
    {
        private readonly IRepository<Invoice> _invoiceRepository;
        private readonly IRepository<InvoiceItem> _invoiceItemRepository;
        private readonly IRepository<Product> _productRepository;


        public InvoiceController(IRepository<Invoice> invoiceRepository, IRepository<Product> productRepository, IRepository<InvoiceItem> invoiceItemRepository)
        {
            _invoiceRepository = invoiceRepository;
            _productRepository = productRepository;
            _invoiceItemRepository = invoiceItemRepository;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
                return Unauthorized();


            var invoices = await _invoiceRepository.GetAll(userId,i => i.Client);
            var dtoList = invoices.Select(i => new GetAllInvoiceDTO
            {
                InvoiceId = i.Id,
                InvoiceCode = i.Code,
                InvoiceCreateAt  = i.CreateAt,
                InvoiceValue  = i.Value,
                InvoiceStatus = i.IsPaid ? "paid " : "active",
                InvoiceType = i.InvoiceType,
                ClientId = i.ClientId,
                ClientName=i.Client.Name,
                
              
            });

            return Ok(new GeneralResponse<IEnumerable<GetAllInvoiceDTO>>
            {
                Success = true,
                Message = "Invoices retrieved successfully.",
                Data = dtoList
            });
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(string id)
        {
            var invoice = await _invoiceRepository.GetById(id);
            if (invoice == null)
            {
                return NotFound(new GeneralResponse<object>
                {
                    Success = false,
                    Message = $"Invoice with ID {id} not found.",
                    Data = null
                });
            }

            var dto = new InvoiceDetailsDTO
            {
                Id = invoice.Id,
                Number = invoice.Code,
                CreateAt = invoice.CreateAt,
                TaxNumber = invoice.TaxNumber,
                Value = invoice.Value,
                Description = invoice.Description,
                InvoiceStatus = invoice.IsPaid ? "paid " : "active",
                InvoiceType = invoice.InvoiceType,
                UserId = invoice.UserId,
                StoreId = invoice.StoreId,
                ClientId = invoice.ClientId,
                LanguageId = invoice.LanguageId
            };

            return Ok(new GeneralResponse<InvoiceDetailsDTO>
            {
                Success = true,
                Message = "Invoice retrieved successfully.",
                Data = dto
            });
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] InvoiceInfoDTO dto)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
                return Unauthorized();

            if (!ModelState.IsValid)
            {
                return BadRequest(new GeneralResponse<object>
                {
                    Success = false,
                    Message = "Validation failed.",
                    Data = ModelState
                });
            }


            var invoice = new Invoice
            {
                // Code = dto.Code,
                CreateAt = dto.CreateAt,
                // TaxNumber = dto.TaxNumber,

               // Description = dto.Description,
                //  InvoiceStatus = dto.InvoiceStatus,
                //  InvoiceStatus=InvoiceStatus.Active,
                InvoiceType = dto.InvoiceType,
                UserId = userId,
                //StoreId = dto.StoreId,
                ClientId = dto.ClientId,
                LanguageId = dto.LanguageId,
                TermsConditions = dto.TermsConditions,
                Value = 0,


            };
            invoice.InvoiceItems = new List<InvoiceItem>();

            foreach (var item in dto.Items)
            {
                var product =await _productRepository.GetById(item.ProductId, userId);
                if (product == null) return BadRequest($"Product {item.ProductId} not found");

                invoice.InvoiceItems.Add(new InvoiceItem
                {
                    ProductId = product.Id,
                    Quantity = item.Quantity,
                    UnitPrice = product.Price,

                });

                invoice.Value += product.Price * item.Quantity;

                invoice.FinalValue = invoice.Value;

                if (dto.DiscountType == DiscountType.Amount)
                {
                    invoice.FinalValue -= dto.DiscountValue ?? 0;
                }
                else if (dto.DiscountType == DiscountType.Percentage)
                {
                    invoice.FinalValue -= (invoice.Value * (dto.DiscountValue ?? 0) / 100);
                }

            }

            var result = await _invoiceRepository.Add(invoice);
            if (!result.Success)
            {
                return StatusCode(500, new GeneralResponse<object>
                {
                    Success = false,
                    Message = result.Message ?? "Failed to create client.",
                    Data = null
                });
            }

            return Ok(new GeneralResponse<ClientDetailsDTO>
            {
                Success = true,
                Message = "Client updated successfully.",
                Data = null
            });
        }

        [HttpPut("{id}")]
        //public async Task<IActionResult> Update(string id, [FromBody] InvoiceInfoDTO dto)
        //{
        //    if (id != dto.Id)
        //    {
        //        return BadRequest(new GeneralResponse<object>
        //        {
        //            Success = false,
        //            Message = "Mismatched ID.",
        //            Data = null
        //        });
        //    }

        //    if (!ModelState.IsValid)
        //    {
        //        return BadRequest(new GeneralResponse<object>
        //        {
        //            Success = false,
        //            Message = "Validation failed.",
        //            Data = ModelState
        //        });
        //    }

        //    var invoice = await _invoiceRepository.GetById(id);
        //    if (invoice == null)
        //    {
        //        return NotFound(new GeneralResponse<object>
        //        {
        //            Success = false,
        //            Message = $"Invoice with ID {id} not found.",
        //            Data = null
        //        });
        //    }

        //  //  invoice.Code = dto.nu;
        //    invoice.CreateAt = dto.CreateAt;
        //    invoice.TaxNumber = dto.TaxNumber;
        //    invoice.Value = dto.Value;
        //    invoice.Description = dto.Description;
        //    //invoice.InvoiceStatus = dto.InvoiceStatus;
        //    invoice.InvoiceType = dto.InvoiceType;
        //    invoice.UserId = dto.UserId;
        //    invoice.StoreId = dto.StoreId;
        //    invoice.ClientId = dto.ClientId;
        //    invoice.LanguageId = dto.LanguageId;

        //    await _invoiceRepository.Update(invoice);

        //    var result = new InvoiceDetailsDTO
        //    {
        //        Id = invoice.Id,
        //        Number = invoice.Code,
        //        CreateAt = invoice.CreateAt,
        //        TaxNumber = invoice.TaxNumber,
        //        Value = invoice.Value,
        //        Description = invoice.Description,
        //        InvoiceStatus = invoice.IsPaid ? "paid " : "active",
        //        InvoiceType = invoice.InvoiceType,
        //        UserId = invoice.UserId,
        //        StoreId = invoice.StoreId,
        //        ClientId = invoice.ClientId,
        //        LanguageId = invoice.LanguageId
        //    };

        //    return Ok(new GeneralResponse<InvoiceDetailsDTO>
        //    {
        //        Success = true,
        //        Message = "Invoice updated successfully.",
        //        Data = result
        //    });
        //}

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
                return Unauthorized();

            var invoice = await _invoiceRepository.GetById(id, userId);
            if (invoice == null)
            {
                return NotFound(new GeneralResponse<object>
                {
                    Success = false,
                    Message = $"Invoice with ID {id} not found.",
                    Data = null
                });
            }

            await _invoiceRepository.Delete(id);

            return Ok(new GeneralResponse<object>
            {
                Success = true,
                Message = "Invoice deleted successfully.",
                Data = null
            });
        }
    }
}
