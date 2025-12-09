using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace invoice.Core.DTO.Product
{
    public class ProductUpdateDTO
    {
        [Required]
        public string Name { get; set; }
        [FromForm(Name = "mainImage")]
        public IFormFile? MainImage { get; set; }


        [Required]
        public decimal Price { get; set; }
        public string? Quantity { get; set; }

        public bool InPOS { get; set; }
        public bool InStore { get; set; }

        public string? CategoryId { get; set; }
       
    }
}