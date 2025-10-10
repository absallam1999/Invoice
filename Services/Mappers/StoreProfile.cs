using AutoMapper;
using invoice.Core.DTO.ContactInformation;
using invoice.Core.DTO.PaymentOptions;
using invoice.Core.DTO.PurchaseCompletionOptions;
using invoice.Core.DTO.Shipping;
using invoice.Core.DTO.Store;
using invoice.Core.DTO.StoreSettings;
using invoice.Core.Entities;
using invoice.Core.Entities.utils;
using invoice.Models.DTO.PurchaseCompletionOptions;
using invoice.Models.Entities.utils;

namespace invoice.Services.Mappers
{
    public class StoreProfile : Profile
    {
        public StoreProfile()
        {
            CreateMap<StoreCreateDTO, Store>();

            CreateMap<StoreUpdateDTO, Store>()
                .ForAllMembers(opt => opt.Condition((src, dest, srcMember) => srcMember != null));

            CreateMap<StoreUpdateDTO, Store>()
              .ForMember(dest => dest.StoreSettings, opt => opt.MapFrom(src => src.StoreSettings))
              .ForMember(dest => dest.Shipping, opt => opt.MapFrom(src => src.Shipping))
              .ForMember(dest => dest.ContactInformations, opt => opt.MapFrom(src => src.ContactInfo))
              .ForMember(dest => dest.PaymentOptions, opt => opt.MapFrom(src => src.PaymentOptions));

            CreateMap<CreateOrderDTO, Order>();

            CreateMap<Store, StoreReadDTO>()
                .ForMember(dest => dest.StoreSettings, opt => opt.MapFrom(src => src.StoreSettings))
                .ForMember(dest => dest.Shipping, opt => opt.MapFrom(src => src.Shipping))
                .ForMember(dest => dest.ContactInfo, opt => opt.MapFrom(src => src.ContactInformations))
                .ForMember(dest => dest.PaymentOptions, opt => opt.MapFrom(src => src.PaymentOptions))
                .ForMember(dest => dest.Tax, opt => opt.MapFrom(src => src.User.Tax))
;

            CreateMap<StoreSettings, StoreSettingsReadDTO>().ReverseMap();
            CreateMap<StoreSettings, StoreSettingsUpdateDTO>().ReverseMap();

            CreateMap<Shipping, ShippingReadDTO>().ReverseMap();
            CreateMap<Shipping, ShippingUpdateDTO>().ReverseMap();

            CreateMap<PaymentOptions, PaymentOptionsDTO>().ReverseMap();

            CreateMap<ContactInfo, ContactInfoReadDTO>().ReverseMap();
            CreateMap<ContactInfo, ContactInfoUpdateDTO>().ReverseMap();

            CreateMap<PurchaseCompletionOptions, PurchaseCompletionOptionsReadDTO>();
            CreateMap<PurchaseCompletionOptionsCreateDTO, PurchaseCompletionOptions>();
            CreateMap<PurchaseCompletionOptionsUpdateDTO, PurchaseCompletionOptions>();
        }
    }

}