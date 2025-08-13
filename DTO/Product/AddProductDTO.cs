using invoice.Custom_Validation;
using System.ComponentModel.DataAnnotations;

namespace invoice.DTO.Product
{
    public class AddProductDTO
    {
        [Required(ErrorMessage = "Name is required.")]
        [MaxLength(100, ErrorMessage = "Name cannot be longer than 100 characters.")]
        [RegularExpression(@"^(?!\s*$).+", ErrorMessage = "Name cannot be empty or whitespace.")]
        public string Name { get; set; }
     
        [Range(0.01, double.MaxValue, ErrorMessage = "Price must be greater than 0.")]
        public decimal Price { get; set; }
        //public bool? InProductList { get; set; }=true;

        
    }
}
