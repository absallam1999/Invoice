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


            CreateMap<PaymentLink, PaymentLinkWithUserDTO>()
              .ForMember(dest => dest.PaymentOptions, opt => opt.MapFrom(src => src.PaymentOptions))
              .ForMember(dest => dest.PurchaseOptions, opt => opt.MapFrom(src => src.purchaseOptions))
              .ForMember(dest => dest.Tax, opt => opt.MapFrom(src => src.User.Tax))

                .ForMember(
                    dest => dest.RemainingPaymentsNumber,
                    opt => opt.MapFrom(src =>
                        src.MaxPaymentsNumber == null
                            ? (int?)null
                            : src.MaxPaymentsNumber - src.PaymentsNumber
                    )
                );





            CreateMap<PaymentLink, PaymentLinkReadDTO>()
                .ForMember(dest => dest.PaymentOptions, opt => opt.MapFrom(src => src.PaymentOptions))
                .ForMember(dest => dest.purchaseOptions, opt => opt.MapFrom(src => src.purchaseOptions))
                .ForMember(dest => dest.Invoices, opt => opt.MapFrom(src =>
                 src.PaymentLinkPayments.Select(p => p.Invoice))); 
            ;

            CreateMap<CreatePaymentDTO, PaymentLinkPayments>();


        }
    }
}
