﻿using invoice.Core.DTO;
using invoice.Core.DTO.Page;
using invoice.Core.Entities;

namespace invoice.Core.Interfaces.Services
{
    public interface IPageService
    {

        Task<GeneralResponse<IEnumerable<GetAllPagesDTO>>> GetAllAsync(string userId);


        Task<GeneralResponse<PageReadDTO>> GetByIdAsync(string id);
        Task<GeneralResponse<Page>> GetByTitleAsync(string title, string storeId = null, string languageId = null);
        Task<GeneralResponse<IEnumerable<Page>>> SearchAsync(string keyword, string storeId = null, string languageId = null);
        Task<GeneralResponse<PageReadDTO>> CreateAsync(PageCreateDTO dto, string StoreId);
        Task<GeneralResponse<IEnumerable<Page>>> CreateRangeAsync(IEnumerable<Page> pages);

        Task<GeneralResponse<PageReadDTO>> UpdateAsync(string id, PageUpdateDTO page);
        Task<GeneralResponse<IEnumerable<Page>>> UpdateRangeAsync(IEnumerable<Page> pages);

        Task<GeneralResponse<bool>> DeleteAsync(string id);
        Task<GeneralResponse<bool>> DeleteRangeAsync(IEnumerable<string> ids);

        Task<bool> ExistsAsync(string id);
        Task<int> CountAsync(string userId = null,string storeId = null, string languageId = null);
    }
}