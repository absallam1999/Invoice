using AutoMapper;
using invoice.Core.DTO.PaymentMethod;
using invoice.Core.Entities;

namespace invoice.Services.Mappers
{
    public class PaymentMethodProfile:Profile
    {
        
        public PaymentMethodProfile()
        {
            CreateMap<PaymentMethod, PaymentMethodReadDTO>().ReverseMap();


        }
    }
}
