using Microsoft.AspNetCore.Mvc;
using invoice.Models;
using invoice.Data;
using invoice.DTO.Client;
using invoice.DTO;
using Microsoft.AspNetCore.Authorization;

namespace invoice.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class ClientController : ControllerBase
    {
        private readonly IRepository<Client> _repository;

        public ClientController(IRepository<Client> repository)
        {
            _repository = repository;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var clients = await _repository.GetAll();
            var result = clients.Select(c => new ClientDetailsDTO
            {
                Id = c.Id,
                Name = c.Name,
                Email = c.Email,
                PhoneNumber = c.PhoneNumber,
                Address = c.Address,
                Notes = c.Notes,
                TextNumber = c.TextNumber,
                IsDeleted = c.IsDeleted,
                UserId = c.UserId
            });

            return Ok(new GeneralResponse<object>
            {
                Success = true,
                Message = "Clients retrieved successfully",
                Data = result
            });
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(string id)
        {
            var client = await _repository.GetById(id);
            if (client == null)
                return NotFound(new GeneralResponse<object>
                {
                    Success = false,
                    Message = "Client not found"
                });

            var dto = new ClientDetailsDTO
            {
                Id = client.Id,
                Name = client.Name,
                Email = client.Email,
                PhoneNumber = client.PhoneNumber,
                Address = client.Address,
                Notes = client.Notes,
                TextNumber = client.TextNumber,
                IsDeleted = client.IsDeleted,
                UserId = client.UserId
            };

            return Ok(new GeneralResponse<object>
            {
                Success = true,
                Message = "Client retrieved successfully",
                Data = dto
            });
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateClientDTO dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var client = new Client
            {
                Name = dto.Name,
                Email = dto.Email,
                PhoneNumber = dto.PhoneNumber,
                Address = dto.Address,
                Notes = dto.Notes,
                TextNumber = dto.TextNumber,
                UserId = dto.UserId,
                IsDeleted = false
            };

            await _repository.Add(client);

            return CreatedAtAction(nameof(GetById), new { id = client.Id }, new GeneralResponse<object>
            {
                Success = true,
                Message = "Client created successfully",
                Data = client
            });
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(string id, [FromBody] UpdateClientDTO dto)
        {
            if (id != dto.Id)
                return BadRequest("ID mismatch.");

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var client = await _repository.GetById(id);
            if (client == null)
                return NotFound();

            client.Name = dto.Name;
            client.Email = dto.Email;
            client.PhoneNumber = dto.PhoneNumber;
            client.Address = dto.Address;
            client.Notes = dto.Notes;
            client.TextNumber = dto.TextNumber;
            client.UserId = dto.UserId;
            client.IsDeleted = dto.IsDeleted;

            await _repository.Update(client);

            return Ok(new GeneralResponse<object>
            {
                Success = true,
                Message = "Client updated successfully"
            });
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            var client = await _repository.GetById(id);
            if (client == null)
                return NotFound();

            await _repository.Delete(id);

            return Ok(new GeneralResponse<object>
            {
                Success = true,
                Message = "Client deleted successfully"
            });
        }
    }
}