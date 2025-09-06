using AutoMapper;
using invoice.Core.DTO.Order;
using invoice.Core.Entites;
using invoice.Core.Enums;

namespace invoice.Repo.MappingProfiles
{
    public class OrderProfile : Profile
    {
        public OrderProfile()
        {
            CreateMap<Order, OrderReadDTO>()
                .ForMember(dest => dest.OrderStatus, opt => opt.MapFrom(src => src.OrderStatus.ToString()))
                .ForMember(dest => dest.StoreName, opt => opt.MapFrom(src => src.Store != null ? src.Store.Name : null))
                .ForMember(dest => dest.ClientName, opt => opt.MapFrom(src => src.Client != null ? src.Client.Name : null))
                .ForMember(dest => dest.InvoiceFinalValue, opt => opt.MapFrom(src => src.Invoice != null ? src.Invoice.FinalValue : (decimal?)null));

            CreateMap<OrderItem, OrderItemReadDTO>()
                .ForMember(dest => dest.ProductName, opt => opt.MapFrom(src => src.Product != null ? src.Product.Name : null));

            CreateMap<OrderCreateDTO, Order>()
                .ForMember(dest => dest.OrderStatus, opt => opt.MapFrom(src => OrderStatus.Pending))
                .ForMember(dest => dest.OrderItems, opt => opt.MapFrom(src => src.OrderItems));

            CreateMap<OrderItemCreateDTO, OrderItem>();

            CreateMap<OrderUpdateDTO, Order>()
                .ForMember(dest => dest.OrderItems, opt => opt.MapFrom(src => src.OrderItems));

            CreateMap<OrderItemUpdateDTO, OrderItem>();
        }
    }
}
