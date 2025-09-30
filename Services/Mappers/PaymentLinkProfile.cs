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

            CreateMap<PaymentLinkCreateDTO, PaymentLink>()
                .ForMember(dest => dest.Image, opt => opt.Ignore())
                .ForMember(dest => dest.PaymentsNumber, opt => opt.MapFrom(src => src.PaymentsNumber.ToString()));

            CreateMap<PaymentLinkUpdateDTO, PaymentLink>()
                .ForMember(dest => dest.Image, opt => opt.Ignore())
                .ForAllMembers(opt => opt.Condition((src, dest, srcMember) => srcMember != null));

            CreateMap<PaymentLink, GetAllPaymentLinkDTO>()
                .ForMember(dest => dest.PaymentsNumber, opt => opt.MapFrom(src => src.PaymentsNumber));

            CreateMap<PaymentLink, PaymentLinkWithUserDTO>()
                .ForMember(dest => dest.User, opt => opt.Ignore())
                .ForMember(dest => dest.Invoice, opt => opt.Ignore())
                .ForMember(dest => dest.PaymentOptions, opt => opt.Ignore());
        }
    }

}
