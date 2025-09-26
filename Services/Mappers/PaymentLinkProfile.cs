using AutoMapper;
using invoice.Core.DTO.PaymentLink;
using invoice.Core.DTO.Store;
using invoice.Core.Entities;

namespace invoice.Services.Mappers
{
    public class PaymentLinkProfile : Profile
    {
        public PaymentLinkProfile()
        {
            CreateMap<PaymentLink, PaymentLinkReadDTO>();

            CreateMap<PaymentLinkCreateDTO, PaymentLink>();

            CreateMap<PaymentLink, GetAllPaymentLinkDTO>();
            
            CreateMap<PaymentLinkUpdateDTO, PaymentLink>()
              .ForMember(dest => dest.PaymentOptions, opt => opt.MapFrom(src => src.PaymentOptions))
              .ForMember(dest => dest.purchaseOptions, opt => opt.MapFrom(src => src.purchaseOptions));




            CreateMap<PaymentLink, PaymentLinkReadDTO>()
                .ForMember(dest => dest.PaymentOptions, opt => opt.MapFrom(src => src.PaymentOptions))
                .ForMember(dest => dest.purchaseOptions, opt => opt.MapFrom(src => src.purchaseOptions));



        }
    }
}
