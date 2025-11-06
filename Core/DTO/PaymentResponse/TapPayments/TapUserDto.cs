namespace invoice.Core.DTO.PaymentResponse.TapPayments
{
    public class TapUserDto
    {
        public string Email { get; set; }
        public string BusinessName { get; set; }
        public string Country { get; set; } = "SA";
        public string CountryCode { get; set; } = "+966";
        public string PhoneNumber { get; set; }


    }
}
