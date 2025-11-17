using AutoMapper;
using invoice.Core.DTO.Page;
using invoice.Core.Entities;

namespace invoice.Services.Mappers
{
    public class PageProfile : Profile
    {
        public PageProfile()
        {
            CreateMap<Page, PageReadDTO>();

            CreateMap<Page, GetAllPagesDTO>();
            CreateMap<PageCreateDTO, Page>();
            CreateMap<PageUpdateDTO, Page>();
        }
    }
}