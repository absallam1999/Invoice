using AutoMapper;
using invoice.Core.DTO.Invoice;
using invoice.Core.Entities;

namespace invoice.Services.Mappers
{
    public class OrderProfile : Profile
    {
        public OrderProfile()
        {
            CreateMap<Order, OrderDTO>().ReverseMap();

        }


    }
}