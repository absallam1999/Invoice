using invoice.Data;
using invoice.DTO;
using invoice.DTO.Client;
using invoice.DTO.Invoice;
using invoice.DTO.InvoiceItem;
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
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
                return Unauthorized();

            var invoice = await _invoiceRepository.GetById(userId, id);
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
                //url,
                Code = invoice.Code,
                CreateAt = invoice.CreateAt,
                TaxNumber = invoice.TaxNumber,
                Value = invoice.Value,
                InvoiceStatus = invoice.IsPaid ? "paid " : "active",
                InvoiceType = invoice.InvoiceType,
                ClientName = invoice.Client.Name,
                ClientEmail=invoice.Client.Email,
                ClientPhone=invoice.Client.PhoneNumber,
                Language = invoice.Language.Name,
                TermsConditions=invoice.TermsConditions,
                Items=invoice.InvoiceItems.Select(i=>new InvoiceItemDetailsDTO
                {
                   
                    ProductName = i.Product.Name,
                    ProductImge=i.Product.Image,
                    UnitPrice = i.UnitPrice,
                    Quantity = i.Quantity,
                    Subtotal =i.Product.Price
                }).ToList(),

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
                CreateAt = dto.CreateAt ?? DateTime.UtcNow,
                // TaxNumber = dto.TaxNumber,
                IsPaid = false,
                InvoiceType = InvoiceType.Detailed,
                UserId = userId,
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
                    Message = result.Message ?? "Failed to create invoice.",
                    Data = null
                });
            }



            return Ok(new GeneralResponse<InvoiceInfoDTO>
            {
                Success = true,
                Message = "invoice created successfully.",
                Data = invoice.Id,
                
            });

        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(string id, [FromBody] InvoiceInfoDTO dto)
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
                CreateAt = dto.CreateAt ?? DateTime.UtcNow,
                // TaxNumber = dto.TaxNumber,
                InvoiceType = InvoiceType.Detailed,
                UserId = userId,
                ClientId = dto.ClientId,
                LanguageId = dto.LanguageId,
                TermsConditions = dto.TermsConditions,
                Value = 0,


            };
            invoice.InvoiceItems = new List<InvoiceItem>();

            foreach (var item in dto.Items)
            {
                var product = await _productRepository.GetById(item.ProductId, userId);
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

            var result = await _invoiceRepository.Update(invoice);
            if (!result.Success)
            {
                return StatusCode(500, new GeneralResponse<object>
                {
                    Success = false,
                    Message = result.Message ?? "Failed to Update invoice.",
                    Data = null
                });
            }



            return Ok(new GeneralResponse<InvoiceInfoDTO>
            {
                Success = true,
                Message = "invoice updated successfully.",
                Data = invoice.Id
            });
        }
           

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
