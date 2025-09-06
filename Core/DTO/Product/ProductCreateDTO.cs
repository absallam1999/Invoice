using Microsoft.AspNetCore.Mvc;

namespace invoice.Core.DTO.Product
{
    public class ProductCreateDTO
    {
        public string Name { get; set; }

        public string? MainImage { get; set; }
        public List<string>? Images { get; set; }

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

    public class ProductCreateRequest
    {
        public string Name { get; set; }
        [FromForm(Name = "mainImage")]
        public IFormFile? MainImage { get; set; }
        [FromForm(Name = "images")]
        public List<IFormFile>? Images { get; set; }
        public decimal Price { get; set; }
        public int? Quantity { get; set; } = 0;
        public bool InPOS { get; set; } = true;
        public bool InStore { get; set; } = true;
        public string? CategoryId { get; set; }
    }

    public class ProductCreateRangeDTO
    {
        [FromForm]
        public List<ProductCreateRequest> Products { get; set; }
    }
}
