using AutoMapper;
using invoice.Core.DTO;
using invoice.Core.DTO.Invoice;
using invoice.Core.Entities;
using invoice.Core.Enums;
using invoice.Core.Interfaces.Services;
using invoice.Repo;

namespace invoice.Services
{
    public class POSService : IPOSService
    {
        private readonly IRepository<Product> _productRepo;
        private readonly IRepository<Invoice> _invoiceRepo;
        private readonly IRepository<InvoiceItem> _invoiceItemRepo;
        private readonly IRepository<PaymentMethod> _paymentMethodRepo;
        private readonly IRepository<Payment> _paymentRepo;
        private readonly IMapper _mapper;

        public POSService(
            IRepository<Product> productRepo,
            IRepository<Invoice> invoiceRepo,
            IRepository<InvoiceItem> invoiceItemRepo,
            IRepository<PaymentMethod> paymentMethodRepo,
            IRepository<Payment> paymentRepo,
            IMapper mapper)
        {
            _productRepo = productRepo;
            _invoiceRepo = invoiceRepo;
            _invoiceItemRepo = invoiceItemRepo;
            _paymentMethodRepo = paymentMethodRepo;
            _paymentRepo = paymentRepo;
            _mapper = mapper;
        }

        public async Task<GeneralResponse<Product>> GetProductByNameAsync(string name, string userId)
        {
            var products = await _productRepo.QueryAsync(
                p => p.Name == name && p.UserId == userId);

            var product = products.FirstOrDefault();

            return product == null
                ? new GeneralResponse<Product> { Success = false, Message = "Product not found" }
                : new GeneralResponse<Product> { Success = true, Message = "Product retrieved", Data = product };
        }

        public async Task<GeneralResponse<IEnumerable<Product>>> SearchProductsAsync(string keyword, string userId)
        {
            var products = await _productRepo.QueryAsync(
                p => p.InStore == true && p.UserId == userId &&
                     (p.Name.Contains(keyword)));

            return new GeneralResponse<IEnumerable<Product>>
            {
                Success = products.Any(),
                Message = products.Any() ? "Products found" : "No products matched",
                Data = products
            };
        }

        public async Task<GeneralResponse<InvoiceReadDTO>> CreateDraftInvoiceAsync(string storeId, string userId)
        {

            var invoice = new Invoice
            {
                Code = $"INV-{DateTime.UtcNow.Ticks}",
              //  StoreId = storeId,
                UserId = userId,
                InvoiceStatus = InvoiceStatus.Draft,
                InvoiceType = InvoiceType.Cashier,
               // Currency = "USD",
                Value = 0,
                FinalValue = 0,
                LanguageId = "AR_I",
                InvoiceItems = new List<InvoiceItem>()
            };

            await _invoiceRepo.AddAsync(invoice);

            return new GeneralResponse<InvoiceReadDTO>
            {
                Success = true,
                Message = "Draft invoice created",
                Data = _mapper.Map<InvoiceReadDTO>(invoice)
            };
        }

        public async Task<GeneralResponse<InvoiceReadDTO>> AddItemToInvoiceAsync(string invoiceId, string productId, int quantity, string userId)
        {
            var invoice = await _invoiceRepo.GetByIdAsync(invoiceId);
            if (invoice == null || invoice.UserId != userId)
                return new GeneralResponse<InvoiceReadDTO> { Success = false, Message = "Invoice not found" };

            var product = await _productRepo.GetByIdAsync(productId);
            if (product == null)
                return new GeneralResponse<InvoiceReadDTO> { Success = false, Message = "Product not found" };

            var invoiceItem = new InvoiceItem
            {
                InvoiceId = invoiceId,
                ProductId = productId,
                Quantity = quantity,
                UnitPrice = product.Price,
            };

            invoice.Value += invoiceItem.LineTotal;
            invoice.FinalValue = invoice.Value;

            await _invoiceItemRepo.AddAsync(invoiceItem);
            await _invoiceRepo.UpdateAsync(invoice);

            return new GeneralResponse<InvoiceReadDTO>
            {
                Success = true,
                Message = "Item added to invoice",
                Data = _mapper.Map<InvoiceReadDTO>(invoice)
            };
        }

        public async Task<GeneralResponse<InvoiceReadDTO>> UpdateInvoiceItemAsync(string invoiceItemId, int newQuantity, string userId)
        {
            var invoiceItem = await _invoiceItemRepo.GetByIdAsync(invoiceItemId);
            if (invoiceItem == null)
                return new GeneralResponse<InvoiceReadDTO> { Success = false, Message = "Invoice item not found" };

            var invoice = await _invoiceRepo.GetByIdAsync(invoiceItem.InvoiceId);
            if (invoice == null || invoice.UserId != userId)
                return new GeneralResponse<InvoiceReadDTO> { Success = false, Message = "Invoice not found" };

            invoice.Value -= invoiceItem.LineTotal;

            invoiceItem.Quantity = newQuantity;

            invoice.Value += invoiceItem.LineTotal;
            invoice.FinalValue = invoice.Value;

            await _invoiceItemRepo.UpdateAsync(invoiceItem);
            await _invoiceRepo.UpdateAsync(invoice);

            return new GeneralResponse<InvoiceReadDTO>
            {
                Success = true,
                Message = "Invoice item updated",
                Data = _mapper.Map<InvoiceReadDTO>(invoice)
            };
        }

