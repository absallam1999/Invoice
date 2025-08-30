namespace invoice.Core.DTO.Product
{
    public class ProductCreateDTO
    {
        public string Name { get; set; }
        public string? Image { get; set; }
        public decimal Price { get; set; }
        public int? Quantity { get; set; } = 0;

      //  public bool InProductList { get; set; } = true;
        public bool InPOS { get; set; } = true;
        public bool InStore { get; set; } = true;

       // public string UserId { get; set; }
        public string? CategoryId { get; set; }
       // public string? StoreId { get; set; }
       // public string? Url { get; set; }
    }
}
