namespace invoice.Core.DTO.PaymentOptions
{
    public class PaymentOptionsDTO
    {

        public bool Cash { get; set; } 
        public bool BankTransfer { get; set; } 
        public bool PayPal { get; set; } 

        public bool Tax { get; set; } 
    }
}
