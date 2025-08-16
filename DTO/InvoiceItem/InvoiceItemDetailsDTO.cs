namespace invoice.DTO.InvoiceItem
{
    public class InvoiceItemDetailsDTO
    {
        public string ProductId { get; set; }
        public string ProductName { get; set; }
        public string ProductImge{ get; set; }
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal Subtotal { get; set; }
    }
}
