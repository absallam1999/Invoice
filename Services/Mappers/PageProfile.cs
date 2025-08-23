using AutoMapper;
using invoice.Core.DTO.Page;
using invoice.Core.Entites;

namespace invoice.Services.Mappers
{
    public class PageProfile : Profile
    {
        public PageProfile()
        {
            CreateMap<Page, PageReadDTO>()
                .ForMember(dest => dest.StoreName, opt => opt.MapFrom(src => src.Store != null ? src.Store.Name : null))
                .ForMember(dest => dest.LanguageName, opt => opt.MapFrom(src => src.Language != null ? src.Language.Name.ToString() : null));

            CreateMap<PageCreateDTO, Page>();
            CreateMap<PageUpdateDTO, Page>();
        }
    }
}
