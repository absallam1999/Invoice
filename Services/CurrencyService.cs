using AutoMapper;
using invoice.Core.DTO;
using invoice.Core.DTO.Currency;
using invoice.Core.DTO.Tax;
using invoice.Core.Entities;
using invoice.Core.Interfaces.Services;
using invoice.Repo;

namespace invoice.Services
{
    public class CurrencyService : ICurrencyService
    {
        private readonly IRepository<Currency> _currencyRepo;
        private readonly IMapper _mapper;

        public CurrencyService(IRepository<Currency> currencyRepo, IMapper mapper)
        {
            _currencyRepo = currencyRepo;
            _mapper = mapper;
        }
        public async Task<GeneralResponse<CurrencyReadDTO>> CreateAsync(CurrencyReadDTO dto, string userId)
        {

            var currencyDB = await _currencyRepo.GetAllAsync(userId);
            if (currencyDB.Any())
            {

                return new GeneralResponse<CurrencyReadDTO>(false, "this user have currency can update it only", null);

            }

            var currency = _mapper.Map<Currency>(dto);
            currency.UserId = userId;

            await _currencyRepo.AddAsync(currency);
            var readDto = _mapper.Map<CurrencyReadDTO>(currency);

            return new GeneralResponse<CurrencyReadDTO>(true, "currency created successfully", readDto);


        }

        public async Task<GeneralResponse<CurrencyReadDTO>> GetByUserIdAsync(string userId)
        {
            var currency = (await _currencyRepo.QueryAsync(p => p.UserId == userId)).FirstOrDefault();

            if (currency == null)
                return new GeneralResponse<CurrencyReadDTO>(false, "No currency found for this user", null);

            var dto = _mapper.Map<CurrencyReadDTO>(currency);
            return new GeneralResponse<CurrencyReadDTO>(true, "currency retrieved successfully", dto);

        }

        public async Task<GeneralResponse<CurrencyReadDTO>> UpdateAsync(CurrencyReadDTO dto, string userId)
        {
            var currency =( await _currencyRepo.GetByUserIdAsync(userId))?.FirstOrDefault();
            if (currency == null)
                return new GeneralResponse<CurrencyReadDTO>(false, "Currency not found");

            _mapper.Map(dto, currency);
            await _currencyRepo.UpdateAsync(currency);

            var readDto = _mapper.Map<CurrencyReadDTO>(currency);
            return new GeneralResponse<CurrencyReadDTO>(true, "Currency updated successfully", readDto);
        }
    }
}