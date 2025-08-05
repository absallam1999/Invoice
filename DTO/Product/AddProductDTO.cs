using invoice.Custom_Validation;
using System.ComponentModel.DataAnnotations;

namespace invoice.DTO.Product
{
    public class AddProductDTO
    {
        [MaxLength(100)]
        public string Name { get; set; }
     
        [Range(0.01, double.MaxValue, ErrorMessage = "Price must be greater than 0.")]
        public decimal Price { get; set; }
        public bool? InProductList { get; set; }=true;

        
    }
}
