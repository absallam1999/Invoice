using AutoMapper;
using invoice.Core.DTO.Client;
using invoice.Core.Entities;

namespace invoice.Services.Mappers
{
    public class ClientProfile : Profile
    {
        public ClientProfile()
        {
            CreateMap<Client, ClientReadDTO>()
                .ForMember(dest => dest.InvoiceCount, opt => opt.MapFrom(src => src.Invoices.Count))
                .ForMember(dest => dest.InvoiceTotal, opt => opt.MapFrom(src => src.Invoices.Sum(i => i.FinalValue)));

            CreateMap<ClientCreateDTO, Client>();
            CreateMap<ClientUpdateDTO, Client>();
            CreateMap<Client, GetAllClientsDTO>()
                .ForMember(dest => dest.InvoiceCount, opt => opt.MapFrom(src => src.Invoices.Count))

                ;

        }
    }
}