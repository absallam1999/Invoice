using AutoMapper;
using invoice.Core.DTO.Product;
using invoice.Core.Entities;
using invoice.Core.Enums;
namespace invoice.Services.Mappers
{
    

    public class ProductProfile : Profile
    {
        public ProductProfile()
        {
            CreateMap<ProductCreateDTO, Product>();
            CreateMap<ProductUpdateDTO, Product>();

            CreateMap<Product, ProductReadDTO>()

              .ForMember(dest => dest.CategoryName,
                              opt => opt.MapFrom(src => src.Category != null ? src.Category.Name : string.Empty));
           

            CreateMap<Product, ProductWithInvoicesReadDTO>()
               .ForMember(dest => dest.CategoryId,
                          opt => opt.MapFrom(src => src.CategoryId))
             
              .ForMember(dest => dest.NumberOfSales,
                         opt => opt.MapFrom(src => src.InvoiceItems != null ? src.InvoiceItems
                          .Where(i => i.Invoice != null && i.Invoice.InvoiceStatus == InvoiceStatus.Paid)
                          .Count() : 0))
             
              .ForMember(dest => dest.TotalOfSales,
                         opt => opt.MapFrom(src => src.InvoiceItems != null ? src.InvoiceItems
                         .Where(i => i.Invoice != null && i.Invoice.InvoiceStatus == InvoiceStatus.Paid)
                         .Sum(i => i.LineTotal) : 0))

              .ForMember(dest => dest.Invoices,
              opt => opt.MapFrom(src => src.InvoiceItems.Select(i => i.Invoice))
                         );

            CreateMap<Product, GetAllProductDTO>()
                   .ForMember(dest => dest.CategoryName,
                              opt => opt.MapFrom(src => src.Category != null ? src.Category.Name : string.Empty));


        }
    }

}

