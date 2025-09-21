using invoice.Core.DTO.Invoice;

namespace invoice.Core.DTO.Product
{
    public class ProductReadDTO
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string? Image { get; set; }
        public decimal Price { get; set; }
        public int Quantity { get; set; }
        public string? CategoryName { get; set; }
    
    }

   
}
