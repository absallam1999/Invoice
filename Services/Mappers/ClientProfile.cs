using AutoMapper;
using invoice.Core.DTO.Client;
using invoice.Core.Entites;

namespace invoice.Services.Mappers
{
    public class ClientProfile : Profile
    {
        public ClientProfile()
        {
            CreateMap<Client, ClientReadDTO>()
                .ForMember(dest => dest.InvoiceCount, opt => opt.MapFrom(src => src.Invoices.Count));

            CreateMap<ClientCreateDTO, Client>();
            CreateMap<ClientUpdateDTO, Client>();
        }
    }
}
