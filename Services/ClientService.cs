using AutoMapper;
using Azure.Core.GeoJson;
using invoice.Core.DTO;
using invoice.Core.DTO.Client;
using invoice.Core.Entites;
using invoice.Core.Interfaces.Services;
using invoice.Repo;

namespace invoice.Services
{
    public class ClientService : IClientService
    {
        private readonly IRepository<Client> _clientRepo;
        private readonly IMapper _mapper;

        public ClientService(IRepository<Client> clientRepo, IMapper mapper)
        {
            _clientRepo = clientRepo;
            _mapper = mapper;
        }

        public async Task<GeneralResponse<IEnumerable<ClientReadDTO>>> GetAllAsync(string userId)
        {
            var clients = await _clientRepo.GetAllAsync(userId, c => c.Invoices);

            var dtoList = _mapper.Map<IEnumerable<ClientReadDTO>>(clients);

            return new GeneralResponse<IEnumerable<ClientReadDTO>>(true, "Clients retrieved successfully", dtoList);
        }

        public async Task<GeneralResponse<ClientReadDTO>> GetByIdAsync(string id, string userId)
        {
            var client = await _clientRepo.GetByIdAsync(id, userId, q => q);

            if (client == null)
                return new GeneralResponse<ClientReadDTO>(false, "Client not found");

            var dto = _mapper.Map<ClientReadDTO>(client);
            return new GeneralResponse<ClientReadDTO>(true, "Client retrieved successfully", dto);
        }

        public async Task<GeneralResponse<IEnumerable<ClientReadDTO>>> GetByNameAsync(string name, string userId)
        {
            var clients = await _clientRepo.QueryAsync(c => c.UserId == userId && (c.Name == name || c.Name.Contains(name)));
            
            var dtos = _mapper.Map<IEnumerable<ClientReadDTO>>(clients);
            return new GeneralResponse<IEnumerable<ClientReadDTO>>(true, "Clients retrieved By Name successfully", dtos);
        }

        public async Task<GeneralResponse<IEnumerable<ClientReadDTO>>> SearchAsync(string keyword, string userId)
        {
            var clients = await _clientRepo.QueryAsync(
                c => c.UserId == userId &&
                (c.Name.Contains(keyword) ||
                (c.Email ?? "").Contains(keyword) ||
                (c.PhoneNumber ?? "").Contains(keyword)));

            var dtoList = _mapper.Map<IEnumerable<ClientReadDTO>>(clients);
            return new GeneralResponse<IEnumerable<ClientReadDTO>>(true, "Search completed", dtoList);
        }

        public async Task<GeneralResponse<ClientReadDTO>> CreateAsync(ClientCreateDTO dto, string userId)
        {
            var entity = _mapper.Map<Client>(dto);
            entity.UserId = userId;

            var result = await _clientRepo.AddAsync(entity);
            if (!result.Success) return new GeneralResponse<ClientReadDTO>(false, result.Message);

            var dtoResult = _mapper.Map<ClientReadDTO>(result.Data);
            return new GeneralResponse<ClientReadDTO>(true, "Client created successfully", dtoResult);
        }

        public async Task<GeneralResponse<IEnumerable<ClientReadDTO>>> CreateRangeAsync(IEnumerable<ClientCreateDTO> dtos, string userId)
        {
            var entities = _mapper.Map<IEnumerable<Client>>(dtos).ToList();
            entities.ForEach(c => c.UserId = userId);

            var result = await _clientRepo.AddRangeAsync(entities);
            if (!result.Success) return new GeneralResponse<IEnumerable<ClientReadDTO>>(false, result.Message);

            var dtoList = _mapper.Map<IEnumerable<ClientReadDTO>>(result.Data);
            return new GeneralResponse<IEnumerable<ClientReadDTO>>(true, "Clients created successfully", dtoList);
        }

        public async Task<GeneralResponse<ClientReadDTO>> UpdateAsync(string id, ClientUpdateDTO dto, string userId)
        {
            var existing = await _clientRepo.GetByIdAsync(id, userId, q => q);
            if (existing == null) return new GeneralResponse<ClientReadDTO>(false, "Client not found");

            _mapper.Map(dto, existing);

            var result = await _clientRepo.UpdateAsync(existing);
            if (!result.Success) return new GeneralResponse<ClientReadDTO>(false, result.Message);

            var dtoResult = _mapper.Map<ClientReadDTO>(result.Data);
            return new GeneralResponse<ClientReadDTO>(true, "Client updated successfully", dtoResult);
        }

        public async Task<GeneralResponse<IEnumerable<ClientReadDTO>>> UpdateRangeAsync(IEnumerable<ClientUpdateDTO> dtos, string userId)
        {
            var entities = new List<Client>();

            foreach (var dto in dtos)
            {
                var existing = await _clientRepo.GetByIdAsync(dto.Id, userId, q => q);
                if (existing != null)
                {
                    _mapper.Map(dto, existing);
                    entities.Add(existing);
                }
            }

            var result = await _clientRepo.UpdateRangeAsync(entities);
            if (!result.Success) return new GeneralResponse<IEnumerable<ClientReadDTO>>(false, result.Message);

            var dtoList = _mapper.Map<IEnumerable<ClientReadDTO>>(result.Data);
            return new GeneralResponse<IEnumerable<ClientReadDTO>>(true, "Clients updated successfully", dtoList);
        }

        public async Task<GeneralResponse<bool>> DeleteAsync(string id, string userId)
        {
            var existing = await _clientRepo.GetByIdAsync(id, userId, q => q);
            if (existing == null) return new GeneralResponse<bool>(false, "Client not found");

            var result = await _clientRepo.DeleteAsync(id);
            return new GeneralResponse<bool>(result.Success, result.Message, result.Success);
        }

        public async Task<GeneralResponse<bool>> DeleteRangeAsync(IEnumerable<string> ids, string userId)
        {
            var result = await _clientRepo.DeleteRangeAsync(ids);
            return new GeneralResponse<bool>(result.Success, result.Message, result.Success);
        }

        public async Task<bool> ExistsAsync(string id, string userId)
        {
            return await _clientRepo.ExistsAsync(c => c.Id == id && c.UserId == userId);
        }

        public async Task<int> CountAsync(string userId)
        {
            return await _clientRepo.CountAsync(c => c.UserId == userId);
        }
    }
}
