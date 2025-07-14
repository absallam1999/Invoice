namespace invoice.DTO.InvoiceItem
{
    public class InvoiceItemDetailsDTO
    {
        public string Id { get; set; }
        public string InvoiceId { get; set; }
        public string ProductId { get; set; }

        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal Subtotal { get; set; }
    }
}
