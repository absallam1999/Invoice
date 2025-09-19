using AutoMapper;
using invoice.Core.DTO.Shipping;
using invoice.Core.DTO.Store;
using invoice.Core.DTO.StoreSettings;
using invoice.Core.Entites;
using invoice.Core.Entites.utils;
using invoice.Models.Entites.utils;

namespace invoice.Services.Mappers
{
    public class StoreProfile : Profile
    {
        public StoreProfile()
        {
            CreateMap<StoreCreateDTO, Store>();

            CreateMap<StoreUpdateDTO, Store>()
                .ForAllMembers(opt => opt.Condition((src, dest, srcMember) => srcMember != null));

            CreateMap<Store, StoreReadDTO>()
                .ForMember(dest => dest.StoreSettings, opt => opt.MapFrom(src => src.StoreSettings))
                .ForMember(dest => dest.Shipping, opt => opt.MapFrom(src => src.Shipping));

            CreateMap<StoreSettings, StoreSettingsReadDTO>();
            CreateMap<Shipping, ShippingReadDTO>();

            CreateMap<StoreSettingsReadDTO, StoreSettings>()
                .ForMember(dest => dest.purchaseOptions, opt => opt.MapFrom(src => src.PurchaseOptions));

            CreateMap<ShippingReadDTO, Shipping>();
        }
    }
}
