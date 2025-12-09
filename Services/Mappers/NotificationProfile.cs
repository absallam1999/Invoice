using AutoMapper;
using invoice.Core.DTO.Notification;
using invoice.Core.Entities;

namespace invoice.Services.Mappers
{
    public class NotificationProfile : Profile
    {
        public NotificationProfile()
        {
            CreateMap<Notification, NotificationReadDTO>().ReverseMap();
            CreateMap<NotificationCreateDTO, Notification>();
            CreateMap<NotificationUpdateDTO, Notification>();
        }
    }
}
