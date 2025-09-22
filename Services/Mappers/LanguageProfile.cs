using AutoMapper;
using invoice.Core.DTO.Language;
using invoice.Core.Entities;

namespace invoice.Services.Mappers
{
    public class LanguageProfile : Profile
    {
        public LanguageProfile()
        {
            CreateMap<Language, LanguageReadDTO>();

            CreateMap<CreateLanguageDTO, Language>();

            CreateMap<UpdateLanguageDTO, Language>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id));
        }
    }
}