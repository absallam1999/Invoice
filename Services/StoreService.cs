using AutoMapper;
using invoice.Core.DTO;
using invoice.Core.DTO.Store;
using invoice.Core.DTO.StoreSettings;
using invoice.Core.Entites;
using invoice.Core.Enums;
using invoice.Core.Interfaces.Services;
using invoice.Models.Entites.utils;
using invoice.Repo;
using System.Linq.Expressions;

namespace invoice.Services
{
    public class StoreService : IStoreService
    {
        private readonly IRepository<Store> _storeRepo;
        private readonly IMapper _mapper;

        public StoreService(IRepository<Store> storeRepo, IMapper mapper)
        {
            _storeRepo = storeRepo;
            _mapper = mapper;
        }

        public async Task<GeneralResponse<StoreReadDTO>> CreateAsync(StoreCreateDTO dto, string userId)
        {
            if (dto == null)
                return new GeneralResponse<StoreReadDTO>(false, "Invalid payload");

            var entity = _mapper.Map<Store>(dto);
            entity.UserId = userId;

            if (entity.StoreSettings == null)
            {
                entity.StoreSettings = new StoreSettings
                {
                    Url = $"store-{Guid.NewGuid():N}",
                    Color = "#FFFFFF",
                    Currency = "USD",
                    purchaseOptions = new PurchaseCompletionOptions
                    {
                        Name = true,
                        Email = false,
                        phone = false,
                        TermsAndConditions = null
                    }
                };
            }
            else
            {
                entity.StoreSettings.Url ??= $"store-{Guid.NewGuid():N}";
                entity.StoreSettings.Color ??= "#FFFFFF";
                entity.StoreSettings.Currency ??= "USD";
                entity.StoreSettings.purchaseOptions ??= new PurchaseCompletionOptions
                {
                    Name = true,
                    Email = false,
                    phone = false,
                    TermsAndConditions = null
                };
            }

            if (entity.Shipping == null)
            {
                entity.Shipping = new Shipping
                {
                    PaymentType = dto.PaymentMethod,
                    FromStore = true,
                };
            }
            else
            {
                entity.Shipping.PaymentType = dto.PaymentMethod;
                entity.Shipping.FromStore = true;
            }

            var resp = await _storeRepo.AddAsync(entity);
            if (!resp.Success)
                return new GeneralResponse<StoreReadDTO>(false, resp.Message);

            return new GeneralResponse<StoreReadDTO>(
                true,
                "Store created successfully",
                _mapper.Map<StoreReadDTO>(resp.Data)
            );
        }

        public async Task<GeneralResponse<IEnumerable<StoreReadDTO>>> AddRangeAsync(IEnumerable<StoreCreateDTO> dtos, string userId)
        {
            if (dtos == null)
                return new GeneralResponse<IEnumerable<StoreReadDTO>>(false, "Invalid payload");

            var entities = _mapper.Map<List<Store>>(dtos);

            foreach (var e in entities)
            {
                e.UserId = userId;

                if (e.StoreSettings == null)
                {
                    e.StoreSettings = new StoreSettings
                    {
                        Url = $"store-{Guid.NewGuid():N}",
                        Color = "#FFFFFF",
                        Currency = "USD",
                        purchaseOptions = new PurchaseCompletionOptions
                        {
                            Name = true,
                            Email = false,
                            phone = false,
                            TermsAndConditions = null
                        }
                    };
                }
                else
                {
                    if (string.IsNullOrWhiteSpace(e.StoreSettings.Url))
                        e.StoreSettings.Url = $"store-{Guid.NewGuid():N}";

                    e.StoreSettings.Color ??= "#FFFFFF";
                    e.StoreSettings.Currency ??= "USD";
                    e.StoreSettings.purchaseOptions ??= new PurchaseCompletionOptions
                    {
                        Name = true,
                        Email = false,
                        phone = false,
                        TermsAndConditions = null
                    };
                }
            }

            var resp = await _storeRepo.AddRangeAsync(entities);
            if (!resp.Success) return new GeneralResponse<IEnumerable<StoreReadDTO>>(false, resp.Message);

            return new GeneralResponse<IEnumerable<StoreReadDTO>>(true, "Stores created successfully",
                _mapper.Map<IEnumerable<StoreReadDTO>>(resp.Data));
        }

