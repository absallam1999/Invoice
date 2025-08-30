using AutoMapper;
using invoice.Core.DTO.PaymentMethod;
using invoice.Core.Entites;

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
