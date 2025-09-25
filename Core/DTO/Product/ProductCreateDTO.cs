namespace invoice.Core.DTO.Product
{
    public class ProductCreateDTO
    {
        public string Name { get; set; }
        public IFormFile? Image { get; set; }
        public decimal Price { get; set; }
        public int? Quantity { get; set; } = null;

        public bool InPOS { get; set; } = true;
        public bool InStore { get; set; } = true;

        public string? CategoryId { get; set; }
        
    }
}