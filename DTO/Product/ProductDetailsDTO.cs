namespace invoice.DTO.Product
{
    public class ProductDetailsDTO
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Image { get; set; }
        public decimal Price { get; set; }
        public int Quantity { get; set; }

        public string CategoryId { get; set; }
        public string CategoryName { get; set; }

        public string StoreId { get; set; }
        public string StoreName { get; set; }
    }
}