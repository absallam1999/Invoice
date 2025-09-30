using AutoMapper;
using invoice.Core.DTO;
using invoice.Core.DTO.Page;
using invoice.Core.Entities;
using invoice.Core.Interfaces.Services;
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

        public async Task<GeneralResponse<IEnumerable<PageReadDTO>>> GetAllAsync(string storeId)
        {
            var pages = await _pageRepo.QueryAsync(
                p => (storeId == null || p.StoreId == storeId),
                p => p.Store
            );

            var readDtos = _mapper.Map<IEnumerable<PageReadDTO>>(pages);
            return new GeneralResponse<IEnumerable<PageReadDTO>>(
                true,
                readDtos.Any() ? "Pages retrieved successfully." : "No pages found.",
                readDtos
            );
        }

        public async Task<GeneralResponse<PageReadDTO>> GetByIdAsync(string id)
        {
            var page = await _pageRepo.GetByIdAsync(id);
            if (page == null)
                return new GeneralResponse<PageReadDTO>(false, "Page not found");

            var dto = _mapper.Map<PageReadDTO>(page);
            return new GeneralResponse<PageReadDTO>(true, "Page retrieved successfully.", dto);
        }

        public async Task<GeneralResponse<PageReadDTO>> GetByTitleAsync(string title, string storeId)
        {
            var pages = await _pageRepo.QueryAsync(
                p => p.Title == title &&
                     (storeId == null || p.StoreId == storeId),
                p => p.Store
            );

            var page = pages.FirstOrDefault();
            if (page == null)
                return new GeneralResponse<PageReadDTO>(false, "Page not found");

            var dto = _mapper.Map<PageReadDTO>(page);
            return new GeneralResponse<PageReadDTO>(true, "Page retrieved successfully.", dto);
        }

        public async Task<GeneralResponse<IEnumerable<PageReadDTO>>> SearchAsync(string keyword, string storeId)
        {
            var pages = await _pageRepo.QueryAsync(
                p => (p.Title.Contains(keyword) || p.Content.Contains(keyword)) &&
                     (storeId == null || p.StoreId == storeId),
                p => p.Store
            );

            var readDtos = _mapper.Map<IEnumerable<PageReadDTO>>(pages);
            return new GeneralResponse<IEnumerable<PageReadDTO>>(
                true,
                readDtos.Any() ? "Search results retrieved successfully." : "No results found.",
                readDtos
            );
        }

        public async Task<int> CountAsync(string storeId = null, string languageId = null)
        {
            var pages = await _pageRepo.QueryAsync(
                p => (storeId == null || p.StoreId == storeId)
            );

            return pages.Count();
        }

        public async Task<bool> ExistsAsync(string id)
        {
            var entity = await _pageRepo.GetByIdAsync(id);
            return entity != null;
        }

        public async Task<GeneralResponse<PageReadDTO>> CreateAsync(PageCreateDTO dto, PageImageDTO image)
        {
            var entity = _mapper.Map<Page>(dto);

            if (image.Image != null)
                entity.Image = await _fileService.UploadImageAsync(image.Image, "pages");

            await _pageRepo.AddAsync(entity);

            var readDto = _mapper.Map<PageReadDTO>(entity);
            return new GeneralResponse<PageReadDTO>(true, "Page created successfully", readDto);
        }

        public async Task<GeneralResponse<IEnumerable<PageReadDTO>>> CreateRangeAsync(PageCreateRangeRequest request)
        {
            var entities = new List<Page>();

            for (int i = 0; i < request.Pages.Count; i++)
            {
                var dto = request.Pages[i];
                var entity = _mapper.Map<Page>(dto);

                if (request.Images != null && request.Images.Count > i && request.Images[i] != null)
                    entity.Image = await _fileService.UploadImageAsync(request.Images[i], "pages");

                entities.Add(entity);
            }

            await _pageRepo.AddRangeAsync(entities);
            var readDtos = _mapper.Map<IEnumerable<PageReadDTO>>(entities);

            return new GeneralResponse<IEnumerable<PageReadDTO>>(true, "Pages created successfully", readDtos);
        }

        public async Task<GeneralResponse<PageReadDTO>> UpdateAsync(PageUpdateDTO dto, PageImageDTO image)
        {
            var entity = await _pageRepo.GetByIdAsync(dto.Id);
            if (entity.Image == null)
                return new GeneralResponse<PageReadDTO>(false, "Page not found");

            _mapper.Map(dto, entity);

            if (image != null)
                entity.Image = await _fileService.UploadImageAsync(image.Image, "pages");

            await _pageRepo.UpdateAsync(entity);

            var readDto = _mapper.Map<PageReadDTO>(entity);
            return new GeneralResponse<PageReadDTO>(true, "Page updated successfully", readDto);
        }

        public async Task<GeneralResponse<IEnumerable<PageReadDTO>>> UpdateRangeAsync(PageUpdateRangeRequest request)
        {
            var entities = new List<Page>();

            for (int i = 0; i < request.Pages.Count; i++)
            {
                var dto = request.Pages[i];
                var entity = await _pageRepo.GetByIdAsync(dto.Id);
                if (entity == null) continue;

                _mapper.Map(dto, entity);

                if (request.Images != null && request.Images.Count > i && request.Images[i] != null)
                    entity.Image = await _fileService.UploadImageAsync(request.Images[i], "pages");

                entities.Add(entity);
            }

            await _pageRepo.UpdateRangeAsync(entities);
            var readDtos = _mapper.Map<IEnumerable<PageReadDTO>>(entities);

            return new GeneralResponse<IEnumerable<PageReadDTO>>(true, "Pages updated successfully", readDtos);
        }

        public async Task<GeneralResponse<PageReadDTO>> UpdateImageAsync(string id, IFormFile image)
        {
            var entity = await _pageRepo.GetByIdAsync(id);
            if (entity == null)
                return new GeneralResponse<PageReadDTO>(false, "Page not found");

            entity.Image = await _fileService.UploadImageAsync(image, "pages");

            await _pageRepo.UpdateAsync(entity);

            var dto = _mapper.Map<PageReadDTO>(entity);
            return new GeneralResponse<PageReadDTO>(true, "Image updated successfully", dto);
        }

        public async Task<GeneralResponse<bool>> DeleteAsync(string id)
        {
            var entity = await _pageRepo.GetByIdAsync(id);
            if (entity == null)
                return new GeneralResponse<bool>(false, "Page not found", false);

            var result = await _pageRepo.DeleteAsync(id);
            return new GeneralResponse<bool>(result.Success, result.Message, result.Success);
        }

        public async Task<GeneralResponse<bool>> DeleteRangeAsync(IEnumerable<string> ids)
        {
            var result = await _pageRepo.DeleteRangeAsync(ids);
            return new GeneralResponse<bool>(result.Success, result.Message, result.Success);
        }
    }
}
