using invoice.Core.DTO;
using invoice.Core.DTO.Store;
using invoice.Core.DTO.StoreSettings;
using invoice.Core.Entities;
using invoice.Core.Enums;
using System.Linq.Expressions;

namespace invoice.Core.Interfaces.Services
{
    public interface IStoreService
    {
        Task<GeneralResponse<StoreReadDTO>> CreateAsync(StoreCreateDTO dto, string userId);
        Task<GeneralResponse<StoreReadDTO>> UpdateAsync(StoreUpdateDTO dto, string userId);
        Task<GeneralResponse<bool>> DeleteAsync(string id, string userId);
        Task<GeneralResponse<StoreReadDTO>> GetByIdAsync(string id, string userId);
        Task<GeneralResponse<StoreReadDTO>> GetBySlug(string slug);
        Task<GeneralResponse<object>> CreateOrderAsync(CreateOrderDTO dto, string userId,string storeId);

        Task<GeneralResponse<StoreReadDTO>> GetAsync(string userId);
        Task<GeneralResponse<IEnumerable<StoreReadDTO>>> QueryAsync(Expression<Func<Store, bool>> predicate, string userId);

        Task<GeneralResponse<StoreReadDTO>> GetByUserAsync(string userId);
        Task<GeneralResponse<IEnumerable<StoreReadDTO>>> GetActiveStoresAsync(string userId);
        Task<GeneralResponse<IEnumerable<StoreReadDTO>>> GetInactiveStoresAsync(string userId);

        Task<GeneralResponse<bool>> ActivateStoreAsync(string userId);

        Task<GeneralResponse<StoreSettingsReadDTO>> GetSettingsAsync(string storeId, string userId);
        Task<GeneralResponse<bool>> UpdateSettingsAsync(string storeId, StoreSettingsUpdateDTO settingsDto, string userId);
    
        Task<GeneralResponse<bool>> UpdatePaymentMethodsAsync(string storeId, PaymentType paymentType, string userId);

        Task<GeneralResponse<IEnumerable<StoreReadDTO>>> AddRangeAsync(IEnumerable<StoreCreateDTO> dtos, string userId);
        Task<GeneralResponse<IEnumerable<StoreReadDTO>>> UpdateRangeAsync(IEnumerable<StoreUpdateDTO> dtos, string userId);
        Task<GeneralResponse<bool>> DeleteRangeAsync(IEnumerable<string> ids, string userId);

        Task<bool> ExistsAsync(Expression<Func<Store, bool>> predicate, string userId);
        Task<int> CountAsync(Expression<Func<Store, bool>> predicate, string userId);

        IQueryable<Store> GetQueryable();
    }
}