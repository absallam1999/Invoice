using System.ComponentModel.DataAnnotations;
using invoice.DTO.InvoiceItem;
using invoice.Models.Enums;

namespace invoice.DTO.Invoice
{
    public class InvoiceInfoDTO
    {
        [Required]
     //   public string Code { get; set; }

       
        public DateTime CreateAt { get; set; }

        //public string TaxNumber { get; set; }

      //  public decimal Value { get; set; }

        //public string Description { get; set; }


        //[Required]
       // public InvoiceStatus InvoiceStatus { get; set; }

        [Required]
        public InvoiceType InvoiceType { get; set; }

       // [Required]
       // public string UserId { get; set; }

        [Required]
        public string StoreId { get; set; }

        [Required]
        public string ClientId { get; set; }

        [Required]
        public string LanguageId { get; set; }

        public string TermsConditions { get; set; }
        public DiscountType? DiscountType { get; set; }
        public decimal? DiscountValue { get; set; }
        public List<InvoiceItemDTO> Items { get; set; }

    }
}
