using invoice.Core.DTO;
using System.Linq.Expressions;
using invoice.Core.DTO.Product;
using invoice.Core.Entites;

namespace Core.Interfaces.Services
{
    public interface IProductService
    {
        Task<GeneralResponse<ProductReadDTO>> CreateAsync(ProductCreateDTO dto, string userId);
        Task<GeneralResponse<ProductReadDTO>> UpdateAsync(string id, ProductUpdateDTO dto, string userId);
        Task<GeneralResponse<bool>> DeleteAsync(string id, string userId);
        Task<GeneralResponse<ProductReadDTO>> GetByIdAsync(string id, string userId);

        Task<GeneralResponse<IEnumerable<GetAllProductDTO>>> GetAllAsync(string userId);
        Task<GeneralResponse<IEnumerable<ProductReadDTO>>> QueryAsync(Expression<Func<Product, bool>> predicate, string userId);

        Task<GeneralResponse<IEnumerable<ProductReadDTO>>> GetByCategoryAsync(string categoryId, string userId);
        Task<GeneralResponse<IEnumerable<ProductReadDTO>>> GetByStoreAsync(string storeId, string userId);
        Task<GeneralResponse<IEnumerable<GetAllProductDTO>>> GetAvailableForPOSAsync(string userId);
        Task<GeneralResponse<IEnumerable<GetAllProductDTO>>> GetAvailableForStoreAsync(string userId);
        Task<GeneralResponse<IEnumerable<GetAllProductDTO>>> GetProductListAsync(string userId);

        Task<GeneralResponse<bool>> UpdateQuantityAsync(string id, int quantity, string userId);
        Task<GeneralResponse<bool>> IncrementQuantityAsync(string id, int amount, string userId);
        Task<GeneralResponse<bool>> DecrementQuantityAsync(string id, int amount, string userId);

        Task<GeneralResponse<IEnumerable<ProductReadDTO>>> AddRangeAsync(IEnumerable<ProductCreateDTO> dtos, string userId);
        Task<GeneralResponse<IEnumerable<ProductReadDTO>>> UpdateRangeAsync(IEnumerable<ProductUpdateDTO> dtos, string userId);
        Task<GeneralResponse<bool>> DeleteRangeAsync(IEnumerable<string> ids, string userId);

        Task<bool> ExistsAsync(Expression<Func<Product, bool>> predicate, string userId);
        Task<int> CountAsync(Expression<Func<Product, bool>> predicate, string userId);

        IQueryable<Product> GetQueryable();
    }
}
