using AutoMapper;
using invoice.Core.DTO;
using invoice.Core.Entites;
using invoice.Core.Interfaces.Services;
using invoice.Repo;
using Microsoft.EntityFrameworkCore;

namespace invoice.Services
{
    public class PageService : IPageService
    {
        private readonly IRepository<Page> _pageRepo;
        private readonly IMapper _mapper;

        public PageService(IRepository<Page> pageRepo, IMapper mapper)
        {
            _pageRepo = pageRepo;
            _mapper = mapper;
        }

        public async Task<GeneralResponse<IEnumerable<Page>>> GetAllAsync(string storeId = null, string languageId = null)
        {
            var pages = await _pageRepo.QueryAsync(
                p => (storeId == null || p.StoreId == storeId) &&
                     (languageId == null || p.LanguageId == languageId),
                p => p.Store,
                p => p.Language
            );

            return new GeneralResponse<IEnumerable<Page>>()
            {
                Success = true,
                Message = pages.Any() ? "Pages retrieved successfully." : "No pages found.",
                Data = pages
            };
        }

        public async Task<GeneralResponse<Page>> GetByIdAsync(string id)
        {
            var page = await _pageRepo.GetByIdAsync(id, include: q => q.Include(p => p.Store).Include(p => p.Language));
            if (page == null)
                return new GeneralResponse<Page>() { Success = false, Message = "Page not found." };

            return new GeneralResponse<Page>() { Success = true, Data = page, Message = "Page retrieved successfully." };
        }

        public async Task<GeneralResponse<Page>> GetByTitleAsync(string title, string storeId = null, string languageId = null)
        {
            var page = (await _pageRepo.QueryAsync(
                p => p.Title.Contains(title) &&
                     (storeId == null || p.StoreId == storeId) &&
                     (languageId == null || p.LanguageId == languageId),
                p => p.Store,
                p => p.Language
            )).FirstOrDefault();

            if (page == null)
                return new GeneralResponse<Page>() { Success = false, Message = "Page not found." };

            return new GeneralResponse<Page>() { Success = true, Data = page, Message = "Page retrieved successfully." };
        }

        public async Task<GeneralResponse<IEnumerable<Page>>> SearchAsync(string keyword, string storeId = null, string languageId = null)
        {
            var pages = await _pageRepo.QueryAsync(
                p => (p.Title.Contains(keyword) || p.Content.Contains(keyword)) &&
                     (storeId == null || p.StoreId == storeId) &&
                     (languageId == null || p.LanguageId == languageId),
                p => p.Store,
                p => p.Language
            );

            return new GeneralResponse<IEnumerable<Page>>()
            {
                Success = true,
                Message = pages.Any() ? "Search results retrieved successfully." : "No pages found matching the keyword.",
                Data = pages
            };
        }

        public async Task<GeneralResponse<Page>> CreateAsync(Page page)
        {
            var response = await _pageRepo.AddAsync(page);
            return response.Success
                ? new GeneralResponse<Page>() { Success = true, Message = "Page created successfully.", Data = response.Data }
                : new GeneralResponse<Page>() { Success = false, Message = response.Message };
        }

        public async Task<GeneralResponse<IEnumerable<Page>>> CreateRangeAsync(IEnumerable<Page> pages)
        {
            var response = await _pageRepo.AddRangeAsync(pages);
            return response.Success
                ? new GeneralResponse<IEnumerable<Page>>() { Success = true, Message = "Pages created successfully.", Data = response.Data }
                : new GeneralResponse<IEnumerable<Page>>() { Success = false, Message = response.Message };
        }

        public async Task<GeneralResponse<Page>> UpdateAsync(string id, Page page)
        {
            var existing = await _pageRepo.GetByIdAsync(id);
            if (existing == null)
                return new GeneralResponse<Page>() { Success = false, Message = "Page not found." };

            _mapper.Map(page, existing);
            var response = await _pageRepo.UpdateAsync(existing);

            return response.Success
                ? new GeneralResponse<Page>() { Success = true, Message = "Page updated successfully.", Data = response.Data }
                : new GeneralResponse<Page>() { Success = false, Message = response.Message };
        }

        public async Task<GeneralResponse<IEnumerable<Page>>> UpdateRangeAsync(IEnumerable<Page> pages)
        {
            var existingPages = new List<Page>();
            foreach (var page in pages)
            {
                var existing = await _pageRepo.GetByIdAsync(page.Id);
                if (existing != null)
                {
                    _mapper.Map(page, existing);
                    existingPages.Add(existing);
                }
            }

            var response = await _pageRepo.UpdateRangeAsync(existingPages);
            return response.Success
                ? new GeneralResponse<IEnumerable<Page>>() { Success = true, Message = "Pages updated successfully.", Data = response.Data }
                : new GeneralResponse<IEnumerable<Page>>() { Success = false, Message = response.Message };
        }

        public async Task<GeneralResponse<bool>> DeleteAsync(string id)
        {
            var existing = await _pageRepo.GetByIdAsync(id);
            if (existing == null)
                return new GeneralResponse<bool>() { Success = false, Message = "Page not found.", Data = false };

            var response = await _pageRepo.DeleteAsync(id);
            return new GeneralResponse<bool>() { Success = response.Success, Message = response.Message, Data = response.Success };
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

        public async Task<int> CountAsync(string storeId = null, string languageId = null)
        {
            return await _pageRepo.CountAsync(p => (storeId == null || p.StoreId == storeId) &&
                                                   (languageId == null || p.LanguageId == languageId));
        }
    }
}
