using invoice.Models;

namespace invoice.DTO.Product
{
    public class GetAllProductDTO
    {

        public string ProductId { get; set; } 
        public string Name { get; set; }
        public string? Image { get; set; }
        public decimal Price { get; set; }
        public int? Quantity { get; set; }

        //type + cat
        

     
        
        

        
    }
}
