using AutoMapper;
using invoice.Core.DTO.Product;
using invoice.Core.Entites;

namespace invoice.Services.Mappers
{
    using AutoMapper;
    using invoice.Core.Entites;
    using invoice.Core.DTO;
    using invoice.Core.Enums;

    public class ProductProfile : Profile
    {
        public ProductProfile()
        {
            CreateMap<ProductCreateDTO, Product>();
            CreateMap<ProductUpdateDTO, Product>();

            CreateMap<Product, ProductReadDTO>()
                .ForMember(dest => dest.CategoryName,
                           opt => opt.MapFrom(src => src.Category != null ? src.Category.Name : string.Empty))
                //.ForMember(dest => dest.StoreName,
                //           opt => opt.MapFrom(src => src.Store != null ? src.Store.Name : string.Empty))
                .ForMember(dest => dest.CategoryId,
                           opt => opt.MapFrom(src => src.CategoryId))
              .ForMember(dest => dest.CategoryName,
                              opt => opt.MapFrom(src => src.Category != null ? src.Category.Name : string.Empty))
                //.ForMember(dest => dest.StoreId,
                //           opt => opt.MapFrom(src => src.StoreId))
               .ForMember(dest => dest.NumberOfSales,                 
                          opt => opt.MapFrom(src => src.InvoiceItems != null? src.InvoiceItems
                           .Where(i => i.Invoice != null && i.Invoice.InvoiceStatus == InvoiceStatus.Paid)
                           .Count() : 0))
               //           opt => opt.MapFrom(src => src.StoreId))
               .ForMember(dest => dest.TotalOfSales,
                          opt => opt.MapFrom(src => src.InvoiceItems != null ? src.InvoiceItems
                          .Where(i => i.Invoice != null && i.Invoice.InvoiceStatus == InvoiceStatus.Paid)
                          .Sum(i=>i.LineTotal) : 0))

               .ForMember(dest => dest.Invoices,
               opt => opt.MapFrom(src => src.InvoiceItems.Select(i => i.Invoice))
                          );

            CreateMap<Product, GetAllProductDTO>()
                   .ForMember(dest => dest.CategoryName,
                              opt => opt.MapFrom(src => src.Category != null ? src.Category.Name : string.Empty));


        }
    }
}

