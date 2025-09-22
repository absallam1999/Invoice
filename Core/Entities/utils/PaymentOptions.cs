namespace invoice.Core.Entities.utils
{
    public class PaymentOptions
    {
        public bool Cash { get; set; } = true;
        public bool BankTransfer { get; set; } = false;
        public bool PayPal { get; set; } = false;

        public bool Tax { get; set; } = false;


    }
}
