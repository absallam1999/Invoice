using AutoMapper;
using invoice.Core.DTO.ContactInformation;
using invoice.Core.Entites;

namespace invoice.Services.Mappers
{
    public class ContactInfoProfile : Profile
    {
        public ContactInfoProfile()
        {
            CreateMap<ContactInfoCreateDTO, ContactInfo>();

            CreateMap<ContactInfo, ContactInfoReadDTO>()
                .ForMember(dest => dest.StoreName,
                           opt => opt.MapFrom(src => src.Store != null ? src.Store.Name : null));

            CreateMap<ContactInfoUpdateDTO, ContactInfo>()
                .ForMember(dest => dest.Id, opt => opt.Ignore());
        }
    }
}
