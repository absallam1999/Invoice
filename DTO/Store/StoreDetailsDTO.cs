using invoice.DTO.Invoice;
using invoice.DTO.Product;
using invoice.DTO.PurchaseCompletionOptions;

namespace invoice.DTO.Store
{
    public class StoreDetailsDTO
    {
            public string Id { get; set; }
            public string Name { get; set; }
            public string Description { get; set; }
            public string Url { get; set; }
            public string Logo { get; set; }
            public string CoverImage { get; set; }
            public string Color { get; set; }
            public string Currency { get; set; }
            public bool IsActivated { get; set; }
            public string UserId { get; set; }

            public IEnumerable<ProductDetailsDTO> Products { get; set; }
            public IEnumerable<InvoiceDetailsDTO> Invoices { get; set; }
            public IEnumerable<ContactInformationDetailsDTO> ContactInformations { get; set; }
            public PurchaseCompletionOptionsDTO PurchaseCompletionOptions { get; set; }
    }
}
