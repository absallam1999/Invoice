namespace invoice.Core.DTO.Product
{
    public class ProductCreateDTO
    {
        public string Name { get; set; }
        public IFormFile? Image { get; set; }
        public decimal Price { get; set; }
        public string? Quantity { get; set; }
        public bool InPOS { get; set; } = true;
        public bool InStore { get; set; } = true;

        public string? CategoryId { get; set; }
        
    }
}