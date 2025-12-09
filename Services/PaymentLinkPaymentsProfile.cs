using AutoMapper;
using invoice.Core.DTO.Invoice;
using invoice.Core.Entities;

namespace invoice.Services
{
    public class PaymentLinkPaymentsProfile:Profile
    {
        public PaymentLinkPaymentsProfile()
        {
            CreateMap<PaymentLinkPayments, PaymentLinkPaymentsDTO>().ReverseMap();

        }

    }
}
