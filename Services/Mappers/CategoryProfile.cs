using AutoMapper;
using System.Reflection;
using invoice.Core.DTO.Category;
using invoice.Core.Entites;

namespace invoice.Services.Mappers
{
    public class CategoryProfile : Profile
    {
        public CategoryProfile()
        {
            CreateMap<Category, CategoryReadDTO>();
                //.ForMember(dest => dest.ProductCount, opt => opt.MapFrom(src => src.Products.Count))
                //.ForMember(dest => dest.Products, opt => opt.MapFrom(src => src.Products));

            //CreateMap<Product, ProductSummaryDTO>();

            CreateMap<CategoryCreateDTO, Category>();
            CreateMap<CategoryUpdateDTO, Category>();
        }
    }
}