        public async Task<GeneralResponse<StoreReadDTO>> UpdateAsync(string id, StoreUpdateDTO dto, string userId)
        {
            var entity = await _storeRepo.GetByIdAsync(id, userId);
            if (entity == null)
                return new GeneralResponse<StoreReadDTO>(false, "Store not found");

            _mapper.Map(dto, entity);
            await _storeRepo.UpdateAsync(entity);

            return new GeneralResponse<StoreReadDTO>(true, "Store updated successfully", _mapper.Map<StoreReadDTO>(entity));
        }

        public async Task<GeneralResponse<IEnumerable<StoreReadDTO>>> UpdateRangeAsync(IEnumerable<StoreUpdateDTO> dtos, string userId)
        {
            var updatedEntities = new List<Store>();
            foreach (var dto in dtos)
            {
                var entity = await _storeRepo.GetByIdAsync(dto.Id, userId);
                if (entity != null)
                {
                    _mapper.Map(dto, entity);
                    updatedEntities.Add(entity);
                }
            }

            await _storeRepo.UpdateRangeAsync(updatedEntities);
            return new GeneralResponse<IEnumerable<StoreReadDTO>>(true, "Stores updated successfully", _mapper.Map<IEnumerable<StoreReadDTO>>(updatedEntities));
        }

        public async Task<GeneralResponse<bool>> UpdateSettingsAsync(string storeId, StoreSettingsReadDTO settingsDto, string userId)
        {
            var entity = await _storeRepo.GetByIdAsync(storeId, userId);
            if (entity == null)
                return new GeneralResponse<bool>(false, "Store not found");

            entity.StoreSettings = _mapper.Map<StoreSettings>(settingsDto);
            await _storeRepo.UpdateAsync(entity);

            return new GeneralResponse<bool>(true, "Settings updated successfully", true);
        }

        public async Task<GeneralResponse<bool>> UpdatePaymentMethodsAsync(string storeId, PaymentType paymentType, string userId)
        {
            var entity = await _storeRepo.GetByIdAsync(storeId, userId);
            if (entity == null)
                return new GeneralResponse<bool>(false, "Store not found");

            entity.PaymentMethod = paymentType;
            await _storeRepo.UpdateAsync(entity);

            return new GeneralResponse<bool>(true, "Payment methods updated successfully", true);
        }

        public async Task<GeneralResponse<bool>> DeleteAsync(string id, string userId)
        {
            var entity = await _storeRepo.GetByIdAsync(id, userId);
            if (entity == null)
                return new GeneralResponse<bool>(false, "Store not found");

            await _storeRepo.DeleteAsync(entity.Id);
            return new GeneralResponse<bool>(true, "Store deleted successfully", true);
        }

        public async Task<GeneralResponse<bool>> DeleteRangeAsync(IEnumerable<string> ids, string userId)
        {
            var entities = new List<string>();
            foreach (var id in ids)
            {
                var entity = await _storeRepo.GetByIdAsync(id, userId);
                if (entity != null) entities.Add(entity.Id);
            }

            await _storeRepo.DeleteRangeAsync(entities);
            return new GeneralResponse<bool>(true, "Stores deleted successfully", true);
        }

        public async Task<GeneralResponse<StoreReadDTO>> GetByIdAsync(string id, string userId)
        {
            var entity = await _storeRepo.GetByIdAsync(id, userId);
            if (entity == null)
                return new GeneralResponse<StoreReadDTO>(false, "Store not found");

            return new GeneralResponse<StoreReadDTO>(true, "Store retrieved successfully", _mapper.Map<StoreReadDTO>(entity));
        }

        public async Task<GeneralResponse<IEnumerable<StoreReadDTO>>> GetAllAsync(string userId)
        {
            var entities = await _storeRepo.GetAllAsync(userId);
            return new GeneralResponse<IEnumerable<StoreReadDTO>>(true, "Stores retrieved successfully", _mapper.Map<IEnumerable<StoreReadDTO>>(entities));
        }

