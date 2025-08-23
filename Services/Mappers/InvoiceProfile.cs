using AutoMapper;
using invoice.Core.DTO.Client;
using invoice.Core.DTO.Invoice;
using invoice.Core.DTO.InvoiceItem;
using invoice.Core.DTO.Language;
using invoice.Core.DTO.Payment;
using invoice.Core.DTO.PaymentLink;
using invoice.Core.DTO.Store;
using invoice.Core.Entites;

namespace invoice.Services.Mappers
{
    public class InvoiceProfile : Profile
    {
        public InvoiceProfile()
        {
            CreateMap<Invoice, InvoiceReadDTO>()
                .ForMember(dest => dest.Store, opt => opt.MapFrom(src => src.Store))
                .ForMember(dest => dest.Client, opt => opt.MapFrom(src => src.Client))
                .ForMember(dest => dest.Language, opt => opt.MapFrom(src => src.Language))
                .ForMember(dest => dest.InvoiceItems, opt => opt.MapFrom(src => src.InvoiceItems))
                .ForMember(dest => dest.Payments, opt => opt.MapFrom(src => src.Payments))
                .ForMember(dest => dest.PaymentLinks, opt => opt.MapFrom(src => src.PaymentLinks))
                .ReverseMap();

            CreateMap<InvoiceCreateDTO, Invoice>()
                .ForMember(dest => dest.InvoiceItems, opt => opt.MapFrom(src => src.InvoiceItems))
                .ForMember(dest => dest.Payments, opt => opt.MapFrom(src => src.Payments))
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
        }
    }

}
