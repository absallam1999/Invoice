using AutoMapper;
using Azure.Core.GeoJson;
using invoice.Core.DTO;
using invoice.Core.DTO.Client;
using invoice.Core.Entities;
using invoice.Core.Interfaces.Services;
using invoice.Helpers;
using invoice.Repo;
using Microsoft.EntityFrameworkCore;

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

        public async Task<GeneralResponse<IEnumerable<GetAllClientsDTO>>> GetAllAsync(string userId)
        {
            var clients = await _clientRepo.GetAllAsync(userId, c => c.Invoices);

            var dtoList = _mapper.Map<IEnumerable<GetAllClientsDTO>>(clients);

            return new GeneralResponse<IEnumerable<GetAllClientsDTO>>(true, "Clients retrieved successfully", dtoList);
        }

        public async Task<GeneralResponse<ClientReadDTO>> GetByIdAsync(string id, string userId)
        {
            var client = await _clientRepo.GetByIdAsync(id, userId, q => q
             .Include(x => x.Invoices));

            if (client == null)
                return new GeneralResponse<ClientReadDTO>(false, "Client not found");

            var dto = _mapper.Map<ClientReadDTO>(client);
            dto.InvoiceCount = client.Invoices.Count;
            dto.InvoiceTotal = client.Invoices.Sum(i => i.FinalValue);
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

            var EmailExists = await _clientRepo.ExistsAsync(c => c.Email == dto.Email && c.UserId == userId);
            if (EmailExists)
                return new GeneralResponse<ClientReadDTO>(false, "Email already exists");


            var nameExists = await _clientRepo.ExistsAsync(c => c.PhoneNumber == dto.PhoneNumber && c.UserId == userId);
            if (nameExists)
                return new GeneralResponse<ClientReadDTO>(false, "Phone number already exists");


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
            var existing = await _clientRepo.GetByIdAsync(id, userId, q => q
            .Include(c => c.Invoices)
            );
            if (existing == null) return new GeneralResponse<ClientReadDTO>(false, "Client not found");

            var exists = await _clientRepo.QueryAsync( c => c.UserId == userId && c.Email == dto.Email && c.Id != id);
          
            if (exists.Any())
            {
                return new GeneralResponse<ClientReadDTO>
                {
                    Success = false,
                    Message = "Email already exists, please choose another one."
                };
            }


            _mapper.Map(dto, existing);
            existing.UpdatedAt = GetSaudiTime.Now();
            var result = await _clientRepo.UpdateAsync(existing);
            if (!result.Success) return new GeneralResponse<ClientReadDTO>(false, result.Message);

            var dtoResult = _mapper.Map<ClientReadDTO>(result.Data);
            dtoResult.InvoiceCount = existing.Invoices.Count;
            dtoResult.InvoiceTotal = existing.Invoices.Sum(i => i.FinalValue);
            return new GeneralResponse<ClientReadDTO>(true, "Client updated successfully", dtoResult);
        }

        public async Task<GeneralResponse<IEnumerable<ClientReadDTO>>> UpdateRangeAsync(IEnumerable<ClientUpdateDTO> dtos, string userId)
        {
            var entities = new List<Client>();

            var result = await _clientRepo.UpdateRangeAsync(entities);
            if (!result.Success) return new GeneralResponse<IEnumerable<ClientReadDTO>>(false, result.Message);

            var dtoList = _mapper.Map<IEnumerable<ClientReadDTO>>(result.Data);
            return new GeneralResponse<IEnumerable<ClientReadDTO>>(true, "Clients updated successfully", dtoList);
        }

        public async Task<GeneralResponse<bool>> DeleteAsync(string id, string userId)
        {
            var transaction = await _clientRepo.BeginTransactionAsync();

            try
            {
                var existing = await _clientRepo.GetByIdAsync(id, userId, q => q);
                if (existing == null)
                    throw new Exception("Client not found");

                existing.DeletedAt = GetSaudiTime.Now();
                await _clientRepo.UpdateAsync(existing);

                var result = await _clientRepo.DeleteAsync(id);
                if (!result.Success)
                    throw new Exception(result.Message);

                await _clientRepo.CommitTransactionAsync(transaction);

                return new GeneralResponse<bool>(true, "Client deleted successfully", true);
            }
            catch (Exception ex)
            {
                await _clientRepo.RollbackTransactionAsync(transaction);
                return new GeneralResponse<bool>(false, "Error deleting client: " + ex.Message, false);
            }
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
            return await _clientRepo.CountAsync(userId);

        }
    }
}