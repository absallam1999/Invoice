using AutoMapper;
using invoice.Repo;
using invoice.Core.DTO;
using invoice.Core.Entities;
using invoice.Core.DTO.Category;
using Microsoft.EntityFrameworkCore;
using invoice.Core.Interfaces.Services;
using System.Reflection;

namespace invoice.Services
{
    public class CategoryService : ICategoryService
    {
        private readonly IRepository<ApplicationUser> _applicationUserRepo;
        private readonly IRepository<Category> _categoryRepo;
        private readonly IMapper _mapper;

        public CategoryService(IRepository<Category> categoryRepo, IRepository<ApplicationUser> applicationUserRepo, IMapper mapper)
        {
            _applicationUserRepo = applicationUserRepo;
            _categoryRepo = categoryRepo;
            _mapper = mapper;
        }

        public async Task<GeneralResponse<IEnumerable<CategoryReadDTO>>> GetAllAsync(string userId)
        {
            var categories = await _categoryRepo.GetAllAsync(userId, c => c.Products);
            var dto = _mapper.Map<IEnumerable<CategoryReadDTO>>(categories);
            return new GeneralResponse<IEnumerable<CategoryReadDTO>>(true, "Categories retrieved successfully", dto);
        }

        public async Task<GeneralResponse<CategoryReadDTO>> GetByIdAsync(string id, string userId)
        {
            var category = await _categoryRepo.GetByIdAsync(id, userId, q => q.Include(c => c.Products));
            if (category == null)
                return new GeneralResponse<CategoryReadDTO>(false, "Category not found");

            return new GeneralResponse<CategoryReadDTO>(true, "Category retrieved successfully", _mapper.Map<CategoryReadDTO>(category));
        }

        public async Task<GeneralResponse<IEnumerable<CategoryReadDTO>>> GetByUserIdAsync(string userId)
        {
            var categories = await _categoryRepo.GetByUserIdAsync(userId, q => q.Include(c => c.Products));

            if (categories == null || !categories.Any())
                return new GeneralResponse<IEnumerable<CategoryReadDTO>>(false, "No categories found for user");

            var mappedCategories = _mapper.Map<IEnumerable<CategoryReadDTO>>(categories);

            return new GeneralResponse<IEnumerable<CategoryReadDTO>>(
                true,
                "Categories retrieved successfully",
                mappedCategories
            );
        }

        public async Task<GeneralResponse<IEnumerable<CategoryReadDTO>>> QueryAsync(string userId, string keyword)
        {
            var categories = await _categoryRepo.QueryAsync(
                c => c.UserId == userId &&
                     (c.Name.Contains(keyword) || (c.Description ?? "").Contains(keyword)),
                c => c.Products
            );

            var dto = _mapper.Map<IEnumerable<CategoryReadDTO>>(categories);
            return new GeneralResponse<IEnumerable<CategoryReadDTO>>(true, "Query results", dto);
        }

        public async Task<bool> ExistsAsync(string id, string userId)
        {
            return await _categoryRepo.ExistsAsync(c => c.Id == id && c.UserId == userId);
        }

        public async Task<int> CountAsync(string userId)
        {
            return await _categoryRepo.CountAsync(userId);

        }

        public async Task<GeneralResponse<CategoryReadDTO>> CreateAsync(CategoryCreateDTO dto, string userId)
        {
            if (string.IsNullOrEmpty(userId))
                return new GeneralResponse<CategoryReadDTO>(false, "User not authenticated");

            var entity = _mapper.Map<Category>(dto);
            entity.UserId = userId;

            var response = await _categoryRepo.AddAsync(entity);
            if (!response.Success)
                return new GeneralResponse<CategoryReadDTO>(false, "Failed to create category");

            var readDto = _mapper.Map<CategoryReadDTO>(response.Data);
            return new GeneralResponse<CategoryReadDTO>(true, "Category created successfully", readDto);
        }


        public async Task<GeneralResponse<IEnumerable<CategoryReadDTO>>> CreateRangeAsync(IEnumerable<CategoryCreateDTO> dtos, string userId)
        {
            var entities = dtos.Select(dto =>
            {
                var cat = _mapper.Map<Category>(dto);
                cat.UserId = userId;
                return cat;
            }).ToList();

            var response = await _categoryRepo.AddRangeAsync(entities);
            if (!response.Success)
                return new GeneralResponse<IEnumerable<CategoryReadDTO>>(false, "Failed to create categories");

            var dto = _mapper.Map<IEnumerable<CategoryReadDTO>>(response.Data);
            return new GeneralResponse<IEnumerable<CategoryReadDTO>>(true, "Categories created successfully", dto);
        }

        public async Task<GeneralResponse<CategoryReadDTO>> UpdateAsync(string id, CategoryUpdateDTO dto, string userId)
        {
            var category = await _categoryRepo.GetByIdAsync(id, userId);
            if (category == null)
                return new GeneralResponse<CategoryReadDTO>(false, "Category not found");

            _mapper.Map(dto, category);
            category.UserId = userId;

            var response = await _categoryRepo.UpdateAsync(category);
            if (!response.Success)
                return new GeneralResponse<CategoryReadDTO>(false, "Failed to update category");

            return new GeneralResponse<CategoryReadDTO>(true, "Category updated successfully", _mapper.Map<CategoryReadDTO>(response.Data));
        }

        public async Task<GeneralResponse<IEnumerable<CategoryReadDTO>>> UpdateRangeAsync(IEnumerable<CategoryUpdateDTO> dtos, string userId)
        {
            ////var ids = dtos.Select(d => d.Id).ToList();
            //var categories = await _categoryRepo.QueryAsync(c => ids.Contains(c.Id) && c.UserId == userId);

            ////foreach (var dto in dtos)
            ////{
            ////    var category = categories.FirstOrDefault(c => c.Id == dto.Id);
            ////    if (category != null)
            ////        _mapper.Map(dto, category);
            ////}

            //var response = await _categoryRepo.UpdateRangeAsync(categories);
            ////if (!response.Success)
            return new GeneralResponse<IEnumerable<CategoryReadDTO>>(false, "Failed to update categories");

            //var dtoList = _mapper.Map<IEnumerable<CategoryReadDTO>>(response.Data);
            //  return new GeneralResponse<IEnumerable<CategoryReadDTO>>(true, "Categories updated successfully", dtoList);
        }

        public async Task<GeneralResponse<bool>> DeleteAsync(string id, string userId)
        {
            var category = await _categoryRepo.GetByIdAsync(id, userId);
            if (category == null)
                return new GeneralResponse<bool>(false, "Category not found");

            var response = await _categoryRepo.DeleteAsync(id);
            return new GeneralResponse<bool>(response.Success, response.Message, response.Success);
        }

        public async Task<GeneralResponse<bool>> DeleteRangeAsync(IEnumerable<string> ids, string userId)
        {
            var categories = await _categoryRepo.QueryAsync(c => ids.Contains(c.Id) && c.UserId == userId);
            if (!categories.Any())
                return new GeneralResponse<bool>(false, "No categories found to delete");

            var response = await _categoryRepo.DeleteRangeAsync(ids);
            return new GeneralResponse<bool>(response.Success, response.Message, response.Success);
        }
    }
}