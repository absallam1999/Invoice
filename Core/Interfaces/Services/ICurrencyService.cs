using invoice.Core.DTO.Tax;
using invoice.Core.DTO;
using invoice.Core.DTO.Currency;

namespace invoice.Core.Interfaces.Services
{
    public interface ICurrencyService
    {

        Task<GeneralResponse<CurrencyReadDTO>> CreateAsync(CurrencyReadDTO dto, string userId);
        Task<GeneralResponse<CurrencyReadDTO>> UpdateAsync(CurrencyReadDTO dto, string userId);
        Task<GeneralResponse<CurrencyReadDTO>> GetByUserIdAsync(string userId);
    }
}
