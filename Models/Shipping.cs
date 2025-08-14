namespace invoice.Models
{
    public class Shipping
    {

        public bool FromStore { get; set; } = true;
        public bool Delivery { get; set; } = false;

        //Delivery fees

        public string StoreId { get; set; }
        public Store Store { get; set; }

    }
}
