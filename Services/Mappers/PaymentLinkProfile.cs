using AutoMapper;
using invoice.Core.DTO.PaymentLink;
using invoice.Core.Entites;

namespace invoice.Services.Mappers
{
    public class PaymentLinkProfile : Profile
    {
        public PaymentLinkProfile()
        {
            CreateMap<PaymentLink, PaymentLinkReadDTO>().ReverseMap();
            CreateMap<PaymentLinkCreateDTO, PaymentLink>();
            CreateMap<PaymentLinkUpdateDTO, PaymentLink>();
        }
    }
}
