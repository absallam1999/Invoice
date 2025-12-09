using AutoMapper;
using System.Reflection;
using invoice.Core.DTO.Category;
using invoice.Core.Entities;

namespace invoice.Services.Mappers
{
    public class CategoryProfile : Profile
    {
        public CategoryProfile()
        {
            CreateMap<Category, CategoryReadDTO>();

            CreateMap<CategoryCreateDTO, Category>();
            CreateMap<CategoryUpdateDTO, Category>();
        }
    }
}