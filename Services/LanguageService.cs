using AutoMapper;
using invoice.Core.Interfaces.Services;
using invoice.Core.DTO;
using invoice.Core.Entities;
using invoice.Core.Enums;
using invoice.Repo;
using invoice.Core.DTO.Language;

namespace invoice.Services
{
    public class LanguageService : ILanguageService
    {
        private readonly IRepository<Language> _languageRepo;
        private readonly IMapper _mapper;

        public LanguageService(IRepository<Language> languageRepo, IMapper mapper)
        {
            _languageRepo = languageRepo;
            _mapper = mapper;
        }

        public async Task<GeneralResponse<IEnumerable<Language>>> GetAllAsync()
        {
            var entities = await _languageRepo.GetAllAsync();
            return new GeneralResponse<IEnumerable<Language>>
            {
                Success = true,
                Data = entities
            };
        }

        public async Task<GeneralResponse<Language>> GetByIdAsync(string id)
        {
            var entity = await _languageRepo.GetByIdAsync(id);
            if (entity == null)
                return new GeneralResponse<Language>
                {
                    Success = false,
                    Message = $"Language with Id '{id}' not found."
                };

            return new GeneralResponse<Language>
            {
                Success = true,
                Data = entity
            };
        }

        public async Task<GeneralResponse<IEnumerable<Language>>> GetByNameAsync(LanguageName name)
        {
            var entities = (await _languageRepo.QueryAsync(l => l.Name == name)).ToList();

            return new GeneralResponse<IEnumerable<Language>>
            {
                Success = true,
                Data = entities
            };
        }

        public async Task<GeneralResponse<IEnumerable<Language>>> GetByTargetAsync(LanguageTarget target)
        {
            var entities = (await _languageRepo.QueryAsync(l => l.Target == target)).ToList();

            return new GeneralResponse<IEnumerable<Language>>
            {
                Success = true,
                Data = entities
            };
        }

        public async Task<GeneralResponse<IEnumerable<Language>>> SearchAsync(string keyword)
        {
            var entities = await _languageRepo.QueryAsync(l => l.Name.ToString().Contains(keyword));
            return new GeneralResponse<IEnumerable<Language>>
            {
                Success = true,
                Data = entities
            };
        }

        public async Task<GeneralResponse<Language>> CreateAsync(CreateLanguageDTO language)
        {
            var newLang = _mapper.Map<Language>(language);

            var response = await _languageRepo.AddAsync(newLang);
            if (!response.Success)
                return new GeneralResponse<Language>
                {
                    Success = false,
                    Message = response.Message
                };

            return new GeneralResponse<Language>
            {
                Success = true,
                Data = response.Data
            };
        }

        public async Task<GeneralResponse<IEnumerable<Language>>> CreateRangeAsync(IEnumerable<CreateLanguageDTO> languages)
        {
            var newLanguages = _mapper.Map<IEnumerable<Language>>(languages);

            var response = await _languageRepo.AddRangeAsync(newLanguages);
            if (!response.Success)
                return new GeneralResponse<IEnumerable<Language>>
                {
                    Success = false,
                    Message = response.Message
                };

            return new GeneralResponse<IEnumerable<Language>>
            {
                Success = true,
                Data = response.Data
            };
        }

        public async Task<GeneralResponse<Language>> UpdateAsync(string id, UpdateLanguageDTO language)
        {
            var existing = await _languageRepo.GetByIdAsync(id);
            if (existing == null)
                return new GeneralResponse<Language>
                {
                    Success = false,
                    Message = $"Language with Id '{id}' not found."
                };

            _mapper.Map(language, existing);

            var response = await _languageRepo.UpdateAsync(existing);
            if (!response.Success)
                return new GeneralResponse<Language>
                {
                    Success = false,
                    Message = response.Message
                };

            return new GeneralResponse<Language>
            {
                Success = true,
                Data = response.Data
            };
        }

        public async Task<GeneralResponse<IEnumerable<Language>>> UpdateRangeAsync(IEnumerable<UpdateLanguageDTO> languages)
        {
            var ids = languages.Select(l => l.Id).ToList();
            var existingEntities = (await _languageRepo.GetAllAsync()).Where(l => ids.Contains(l.Id)).ToList();

            if (!existingEntities.Any())
                return new GeneralResponse<IEnumerable<Language>>
                {
                    Success = false,
                    Message = "No matching languages found to update."
                };

            var entityDict = existingEntities.ToDictionary(l => l.Id);

            foreach (var language in languages)
            {
                if (entityDict.TryGetValue(language.Id, out var existing))
                {
                    _mapper.Map(language, existing);
                }
            }

            var response = await _languageRepo.UpdateRangeAsync(existingEntities);
            if (!response.Success)
                return new GeneralResponse<IEnumerable<Language>>
                {
                    Success = false,
                    Message = response.Message
                };

            return new GeneralResponse<IEnumerable<Language>>
            {
                Success = true,
                Data = response.Data
            };
        }

        public async Task<GeneralResponse<bool>> DeleteAsync(string id)
        {
            var response = await _languageRepo.DeleteAsync(id);
            return new GeneralResponse<bool>
            {
                Success = response.Success,
                Message = response.Message,
                Data = response.Success
            };
        }

        public async Task<GeneralResponse<bool>> DeleteRangeAsync(IEnumerable<string> ids)
        {
            var response = await _languageRepo.DeleteRangeAsync(ids);
            return new GeneralResponse<bool>
            {
                Success = response.Success,
                Message = response.Message,
                Data = response.Success
            };
        }

        public async Task<bool> ExistsAsync(string id) => await _languageRepo.ExistsAsync(l => l.Id == id);

        public async Task<int> CountAsync() => await _languageRepo.CountAsync();
    }
}