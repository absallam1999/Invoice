using AutoMapper;
using invoice.Core.DTO.Product;
using invoice.Core.Entites;

namespace invoice.Services.Mappers
{
    using AutoMapper;
    using invoice.Core.Entites;
    using invoice.Core.DTO;

    public class ProductProfile : Profile
    {
        public ProductProfile()
        {
            CreateMap<ProductCreateDTO, Product>();
            CreateMap<ProductUpdateDTO, Product>();

            CreateMap<Product, ProductReadDTO>()
                .ForMember(dest => dest.CategoryName,
                           opt => opt.MapFrom(src => src.Category != null ? src.Category.Name : string.Empty))
                .ForMember(dest => dest.StoreName,
                           opt => opt.MapFrom(src => src.Store != null ? src.Store.Name : string.Empty))
                .ForMember(dest => dest.InvoiceItems,
                           opt => opt.MapFrom(src => src.InvoiceItems ?? new List<InvoiceItem>()))
                .ForMember(dest => dest.CategoryId,
                           opt => opt.MapFrom(src => src.CategoryId))
                .ForMember(dest => dest.StoreId,
                           opt => opt.MapFrom(src => src.StoreId));

            CreateMap<InvoiceItem, InvoiceItemSummaryDTO>()
                .ForMember(dest => dest.LineTotal,
                           opt => opt.MapFrom(src => src.Quantity * src.UnitPrice));
        }
    }

}

