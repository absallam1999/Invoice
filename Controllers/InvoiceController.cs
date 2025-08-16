using invoice.Data;
using invoice.DTO;
using invoice.DTO.Invoice;
using invoice.DTO.InvoiceItem;
using invoice.Models;
using invoice.Models.Enums;
using invoice.Models.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace invoice.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class InvoiceController : ControllerBase
    {
        private readonly IInvoiceRepository _invoiceRepository;
        private readonly IRepository<InvoiceItem> _invoiceItemRepository;
        private readonly IRepository<Product> _productRepository;
        private readonly IRepository<PayInvoice> _PayinvoicRepository;


        public InvoiceController(IRepository<Invoice> InvoiceRepository, IRepository<Product> ProductRepository, IRepository<InvoiceItem> InvoiceItemRepository
            , IRepository<PayInvoice> PayinvoicRepository , IInvoiceRepository invoiceRepository)
        {
            _invoiceRepository = invoiceRepository;
            _productRepository = ProductRepository;
            _invoiceItemRepository = InvoiceItemRepository;
            _PayinvoicRepository = PayinvoicRepository;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
                return Unauthorized();

            var invoices = (await _invoiceRepository.GetAll(userId, i => i.Client))
                .OrderByDescending(i => i.CreateAt);

            var dtoList = invoices.Select(i => new GetAllInvoiceDTO
            {
                InvoiceId = i.Id,
                InvoiceCode = i.Code,
                InvoiceCreateAt  = i.CreateAt,
                InvoiceValue  = i.FinalValue,
                InvoiceStatus = i.InvoiceStatus,
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

            //var invoice = await _invoiceRepository.GetById(id, userId, i => i.Client,i=>i.Language,i=>i.InvoiceItems)
            //.ThenInclude(ii => ii.Product);

            var invoice = await _invoiceRepository.GetById(id,userId, q => q
                 .Include(i => i.Client)
                .Include(i => i.Language) 
                .Include(i => i.PayInvoice)
                .Include(i => i.InvoiceItems)
                    .ThenInclude(ii => ii.Product)
                    );

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
                //url,TaxNumber
                Code = invoice.Code,
                CreateAt = invoice.CreateAt,
                TaxNumber = invoice.TaxNumber,
                Value = invoice.Value,
                FinalValue= invoice.FinalValue,
                DiscountValue = invoice.DiscountValue,
                DiscountType = invoice.DiscountType,
                InvoiceStatus = invoice.InvoiceStatus,
                InvoiceType = invoice.InvoiceType,
                PayAt = invoice.PayInvoice?.PayAt,
                ClintId=invoice.ClientId ?? string.Empty,
                ClientName = invoice.Client?.Name ?? string.Empty,
                ClientEmail=invoice.Client?.Email ?? string.Empty,
                ClientPhone=invoice.Client?.PhoneNumber ?? string.Empty,
                PaymentMethod=invoice.PayInvoice?.PaymentMethod?.Name ?? string.Empty,
                Language = invoice.Language.Name,
                TermsConditions=invoice.TermsConditions,
                Items=invoice.InvoiceItems.Select(i=>new InvoiceItemDetailsDTO
                {
                    ProductId= i.ProductId,
                    ProductName = i.Product.Name,
                    ProductImge=i.Product.Image,
                    UnitPrice = i.UnitPrice,
                    Quantity = i.Quantity,
                    Subtotal = i.UnitPrice * i.Quantity

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
                Code = await _invoiceRepository.GenerateInvoiceCode(userId),
                CreateAt = dto.CreateAt ?? DateTime.UtcNow,
                TaxNumber = "jjj",
                InvoiceStatus=InvoiceStatus.Active,
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
            }
                invoice.FinalValue = invoice.Value;

                if (dto.DiscountType == DiscountType.Amount)
                {
                    invoice.FinalValue -= dto.DiscountValue ?? 0;
                    invoice.DiscountType= DiscountType.Amount;
                    invoice.DiscountValue=dto.DiscountValue;
                }
                else if (dto.DiscountType == DiscountType.Percentage)
                {
                    invoice.FinalValue -= (invoice.Value * (dto.DiscountValue ?? 0) / 100);
                    invoice.DiscountType = DiscountType.Percentage;
                    invoice.DiscountValue = dto.DiscountValue;
                }
            if (invoice.FinalValue < 0)
                invoice.FinalValue = 0;



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
            
            return Ok(new GeneralResponse<string>
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
            //var invoice = await _invoiceRepository.GetById(id,userId);
            var invoice = await _invoiceRepository.GetById(id, userId, q => q
                .Include(i => i.Client)
               .Include(i => i.Language)
               .Include(i => i.PayInvoice)
               .Include(i => i.InvoiceItems)
                   .ThenInclude(ii => ii.Product)
                   );
            if (invoice == null)
            {
                return NotFound(new GeneralResponse<object>
                {
                    Success = false,
                    Message = $"Invoice with ID {id} not found.",
                    Data = null
                });
            }
            invoice.CreateAt = dto.CreateAt ?? invoice.CreateAt;
           // invoice.InvoiceType = InvoiceType.Detailed;
            invoice.ClientId = dto.ClientId;
            invoice.LanguageId = dto.LanguageId;
            invoice.TermsConditions = dto.TermsConditions;
            invoice.Value = 0;

            invoice.InvoiceItems = new List<InvoiceItem>();

            foreach (var item in dto.Items)
            {
                var product = await _productRepository.GetById(item.ProductId, userId);
                if (product == null) return BadRequest($"Product {item.ProductId} not found");



                if (product.Quantity != null)
                {
                    if (product.Quantity < item.Quantity)
                        return BadRequest($"Product Quantity not Enough for {product.Name}");

                    product.Quantity -= item.Quantity;
                    await _productRepository.Update(product);
                }
                invoice.InvoiceItems.Add(new InvoiceItem
                {
                    ProductId = product.Id,
                    Quantity = item.Quantity,
                    UnitPrice = product.Price,
                });

                invoice.Value += product.Price * item.Quantity;
            }
                invoice.FinalValue = invoice.Value;

                if (dto.DiscountType == DiscountType.Amount)
                {
                    invoice.FinalValue -= dto.DiscountValue ?? 0;
                    invoice.DiscountType = DiscountType.Amount;
                    invoice.DiscountValue = dto.DiscountValue;
                }
                else if (dto.DiscountType == DiscountType.Percentage)
                {
                    invoice.FinalValue -= (invoice.Value * (dto.DiscountValue ?? 0) / 100);
                    invoice.DiscountType = DiscountType.Percentage;
                    invoice.DiscountValue = dto.DiscountValue;
                }
                if (invoice.FinalValue < 0)
                    invoice.FinalValue = 0;


            

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



            return Ok(new GeneralResponse<string>
            {
                Success = true,
                Message = "invoice updated successfully.",
                Data = invoice.Id
            });
        }

        [HttpPut("PayInvoice/{id}")]
        public async Task<IActionResult> PayInvoice(string id, [FromBody] PayinvoiceDTO dto)
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

            var invoice = await _invoiceRepository.GetById( id, userId);
            if (invoice == null)
            {
                return NotFound(new GeneralResponse<object>
                {
                    Success = false,
                    Message = $"Invoice with ID {id} not found.",
                    Data = null
                });
            }
            invoice.InvoiceStatus = InvoiceStatus.Paid;

            PayInvoice Payinvoice = new PayInvoice
            {
                PayAt = dto.PayAt?? DateTime.UtcNow,
                PaymentMethodId = dto.PaymentMethodId,
                InvoiceId = id,
            };
            var resultPayment = await _PayinvoicRepository.Add(Payinvoice);
            if (!resultPayment.Success)
            {
                return StatusCode(500, new GeneralResponse<object>
                {
                    Success = false,
                    Message = resultPayment.Message ?? "Failed to record payment.",
                    Data = null
                });
            }

            var resultInvoice = await _invoiceRepository.Update(invoice);
            if (!resultInvoice.Success)
            {
                return StatusCode(500, new GeneralResponse<object>
                {
                    Success = false,
                    Message = resultInvoice.Message ?? "Failed to update invoice status.",
                    Data = null
                });
            }
            return Ok(new GeneralResponse<object>
            {
                Success = true,
                Message = "Invoice paid successfully.",
                Data =invoice.Id
            });
        }
        [HttpPut("RefundInvoice/{id}")]
        public async Task<IActionResult> RefundInvoice(string id)
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

            invoice.InvoiceStatus = InvoiceStatus.Refund;
           

            var result = await _invoiceRepository.Update(invoice);
            if (!result.Success)
            {
                return StatusCode(500, new GeneralResponse<object>
                {
                    Success = false,
                    Message = result.Message ?? "Failed to Refund invoice.",
                    Data = null
                });
            }

            return Ok(new GeneralResponse<string>
            {
                Success = true,
                Message = "invoice Refunded successfully.",
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
