using Microsoft.AspNetCore.Mvc;
using invoice.Models;
using invoice.Data;
using invoice.DTO.Client;
using invoice.DTO;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using invoice.DTO.Invoice;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;

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
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
                return Unauthorized();

            var clients = await _repository.GetAll(userId,c => c.Invoices);
            var result = clients.Select(c => new GetAllClientsDTO
            {
                ClientId = c.Id,
                Name = c.Name,
                Email = c.Email,
                PhoneNumber = c.PhoneNumber,
                InvoiceCount = c.Invoices?.Count ?? 0
            });

            return Ok(new GeneralResponse<IEnumerable<ClientDetailsDTO>>
            {
                Success = true,
                Message = "Clients retrieved successfully.",
                Data = result
            });
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(string id)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(userId))
                return Unauthorized();

           // var client = await _repository.GetById(id, userId, c => c.Invoices);
            var client = await _repository.GetById(id, userId, q=>q 
            .Include(c => c.Invoices));
            if (client == null)
            {
                return NotFound(new GeneralResponse<object>
                {
                    Success = false,
                    Message = $"Client with ID {id} not found.",
                    Data = null
                });
            }

            var dto = new ClientDetailsDTO
            {
                ClientId = client.Id,
                Name = client.Name,
                Email = client.Email,
                PhoneNumber = client.PhoneNumber,
                Address = client.Address,
                Notes = client.Notes,
                TextNumber = client.TextNumber,
                CreateAt = client.CreateAt,
                InvoiceCount = client.Invoices?.Count ?? 0,
                Invoices = client.Invoices?.Select(i=>new GetAllInvoiceDTO
                {
                    InvoiceId = i.Id,
                    InvoiceCreateAt=i.CreateAt,
                    InvoiceValue=i.Value,
                    InvoiceCode = i.Code,
                    InvoiceStatus = i.InvoiceStatus,
                    InvoiceType = i.InvoiceType,
                }).ToList()

            };

            return Ok(new GeneralResponse<ClientDetailsDTO>
            {
                Success = true,
                Message = "Client retrieved successfully.",
                Data = dto
            });
        }

        [HttpPatch]
        public async Task<IActionResult> Create([FromBody] ClientInfoDTO dto)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
                return Unauthorized();
            Console.WriteLine( userId);
            if (!ModelState.IsValid)
            {
                return BadRequest(new GeneralResponse<object>
                {
                    Success = false,
                    Message = "Validation failed.",
                    Data = ModelState
                });
            }

            var duplicates = await _repository.Query(c =>
                c.UserId == userId && (c.Email == dto.Email || c.PhoneNumber == dto.PhoneNumber));

            if (duplicates.Any())
            {
                var existing = duplicates.First();
                string errorMessage = existing.Email == dto.Email && existing.PhoneNumber == dto.PhoneNumber
                    ? "Email and phone number already exist for this user."
                    : existing.Email == dto.Email
                    ? "Email already exists for this user."
                    : "Phone number already exists for this user.";

                return BadRequest(new GeneralResponse<object>
                {
                    Success = false,
                    Message = errorMessage,
                    Data = null
                });
            }

            var client = new Client
            {
                Name = dto.Name,
                Email = dto.Email,
                PhoneNumber = dto.PhoneNumber,
                Address = dto.Address,
                Notes = dto.Notes,
                TextNumber = dto.TextNumber,
                UserId = userId
            };

            var result = await _repository.Add(client);
            if (!result.Success)
            {
                return StatusCode(500, new GeneralResponse<object>
                {
                    Success = false,
                    Message = result.Message ?? "Failed to create client.",
                    Data = null
                });
            }

            var clientWithInvoices = await _repository.GetById(client.Id, userId);

            var dtoResult = new ClientDetailsDTO
            {
                ClientId = clientWithInvoices.Id,
                Name = clientWithInvoices.Name,
                Email = clientWithInvoices.Email,
                PhoneNumber = clientWithInvoices.PhoneNumber,
                Address = clientWithInvoices.Address,
                Notes = clientWithInvoices.Notes,
                TextNumber = clientWithInvoices.TextNumber,
                CreateAt = client.CreateAt,
                InvoiceCount = clientWithInvoices.Invoices?.Count ?? 0,
                Invoices = clientWithInvoices.Invoices?.Select(i => new GetAllInvoiceDTO
                {
                    InvoiceId = i.Id,
                    InvoiceCreateAt = i.CreateAt,
                    InvoiceValue = i.Value,
                    InvoiceCode = i.Code,
                    InvoiceStatus = i.InvoiceStatus,
                    InvoiceType = i.InvoiceType,
                }).ToList()
            };

            return Ok(new GeneralResponse<ClientDetailsDTO>
            {
                Success = true,
                Message = "Client created successfully.",
                Data = dtoResult
            });
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(string id, [FromBody] ClientInfoDTO dto)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
                return Unauthorized();

            if (!ModelState.IsValid)
            {
                return BadRequest(new GeneralResponse<object>
                {
                    Success = false,
                    Message = "Validation failed.",
                    Data = ModelState
                });
            }
        
                    var filterEmailOrPhone = await _repository.Query(c =>
                  c.Id != id && c.UserId == userId && (
                  (dto.Email != null && c.Email == dto.Email) ||
                  (dto.PhoneNumber != null && c.PhoneNumber == dto.PhoneNumber)
                ));
            string errorMessage="";

            var existingClient = filterEmailOrPhone.FirstOrDefault();
            if (existingClient != null)
            {
                 errorMessage = existingClient.Email == dto.Email && existingClient.PhoneNumber == dto.PhoneNumber
               ? "Email and phone number already exist for this user."
               : existingClient.Email == dto.Email
               ? "Email already exists for this user."
               : "Phone number already exists for this user.";

            }

            if (filterEmailOrPhone.Any())
            {
                return BadRequest(new GeneralResponse<object>
                {
                    Success = false,
                    Message = errorMessage,
                    Data = null
                });
            }


            var client = await _repository.GetById(id, userId);
            if (client == null)
            {
                return NotFound(new GeneralResponse<object>
                {
                    Success = false,
                    Message = $"Client with ID {id} not found.",
                    Data = null
                });
            }
            if (!string.IsNullOrWhiteSpace(dto.Name))
                client.Name = dto.Name;

            if (!string.IsNullOrWhiteSpace(dto.Email))
                client.Email = dto.Email;

            if (!string.IsNullOrWhiteSpace(dto.PhoneNumber))
                client.PhoneNumber = dto.PhoneNumber;

            if (!string.IsNullOrWhiteSpace(dto.Address))
                client.Address = dto.Address;

            if (!string.IsNullOrWhiteSpace(dto.Notes))
                client.Notes = dto.Notes;

            if (!string.IsNullOrWhiteSpace(dto.TextNumber))
                client.TextNumber = dto.TextNumber;


            var result = await _repository.Update(client);

            if (!result.Success)
            {
                return StatusCode(500, new GeneralResponse<object>
                {
                    Success = false,
                    Message = result.Message,
                    Data = null
                });
            }
            var updated = new ClientDetailsDTO
            {
                ClientId = client.Id,
                Name = client.Name,
                Email = client.Email,
                PhoneNumber = client.PhoneNumber,
                Address = client.Address,
                Notes = client.Notes,
                TextNumber = client.TextNumber,
                CreateAt=client.CreateAt,
                InvoiceCount = client.Invoices?.Count ?? 0,
                Invoices = client.Invoices?.Select(i => new GetAllInvoiceDTO
                {
                    InvoiceId = i.Id,
                    InvoiceCreateAt = i.CreateAt,
                    InvoiceValue = i.Value,
                    InvoiceCode = i.Code,
                    InvoiceStatus = i.InvoiceStatus,
                    InvoiceType = i.InvoiceType,
                }).ToList()
            };

            return Ok(new GeneralResponse<ClientDetailsDTO>
            {
                Success = true,
                Message = "Client updated successfully.",
                Data = updated
            });
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
                return Unauthorized();

            var client = await _repository.GetById(id, userId);
            if (client == null)
            {
                return NotFound(new GeneralResponse<object>
                {
                    Success = false,
                    Message = $"Client with ID {id} not found.",
                    Data = null
                });
            }


            var result = await _repository.Delete(id);


            if (!result.Success)
                return NotFound(new { result.Message });

            return Ok(new { result.Message });
        }



    }
}