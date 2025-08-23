namespace invoice.Core.DTO.Product
{
    public class ProductUpdateDTO
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string? Image { get; set; }
        public decimal Price { get; set; }
        public int Quantity { get; set; }

        public bool InProductList { get; set; }
        public bool InPOS { get; set; }
        public bool InStore { get; set; }

        public string? CategoryId { get; set; }
        public string? StoreId { get; set; }
        public string? Url { get; set; }
    }
}
