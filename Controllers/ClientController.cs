using Microsoft.AspNetCore.Mvc;
using invoice.Models;
using invoice.Data;
using invoice.DTO.Client;
using invoice.DTO;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

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

            var client = await _repository.GetById(id, userId, c => c.Invoices);
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
                InvoiceCount= client.Invoices?.Count ?? 0,
              //  InvoiceId= client.Invoices.
                //IsDeleted = client.IsDeleted,
                //UserId = client.UserId

                //invoice
            };

            return Ok(new GeneralResponse<ClientDetailsDTO>
            {
                Success = true,
                Message = "Client retrieved successfully.",
                Data = dto
            });
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] ClientInfoDTO dto)
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
            //chick email - phonenumber
            var client = new Client
            {
                Name = dto.Name,
                Email = dto.Email,
                PhoneNumber = dto.PhoneNumber,
                Address = dto.Address,
                Notes = dto.Notes,
                TextNumber = dto.TextNumber,
                UserId = userId,
             
            };

            await _repository.Add(client);

            var result = client.Id;

              //  new ClientDetailsDTO
            //{
            //    Id = client.Id,
            //    Name = client.Name,
            //    Email = client.Email,
            //    PhoneNumber = client.PhoneNumber,
            //    Address = client.Address,
            //    Notes = client.Notes,
            //    TextNumber = client.TextNumber,
            //    //IsDeleted = client.IsDeleted,
            //    //UserId = client.UserId
            //};

            return Ok(new GeneralResponse<ClientDetailsDTO>
            {
                Success = true,
                Message = "Client created successfully.",
                Data = result
            });
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(string id, [FromBody] ClientInfoDTO dto)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
                return Unauthorized();

            //if (id != dto.id)
            //{
            //    return BadRequest(new GeneralResponse<object>
            //    {
            //        Success = false,
            //        Message = "ID mismatch.",
            //        Data = null
            //    });
            //}


            //check email phone
            if (!ModelState.IsValid)
            {
                return BadRequest(new GeneralResponse<object>
                {
                    Success = false,
                    Message = "Validation failed.",
                    Data = ModelState
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

            client.Name = dto.Name;
            client.Email = dto.Email;
            client.PhoneNumber = dto.PhoneNumber;
            client.Address = dto.Address;
            client.Notes = dto.Notes;
            client.TextNumber = dto.TextNumber;
           

            await _repository.Update(client);

            var updated = client.Id;
            //    new ClientDetailsDTO
            //{
            //    Id = client.Id,
            //    Name = client.Name,
            //    Email = client.Email,
            //    PhoneNumber = client.PhoneNumber,
            //    Address = client.Address,
            //    Notes = client.Notes,
            //    TextNumber = client.TextNumber,
            //    //IsDeleted = client.IsDeleted,
            //    //UserId = client.UserId
            //};

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


            await _repository.Delete(id);

            return Ok(new GeneralResponse<object>
            {
                Success = true,
                Message = "Client deleted successfully.",
                Data = null
            });
        }
    }
}