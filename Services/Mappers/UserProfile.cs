using AutoMapper;
using invoice.Core.DTO.User;
using invoice.Core.Entities;

namespace invoice.Services.Mappers
{
    public class UserProfile:Profile
    {
        public UserProfile()
        {
            CreateMap<ApplicationUser, UserDTO>()
                                .ForMember(dest => dest.TaxNumber, opt => opt.MapFrom(src => src.Tax != null ? src.Tax.TaxNumber:string.Empty));
;
        }
    }
}
