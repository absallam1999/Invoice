using AutoMapper;
using invoice.Core.DTO.Payment;
using invoice.Core.DTO.PaymentResponse;
using invoice.Core.Entities;

namespace invoice.Services.Mappers
{
    public class PaymentProfile : Profile
    {
        public PaymentProfile()
        {
            CreateMap<Payment, PaymentReadDTO>().ReverseMap();
            CreateMap<PaymentCreateDTO, Payment>();
            CreateMap<PaymentUpdateDTO, Payment>();

            CreateMap<PaymentSessionResponse, PaymentReadDTO>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.PaymentType.ToString()))
                .ForMember(dest => dest.Link, opt => opt.MapFrom(src => src.PaymentUrl))
                .ForMember(dest => dest.GatewaySessionId, opt => opt.MapFrom(src => src.SessionId))
                .ForMember(dest => dest.Cost, opt => opt.MapFrom(src => src.Amount))
                .ForMember(dest => dest.Currency, opt => opt.MapFrom(src => src.Currency))
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.PaymentStatus))
                .ForMember(dest => dest.Type, opt => opt.MapFrom(src => src.PaymentType))
                .ForMember(dest => dest.InvoiceId, opt => opt.MapFrom(src => src.InvoiceId))
                .ForMember(dest => dest.ExpiresAt, opt => opt.MapFrom(src => src.ExpiresAt));
        }
    }
}
