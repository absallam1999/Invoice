using AutoMapper;
using invoice.Core.DTO;
using invoice.Core.DTO.Page;
using invoice.Core.Entities;
using invoice.Core.Interfaces.Services;
using Microsoft.EntityFrameworkCore;
using invoice.Repo;

namespace invoice.Services
{
    public class PageService : IPageService
    {
        private readonly IRepository<Page> _pageRepo;
        private readonly IFileService _fileService;
        private readonly IMapper _mapper;

        public PageService(IRepository<Page> pageRepo, IFileService fileService, IMapper mapper)
        {
            _pageRepo = pageRepo;
            _fileService = fileService;
            _mapper = mapper;
        }

        public async Task<GeneralResponse<IEnumerable<GetAllPagesDTO>>> GetAllAsync(string userId)
        {

            var pages = await _pageRepo.GetAllAsync(userId);
            var dtos = _mapper.Map<IEnumerable<GetAllPagesDTO>>(pages);

            return new GeneralResponse<IEnumerable<GetAllPagesDTO>>(true, "Pages retrieved successfully", dtos);

        }

        public async Task<GeneralResponse<PageReadDTO>> GetByIdAsync(string id)
        {
            var page = await _pageRepo.GetByIdAsync(id);
            if (page == null)
                return new GeneralResponse<PageReadDTO>() { Success = false, Message = "Page not found." };
            var readDto = _mapper.Map<PageReadDTO>(page);

            return new GeneralResponse<PageReadDTO>() { Success = true, Data = readDto, Message = "Page retrieved successfully." };
        }

        public async Task<GeneralResponse<PageReadDTO>> CreateAsync(PageCreateDTO dto, string StoreId)
        {
            var page = _mapper.Map<Page>(dto);
            page.StoreId = StoreId;

            if (dto.Image != null)
                page.Image = await _fileService.UploadImageAsync(dto.Image, "pages");

            var response = await _pageRepo.AddAsync(page);
            var readDto = _mapper.Map<PageReadDTO>(response.Data);


            return response.Success
                ? new GeneralResponse<PageReadDTO>() { Success = true, Message = "Page created successfully.", Data = readDto }
                : new GeneralResponse<PageReadDTO>() { Success = false, Message = response.Message };
        }

        public async Task<GeneralResponse<IEnumerable<PageReadDTO>>> CreateRangeAsync(IEnumerable<PageCreateDTO> dtos, string storeId)
        {
            var pages = new List<Page>();

            foreach (var dto in dtos)
            {
                var page = _mapper.Map<Page>(dto);
                page.StoreId = storeId;

                if (dto.Image != null)
                    page.Image = await _fileService.UploadImageAsync(dto.Image, "pages");

                pages.Add(page);
            }

            var response = await _pageRepo.AddRangeAsync(pages);

            return response.Success
                ? new GeneralResponse<IEnumerable<PageReadDTO>>
                {
                    Success = true,
                    Message = "Pages created successfully.",
                    Data = _mapper.Map<IEnumerable<PageReadDTO>>(response.Data)
                }
                : new GeneralResponse<IEnumerable<PageReadDTO>>
                {
                    Success = false,
                    Message = response.Message
                };
        }

        public async Task<GeneralResponse<PageReadDTO>> UpdateAsync(string id, PageUpdateDTO dto)
        {
            var existing = await _pageRepo.GetByIdAsync(id);
            if (existing == null)
                return new GeneralResponse<PageReadDTO>(false, "Page not found.");

            _mapper.Map(dto, existing);

            if (dto.Image != null)
                existing.Image = await _fileService.UpdateImageAsync(dto.Image, existing.Image, "pages");

            var response = await _pageRepo.UpdateAsync(existing);

            if (!response.Success)
                return new GeneralResponse<PageReadDTO>(false, response.Message);

            var readDto = _mapper.Map<PageReadDTO>(response.Data);

            return new GeneralResponse<PageReadDTO>(true, "Page updated successfully.", readDto);
        }

        public async Task<GeneralResponse<IEnumerable<PageReadDTO>>> UpdateRangeAsync(IEnumerable<PageUpdateDTO> dtos)
        {
            var updatedPages = new List<Page>();

            foreach (var dto in dtos)
            {
                var existing = await _pageRepo.GetByIdAsync(dto.Id);
                if (existing != null)
                {
                    _mapper.Map(dto, existing);

                    if (dto.Image != null)
                        existing.Image = await _fileService.UpdateImageAsync(dto.Image, existing.Image, "pages");

                    updatedPages.Add(existing);
                }
            }

            if (!updatedPages.Any())
                return new GeneralResponse<IEnumerable<PageReadDTO>> { Success = false, Message = "No valid pages found to update." };

            var response = await _pageRepo.UpdateRangeAsync(updatedPages);

            return response.Success
                ? new GeneralResponse<IEnumerable<PageReadDTO>>
                {
                    Success = true,
                    Message = "Pages updated successfully.",
                    Data = _mapper.Map<IEnumerable<PageReadDTO>>(response.Data)
                }
                : new GeneralResponse<IEnumerable<PageReadDTO>>
                {
                    Success = false,
                    Message = response.Message
                };
        }

        public async Task<GeneralResponse<bool>> DeleteAsync(string id)
        {
            var existing = await _pageRepo.GetByIdAsync(id);
            if (existing == null)
                return new GeneralResponse<bool>() { Success = false, Message = "Page not found.", Data = false };

            var response = await _pageRepo.DeleteAsync(id);
            return new GeneralResponse<bool>() { Success = response.Success, Message = response.Message, Data = response.Success };
        }

        public async Task<GeneralResponse<Page>> GetByTitleAsync(string title, string storeId = null, string languageId = null)
        {
            var page = (await _pageRepo.QueryAsync(
                p => p.Title.Contains(title) &&
                     (storeId == null || p.StoreId == storeId),
                      p => p.Store
            )).FirstOrDefault();

            if (page == null)
                return new GeneralResponse<Page>() { Success = false, Message = "Page not found." };

            return new GeneralResponse<Page>() { Success = true, Data = page, Message = "Page retrieved successfully." };
        }

        public async Task<GeneralResponse<IEnumerable<Page>>> SearchAsync(string keyword, string storeId = null, string languageId = null)
        {
            var pages = await _pageRepo.QueryAsync(
                p => (p.Title.Contains(keyword) || p.Content.Contains(keyword)) &&
                     (storeId == null || p.StoreId == storeId),

                p => p.Store
            );

            return new GeneralResponse<IEnumerable<Page>>()
            {
                Success = true,
                Message = pages.Any() ? "Search results retrieved successfully." : "No pages found matching the keyword.",
                Data = pages
            };
        }

        public async Task<GeneralResponse<bool>> DeleteRangeAsync(IEnumerable<string> ids)
        {
            var response = await _pageRepo.DeleteRangeAsync(ids);
            return new GeneralResponse<bool>() { Success = response.Success, Message = response.Message, Data = response.Success };
        }

        public async Task<bool> ExistsAsync(string id)
        {
            return await _pageRepo.ExistsAsync(p => p.Id == id);
        }


        public async Task<int> CountAsync(string userId = null, string storeId = null)
        {
            var query = _pageRepo.GetQueryable();

            if (!string.IsNullOrEmpty(userId))
                query = query.Where(p => p.Store.UserId == userId);

            if (!string.IsNullOrEmpty(storeId))
                query = query.Where(p => p.StoreId == storeId);

            return await query.CountAsync();
        }
    }
}