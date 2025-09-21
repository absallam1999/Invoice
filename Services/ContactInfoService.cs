using AutoMapper;
using invoice.Core.DTO.ContactInformation;
using invoice.Core.Interfaces.Services;
using invoice.Core.DTO;
using invoice.Core.Entities;
using invoice.Repo;

namespace invoice.Services
{
    public class ContactInfoService : IContactInfoService
    {
        private readonly IRepository<ContactInfo> _contactInfoRepo;
        private readonly IMapper _mapper;

        public ContactInfoService(IRepository<ContactInfo> contactInfoRepo, IMapper mapper)
        {
            _contactInfoRepo = contactInfoRepo;
            _mapper = mapper;
        }

        public async Task<GeneralResponse<IEnumerable<ContactInfoReadDTO>>> GetAllAsync(string userId)
        {
            var entities = await _contactInfoRepo.GetAllAsync(userId, c => c.Store);
            var result = _mapper.Map<IEnumerable<ContactInfoReadDTO>>(entities);

            return new GeneralResponse<IEnumerable<ContactInfoReadDTO>>
            {
                Success = true,
                Data = result
            };
        }

        public async Task<GeneralResponse<ContactInfoReadDTO>> GetByIdAsync(string id, string userId)
        {
            var entity = await _contactInfoRepo.GetByIdAsync(id, userId, q => q);
            if (entity == null)
                return new GeneralResponse<ContactInfoReadDTO>
                {
                    Success = false,
                    Message = "ContactInfo not found"
                };

            return new GeneralResponse<ContactInfoReadDTO>
            {
                Success = true,
                Data = _mapper.Map<ContactInfoReadDTO>(entity)
            };
        }

        public async Task<GeneralResponse<IEnumerable<ContactInfoReadDTO>>> GetByStoreIdAsync(string storeId, string userId)
        {
            var entities = await _contactInfoRepo.QueryAsync(c => c.StoreId == storeId);
            var result = _mapper.Map<IEnumerable<ContactInfoReadDTO>>(entities);

            return new GeneralResponse<IEnumerable<ContactInfoReadDTO>>
            {
                Success = true,
                Data = result
            };
        }

        public async Task<GeneralResponse<ContactInfoReadDTO>> GetByEmailAsync(string email, string userId)
        {
            var entity = (await _contactInfoRepo.QueryAsync(c => c.Email == email)).FirstOrDefault();
            if (entity == null)
                return new GeneralResponse<ContactInfoReadDTO>
                {
                    Success = false,
                    Message = "ContactInfo not found"
                };

            return new GeneralResponse<ContactInfoReadDTO>
            {
                Success = true,
                Data = _mapper.Map<ContactInfoReadDTO>(entity)
            };
        }

        public async Task<GeneralResponse<IEnumerable<ContactInfoReadDTO>>> SearchAsync(string keyword, string userId)
        {
            var entities = await _contactInfoRepo.QueryAsync(c =>
                c.Email.Contains(keyword) ||
                c.Phone.Contains(keyword) ||
                (c.Location != null && c.Location.Contains(keyword)));

            var result = _mapper.Map<IEnumerable<ContactInfoReadDTO>>(entities);

            return new GeneralResponse<IEnumerable<ContactInfoReadDTO>>
            {
                Success = true,
                Data = result
            };
        }

        public async Task<GeneralResponse<ContactInfoReadDTO>> CreateAsync(ContactInfoCreateDTO dto, string userId)
        {
            var entity = _mapper.Map<ContactInfo>(dto);

            var response = await _contactInfoRepo.AddAsync(entity);
            if (!response.Success)
                return new GeneralResponse<ContactInfoReadDTO>
                {
                    Success = false,
                    Message = response.Message
                };

            return new GeneralResponse<ContactInfoReadDTO>
            {
                Success = true,
                Data = _mapper.Map<ContactInfoReadDTO>(response.Data)
            };
        }

        public async Task<GeneralResponse<IEnumerable<ContactInfoReadDTO>>> CreateRangeAsync(IEnumerable<ContactInfoCreateDTO> dtos, string userId)
        {
            var entities = _mapper.Map<IEnumerable<ContactInfo>>(dtos);

            var response = await _contactInfoRepo.AddRangeAsync(entities);
            if (!response.Success)
                return new GeneralResponse<IEnumerable<ContactInfoReadDTO>>
                {
                    Success = false,
                    Message = response.Message
                };

            return new GeneralResponse<IEnumerable<ContactInfoReadDTO>>
            {
                Success = true,
                Data = _mapper.Map<IEnumerable<ContactInfoReadDTO>>(response.Data)
            };
        }

        public async Task<GeneralResponse<ContactInfoReadDTO>> UpdateAsync(string id, ContactInfoUpdateDTO dto, string userId)
        {
            var entity = await _contactInfoRepo.GetByIdAsync(id, userId, q => q);
            if (entity == null)
                return new GeneralResponse<ContactInfoReadDTO>
                {
                    Success = false,
                    Message = "ContactInfo not found"
                };

            _mapper.Map(dto, entity);

            var response = await _contactInfoRepo.UpdateAsync(entity);
            if (!response.Success)
                return new GeneralResponse<ContactInfoReadDTO>
                {
                    Success = false,
                    Message = response.Message
                };

            return new GeneralResponse<ContactInfoReadDTO>
            {
                Success = true,
                Data = _mapper.Map<ContactInfoReadDTO>(response.Data)
            };
        }

        public async Task<GeneralResponse<IEnumerable<ContactInfoReadDTO>>> UpdateRangeAsync(
            IEnumerable<ContactInfoUpdateDTO> dtos, string userId)
        {
            var entities = new List<ContactInfo>();

            foreach (var dto in dtos)
            {
              //  var entity = await _contactInfoRepo.GetByIdAsync(dto.Id, userId);
                //if (entity != null)
                //{
                //    _mapper.Map(dto, entity);
                //    entities.Add(entity);
                //}
            }

            var response = await _contactInfoRepo.UpdateRangeAsync(entities);
            if (!response.Success)
                return new GeneralResponse<IEnumerable<ContactInfoReadDTO>>(false, "Failed to update contacts");

            var dtoList = _mapper.Map<IEnumerable<ContactInfoReadDTO>>(response.Data);
            return new GeneralResponse<IEnumerable<ContactInfoReadDTO>>(true, "Contacts updated successfully", dtoList);
        }

        public async Task<GeneralResponse<bool>> DeleteAsync(string id, string userId)
        {
            var response = await _contactInfoRepo.DeleteAsync(id);

            return new GeneralResponse<bool>
            {
                Success = response.Success,
                Message = response.Message,
                Data = response.Success
            };
        }

        public async Task<GeneralResponse<bool>> DeleteRangeAsync(IEnumerable<string> ids, string userId)
        {
            var response = await _contactInfoRepo.DeleteRangeAsync(ids);

            return new GeneralResponse<bool>
            {
                Success = response.Success,
                Message = response.Message,
                Data = response.Success
            };
        }

        public async Task<bool> ExistsAsync(string id, string userId) =>
            await _contactInfoRepo.ExistsAsync(c => c.Id == id);

        public async Task<int> CountAsync(string userId) =>
            await _contactInfoRepo.CountAsync(c => c.Store.UserId == userId);
    }
}