        public async Task<GeneralResponse<IEnumerable<StoreReadDTO>>> QueryAsync(Expression<Func<Store, bool>> predicate, string userId)
        {
            var entities = await _storeRepo.QueryAsync(predicate);
            entities = entities.Where(s => s.UserId == userId);
            return new GeneralResponse<IEnumerable<StoreReadDTO>>(true, "Stores retrieved successfully", _mapper.Map<IEnumerable<StoreReadDTO>>(entities));
        }

        public async Task<GeneralResponse<IEnumerable<StoreReadDTO>>> GetByUserAsync(string userId)
        {
            var entities = await _storeRepo.GetAllAsync(userId);
            return new GeneralResponse<IEnumerable<StoreReadDTO>>(true, "Stores retrieved successfully", _mapper.Map<IEnumerable<StoreReadDTO>>(entities));
        }

        public async Task<GeneralResponse<IEnumerable<StoreReadDTO>>> GetByLanguageAsync(string languageId, string userId)
        {
            var entities = await _storeRepo.QueryAsync(s => s.LanguageId == languageId && s.UserId == userId);
            return new GeneralResponse<IEnumerable<StoreReadDTO>>(true, "Stores retrieved successfully", _mapper.Map<IEnumerable<StoreReadDTO>>(entities));
        }

        public async Task<GeneralResponse<IEnumerable<StoreReadDTO>>> GetActiveStoresAsync(string userId)
        {
            var entities = await _storeRepo.QueryAsync(s => s.IsActivated && s.UserId == userId);
            return new GeneralResponse<IEnumerable<StoreReadDTO>>(true, "Active stores retrieved successfully", _mapper.Map<IEnumerable<StoreReadDTO>>(entities));
        }

        public async Task<GeneralResponse<IEnumerable<StoreReadDTO>>> GetInactiveStoresAsync(string userId)
        {
            var entities = await _storeRepo.QueryAsync(s => !s.IsActivated && s.UserId == userId);
            return new GeneralResponse<IEnumerable<StoreReadDTO>>(true, "Inactive stores retrieved successfully", _mapper.Map<IEnumerable<StoreReadDTO>>(entities));
        }

        public async Task<GeneralResponse<bool>> ActivateStoreAsync(string id, string userId)
        {
            var entity = await _storeRepo.GetByIdAsync(id, userId);
            if (entity == null)
                return new GeneralResponse<bool>(false, "Store not found");

            entity.IsActivated = true;
            await _storeRepo.UpdateAsync(entity);

            return new GeneralResponse<bool>(true, "Store activated successfully", true);
        }

        public async Task<GeneralResponse<bool>> DeactivateStoreAsync(string id, string userId)
        {
            var entity = await _storeRepo.GetByIdAsync(id, userId);
            if (entity == null)
                return new GeneralResponse<bool>(false, "Store not found");

            entity.IsActivated = false;
            await _storeRepo.UpdateAsync(entity);

            return new GeneralResponse<bool>(true, "Store deactivated successfully", true);
        }

        public async Task<GeneralResponse<StoreSettingsReadDTO>> GetSettingsAsync(string storeId, string userId)
        {
            var entity = await _storeRepo.GetByIdAsync(storeId, userId);
            if (entity == null)
                return new GeneralResponse<StoreSettingsReadDTO>(false, "Store not found");

            return new GeneralResponse<StoreSettingsReadDTO>(true, "Settings retrieved successfully", _mapper.Map<StoreSettingsReadDTO>(entity.StoreSettings));
        }

        public async Task<bool> ExistsAsync(Expression<Func<Store, bool>> predicate, string userId)
        {
            var entities = await _storeRepo.QueryAsync(predicate);
            return entities.Any(s => s.UserId == userId);
        }

        public async Task<int> CountAsync(Expression<Func<Store, bool>> predicate, string userId)
        {
            var entities = await _storeRepo.QueryAsync(predicate);
            return entities.Count(s => s.UserId == userId);
        }

        public IQueryable<Store> GetQueryable()
        {
            return _storeRepo.GetQueryable();
        }
    }
}
