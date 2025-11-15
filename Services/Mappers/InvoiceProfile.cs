using AutoMapper;
using invoice.Core.DTO.Client;
using invoice.Core.DTO.Invoice;
using invoice.Core.DTO.InvoiceItem;
using invoice.Core.DTO.Language;
using invoice.Core.DTO.Payment;
using invoice.Core.DTO.PaymentLink;
using invoice.Core.DTO.Store;
using invoice.Core.DTO.Tax;
using invoice.Core.Entities;

namespace invoice.Services.Mappers
{
    public class InvoiceProfile : Profile
    {
        public InvoiceProfile()
        {
            CreateMap<Invoice, InvoiceReadDTO>()
                .ForMember(dest => dest.Client, opt => opt.MapFrom(src => src.Client))
                .ForMember(dest => dest.PayAt, opt => opt.MapFrom(src => src.PayInvoice.PaidAt))
                .ForMember(dest => dest.Language, opt => opt.MapFrom(src => src.Language))
                .ForMember(dest => dest.InvoiceItems, opt => opt.MapFrom(src => src.InvoiceItems))
                .ForMember(dest => dest.TaxInfo, opt => opt.MapFrom(src => src.Tax))
                .ForMember(dest => dest.Order, opt => opt.MapFrom(src => src.Order))
                .ForMember(dest => dest.PaymentLinkPayment, opt => opt.MapFrom(src => src.PaymentLinkPayment))
             //   .ForMember(dest => dest.Payments, opt => opt.MapFrom(src => src.Payments))
                ;

            //CreateMap<Tax, TaxReadDTO>();


            CreateMap<Invoice, InvoicewithUserDTO>()
                .ForMember(dest => dest.CurrencyInfo, opt => opt.MapFrom(src => src.User.Currency));


            CreateMap<Invoice, GetAllInvoiceDTO>()

                    .ForMember(dest => dest.ClientId, opt => opt.MapFrom(src => src.Client.Id))
                    .ForMember(dest => dest.ClientName, opt => opt.MapFrom(src => src.Client.Name))
                    .ForMember(dest => dest.OrderStatus, opt => opt.MapFrom(src => src.Order.OrderStatus));

            CreateMap<InvoiceCreateDTO, Invoice>()
                .ForMember(dest => dest.InvoiceItems, opt => opt.MapFrom(src => src.InvoiceItems))
                .ReverseMap();

            CreateMap<InvoiceUpdateDTO, Invoice>()
                .ForMember(dest => dest.InvoiceItems, opt => opt.Ignore())
                .ForMember(dest => dest.Payments, opt => opt.Ignore())
                .ReverseMap();

            CreateMap<InvoiceItem, InvoiceItemReadDTO>().ReverseMap();
            CreateMap<InvoiceItemCreateDTO, InvoiceItem>().ReverseMap();

            CreateMap<Payment, PaymentReadDTO>().ReverseMap();
            CreateMap<PaymentCreateDTO, Payment>().ReverseMap();

            CreateMap<PaymentLink, PaymentLinkReadDTO>().ReverseMap();
            CreateMap<PaymentLinkCreateDTO, PaymentLink>().ReverseMap();

            CreateMap<Client, ClientReadDTO>().ReverseMap();
            CreateMap<Client, ClientSummaryDTO>().ReverseMap();
            CreateMap<Store, StoreReadDTO>().ReverseMap();
            CreateMap<Language, LanguageReadDTO>().ReverseMap();
            CreateMap<Tax, TaxReadDTO>().ReverseMap();


        }
    }
}