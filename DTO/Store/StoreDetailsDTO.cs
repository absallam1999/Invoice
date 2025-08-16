using invoice.DTO.Invoice;
using invoice.DTO.Product;
using invoice.DTO.PurchaseCompletionOptions;

namespace invoice.DTO.Store
{
    public class StoreDetailsDTO
    {
            public string StoreId { get; set; }
            public string StoreName { get; set; }
            public string Description { get; set; }
            public string Url { get; set; }    //Slug 
            public bool IsActivated { get; set; } 
            public string Logo { get; set; }
            public string CoverImage { get; set; }
            public string Color { get; set; }
            public string StorePhone { get; set; }
            public string StoreEmail { get; set; }
            public string Storelocation { get; set; }
            public string StoreFacebook { get; set; }
            public string StoreWhatsApp { get; set; }
            public string StoreInstagram { get; set; }
            public string Currency { get; set; }
            public bool Arabic { get; set; } 
            public bool English { get; set; } 

            public bool Cash { get; set; } 
            public bool BankTransfer { get; set; } 
            public bool PayPal { get; set; }
            public bool Tax { get; set; }
           
          //  public string UserId { get; set; }

            public bool FromStore { get; set; } 
            public bool Delivery { get; set; }
        //Delivery

            public bool ClientName{ get; set; } 
            public bool ClientEmail { get; set; }
            public bool Clientphone { get; set; } 
            public string? TermsAndConditions { get; set; }




           public IEnumerable<ProductStore> Products { get; set; }
          
            public IEnumerable<ContactInformationDetailsDTO> ContactInformations { get; set; }
            public PurchaseCompletionOptionsDTO PurchaseCompletionOptions { get; set; }
    }
}
