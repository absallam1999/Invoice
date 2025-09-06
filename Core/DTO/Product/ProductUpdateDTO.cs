using Microsoft.AspNetCore.Mvc;

namespace invoice.Core.DTO.Product
{
    public class ProductUpdateDTO
    {
       // public string Id { get; set; }
        public string Name { get; set; }
        public string? MainImage { get; set; }
        public List<string>? Images { get; set; }
        public decimal Price { get; set; }
        public int Quantity { get; set; }

       // public bool InProductList { get; set; }
        public bool InPOS { get; set; }
        public bool InStore { get; set; }

        public string? CategoryId { get; set; }
       // public string? StoreId { get; set; }
        public string? Url { get; set; }
    }

    public class ProductUpdateRequest
    {
        public string Id { get; set; }
        public string Name { get; set; }

        [FromForm(Name = "mainImage")]
        public IFormFile? MainImage { get; set; }
        
        [FromForm(Name = "images")]
        public List<IFormFile>? Images { get; set; }
        
        public decimal Price { get; set; }
        public int Quantity { get; set; }
        public bool InPOS { get; set; }
        public bool InStore { get; set; }
        public string? CategoryId { get; set; }
        public string? Url { get; set; }
    }
    
    public class ProductUpdateRangeDTO
    {
        [FromForm]
        public List<ProductUpdateRequest> Products { get; set; }
    }
}
