using invoice.Core.DTO.Product;
using invoice.Core.DTO;
using System.Linq.Expressions;
using invoice.Core.DTO.Tax;

namespace invoice.Core.Interfaces.Services
{
    public interface ITaxService
    {

        Task<GeneralResponse<TaxReadDTO>> CreateAsync(TaxReadDTO dto, string userId);
        Task<GeneralResponse<TaxReadDTO>> UpdateAsync(TaxReadDTO dto, string userId);
        Task<GeneralResponse<TaxReadDTO>> GetByUserIdAsync(string userId);



    }
}