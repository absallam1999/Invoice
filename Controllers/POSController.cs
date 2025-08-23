//using invoice.DTO;
//using invoice.Models.Enums;
//using Microsoft.AspNetCore.Http;
//using Microsoft.AspNetCore.Mvc;
//using System.Security.Claims;
//using invoice.Models.Entites;
//using invoice.Models.DTO.Invoice;
//using invoice.Repo.Data;
//using invoice.Repo;


//namespace invoice.Controllers
//{
//    [Route("api/[controller]")]
//    [ApiController]
//    public class POSController : ControllerBase
//    {
//        private readonly IInvoiceRepository _invoiceRepository;
//        private readonly IRepository<InvoiceItem> _invoiceItemRepository;
//        private readonly IRepository<Product> _productRepository;


//        public POSController(IInvoiceRepository invoiceRepository, IRepository<Product> productRepository, IRepository<InvoiceItem> invoiceItemRepository
//            )
//        {
//            _invoiceRepository = invoiceRepository;
//            _productRepository = productRepository;
//            _invoiceItemRepository = invoiceItemRepository;
            
//        }
//        [HttpPost]
//        public async Task<IActionResult> Create([FromBody] InvoiceInfoDTO dto)
//        {
//            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
//            if (string.IsNullOrEmpty(userId))
//                return Unauthorized();

//            if (!ModelState.IsValid)
//            {
//                return BadRequest(new GeneralResponse<object>
//                {
//                    Success = false,
//                    Message = "Validation failed.",
//                    Data = ModelState
//                });
//            }
//            var invoice = new Invoice
//            {
//                Code = await _invoiceRepository.GenerateInvoiceCode(userId),
//                CreateAt = DateTime.UtcNow,
//                TaxNumber = "tt",
//                InvoiceStatus = InvoiceStatus.Active,
//                InvoiceType = InvoiceType.Cashier,
//                UserId = userId,
//                ClientId = dto.ClientId,
//                LanguageId = "ar",
//                Value = 0,

//            };
//            invoice.InvoiceItems = new List<InvoiceItem>();

//            foreach (var item in dto.Items)
//            {
//                if (item.Quantity <= 0)
//                    return BadRequest($"Invalid quantity for product {item.ProductId}");

//                var product = await _productRepository.GetById(item.ProductId, userId);
//                if (product == null) return BadRequest($"Product {item.ProductId} not found");

               

//                if (product.Quantity != null)
//                {
//                    if (product.Quantity < item.Quantity)
//                        return BadRequest($"Product Quantity not Enough for {product.Name}");

//                    product.Quantity -= item.Quantity;
//                    await _productRepository.Update(product);
//                }


//                invoice.InvoiceItems.Add(new InvoiceItem
//                {
//                    ProductId = product.Id,
//                    Quantity = item.Quantity,
//                    UnitPrice = product.Price,

//                });

//                invoice.Value += product.Price * item.Quantity;
//            }
//                invoice.FinalValue = invoice.Value;


//            if (dto.DiscountType == DiscountType.Amount)
//            {
//                invoice.FinalValue -= dto.DiscountValue ?? 0;
//                invoice.DiscountType = DiscountType.Amount;
//                invoice.DiscountValue = dto.DiscountValue;
//            }
//            else if (dto.DiscountType == DiscountType.Percentage)
//            {
//                invoice.FinalValue -= (invoice.Value * (dto.DiscountValue ?? 0) / 100);
//                invoice.DiscountType = DiscountType.Percentage;
//                invoice.DiscountValue = dto.DiscountValue;
//            }
//            if (invoice.FinalValue < 0)
//                invoice.FinalValue = 0;

//            var result = await _invoiceRepository.Add(invoice);
//            if (!result.Success)
//            {
//                return StatusCode(500, new GeneralResponse<object>
//                {
//                    Success = false,
//                    Message = result.Message ?? "Failed to create invoice.",
//                    Data = null
//                });
//            }

//            return Ok(new GeneralResponse<string>
//            {
//                Success = true,
//                Message = "invoice created successfully.",
//                Data = invoice.Id,

//            });

//        }
//    }
//}