        public async Task<GeneralResponse<bool>> RemoveInvoiceItemAsync(string invoiceItemId, string userId)
        {
            var invoiceItem = await _invoiceItemRepo.GetByIdAsync(invoiceItemId);
            if (invoiceItem == null)
                return new GeneralResponse<bool> { Success = false, Message = "Invoice item not found" };

            var invoice = await _invoiceRepo.GetByIdAsync(invoiceItem.InvoiceId);
            if (invoice == null || invoice.UserId != userId)
                return new GeneralResponse<bool> { Success = false, Message = "Invoice not found" };

            invoice.Value -= invoiceItem.LineTotal;
            invoice.FinalValue = invoice.Value;

            await _invoiceItemRepo.DeleteAsync(invoiceItemId);
            await _invoiceRepo.UpdateAsync(invoice);

            return new GeneralResponse<bool> { Success = true, Message = "Invoice item removed", Data = true };
        }

        public async Task<GeneralResponse<InvoiceReadDTO>> FinalizeSaleAsync(string invoiceId, string paymentMethodId, decimal paidAmount, string userId)
        {
            var invoice = await _invoiceRepo.GetByIdAsync(invoiceId);
            if (invoice == null || invoice.UserId != userId)
                return new GeneralResponse<InvoiceReadDTO> { Success = false, Message = "Invoice not found" };

            var paymentMethod = await _paymentMethodRepo.GetByIdAsync(paymentMethodId);
            if (paymentMethod == null)
                return new GeneralResponse<InvoiceReadDTO> { Success = false, Message = "Invalid payment method" };

            var payment = new Payment
            {
                InvoiceId = invoiceId,
                Cost = paidAmount,
                PaymentMethodId = paymentMethodId,
                Status = PaymentStatus.Completed
            };

            invoice.InvoiceStatus = InvoiceStatus.Paid;
            invoice.FinalValue = invoice.Value;

            await _paymentRepo.AddAsync(payment);
            await _invoiceRepo.UpdateAsync(invoice);

            return new GeneralResponse<InvoiceReadDTO>
            {
                Success = true,
                Message = "Sale finalized successfully",
                Data = _mapper.Map<InvoiceReadDTO>(invoice)
            };
        }

        public async Task<GeneralResponse<bool>> CancelSaleAsync(string invoiceId, string userId)
        {
            var invoice = await _invoiceRepo.GetByIdAsync(invoiceId);
            if (invoice == null || invoice.UserId != userId)
                return new GeneralResponse<bool> { Success = false, Message = "Invoice not found" };

            invoice.InvoiceStatus = InvoiceStatus.Cancelled;
            await _invoiceRepo.UpdateAsync(invoice);

            return new GeneralResponse<bool> { Success = true, Message = "Sale cancelled", Data = true };
        }

        public async Task<GeneralResponse<bool>> RefundInvoiceAsync(string invoiceId, string userId)
        {
            var invoice = await _invoiceRepo.GetByIdAsync(invoiceId);
            if (invoice == null || invoice.UserId != userId)
                return new GeneralResponse<bool> { Success = false, Message = "Invoice not found" };

            invoice.InvoiceStatus = InvoiceStatus.Refunded;
            await _invoiceRepo.UpdateAsync(invoice);

            return new GeneralResponse<bool> { Success = true, Message = "Invoice refunded", Data = true };
        }

        public async Task<GeneralResponse<IEnumerable<InvoiceReadDTO>>> GetPOSInvoicesAsync(string storeId, DateTime? date, string userId)
        {
            var invoices = await _invoiceRepo.QueryAsync(i =>
                //i.StoreId == storeId &&
                i.UserId == userId &&
                (!date.HasValue || i.CreatedAt.Date == date.Value.Date));

            return new GeneralResponse<IEnumerable<InvoiceReadDTO>>
            {
                Success = invoices.Any(),
                Message = invoices.Any() ? "Invoices retrieved" : "No invoices found",
                Data = _mapper.Map<IEnumerable<InvoiceReadDTO>>(invoices)
            };
        }

        public async Task<GeneralResponse<decimal>> GetDailySalesTotalAsync(string storeId, DateTime date, string userId)
        {
            var invoices = await _invoiceRepo.QueryAsync(i =>
              //  i.StoreId == storeId &&
                i.UserId == userId &&
                i.InvoiceStatus == InvoiceStatus.Paid &&
                i.CreatedAt.Date == date.Date);

            var total = invoices.Sum(i => i.FinalValue);

            return new GeneralResponse<decimal>
            {
                Success = true,
                Message = "Daily sales total retrieved",
                Data = total
            };
        }
    }
}
