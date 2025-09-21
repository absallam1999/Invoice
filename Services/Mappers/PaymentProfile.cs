using AutoMapper;
using invoice.Core.DTO.Payment;
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
        }
    }
}