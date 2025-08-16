namespace invoice.DTO.Product
{
    public class ProductDetailsDTO
    {
        public string ProductId { get; set; }
        public string Name { get; set; }
        public string? Image { get; set; }
        public decimal Price { get; set; }
        public int? Quantity { get; set; }
        public int NumberOfSales { get; set; }



    }
}