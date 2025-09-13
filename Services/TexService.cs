using AutoMapper;
using invoice.Core.DTO;
using invoice.Core.DTO.Product;
using invoice.Core.DTO.Tax;
using invoice.Core.Interfaces.Services;
using invoice.Repo;
using invoice.Core.Entites;


namespace invoice.Services
{
    public class TexService : ITaxService
    {
        private readonly IRepository<Tax> _taxRepo;
        private readonly IMapper _mapper;

        public TexService(IRepository<Tax> taxRepo, IMapper mapper)
        {
            _taxRepo =taxRepo;
            _mapper = mapper;
        }
        public async Task<GeneralResponse<TaxReadDTO>> CreateAsync(TaxReadDTO dto, string userId)
        {
            var taxDB = await _taxRepo.GetAllAsync(userId);
            if (taxDB.Any())
            {
                return new GeneralResponse<TaxReadDTO>(false, "this user have tax can update it only", null);
            }

            var tax = _mapper.Map<Tax>(dto);
                tax.UserId = userId;

                await _taxRepo.AddAsync(tax);
                var readDto = _mapper.Map<TaxReadDTO>(tax);
            
            return new GeneralResponse<TaxReadDTO>(true, "tax created successfully", readDto);

        }

        public async Task<GeneralResponse<TaxReadDTO>> GetByUserIdAsync(string userId)
        {
            var tax = (await _taxRepo.QueryAsync(p => p.UserId == userId)).FirstOrDefault();

            if (tax == null)
                return new GeneralResponse<TaxReadDTO>(false, "No tax found for this user", null);

            var dto = _mapper.Map<TaxReadDTO>(tax);
            return new GeneralResponse<TaxReadDTO>(true, "tax retrieved successfully", dto);
        }

        public async Task<GeneralResponse<TaxReadDTO>> UpdateAsync(TaxReadDTO dto, string userId)
        {
            var taxes = await _taxRepo.GetByUserIdAsync(userId);
            var tax = taxes.FirstOrDefault(t => t.Id == dto.Id);

            if (tax == null)
                return new GeneralResponse<TaxReadDTO>(false, "Tax not found");

            _mapper.Map(dto, tax);
            await _taxRepo.UpdateAsync(tax);

            var readDto = _mapper.Map<TaxReadDTO>(tax);
            return new GeneralResponse<TaxReadDTO>(true, "Tax updated successfully", readDto);
        }
    }
}
