namespace invoice.Core.DTO.Tax
{
    public class TaxReadDTO
    {
        public string TaxNumber { get; set; }
        public string TaxName { get; set; }
        public decimal Value { get; set; }
    }
}