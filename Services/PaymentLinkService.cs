using AutoMapper;
using invoice.Core.DTO;
using invoice.Core.DTO.Client;
using invoice.Core.DTO.Invoice;
using invoice.Core.DTO.InvoiceItem;
using invoice.Core.DTO.Payment;
using invoice.Core.DTO.PaymentLink;
using invoice.Core.DTO.Store;
using invoice.Core.Entities;
using invoice.Core.Entities.utils;
using invoice.Core.Enums;
using invoice.Core.Interfaces.Services;
using invoice.Models.Entities.utils;
using invoice.Repo;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System.Linq.Expressions;

namespace invoice.Services
{
    public class PaymentLinkService : IPaymentLinkService
    {
        private readonly IRepository<Invoice> _invoiceRepo;
        private readonly IRepository<PaymentLink> _paymentLinkRepo;
        private readonly IFileService _fileService;
        private readonly IRepository<Client> _clientRepo;
        private readonly IRepository<InvoiceItem> _invoiceItemRepo;
        private readonly IMapper _mapper;

        public PaymentLinkService(
            IRepository<PaymentLink> paymentLinkRepo,
            IRepository<Invoice> invoiceRepo,
            IRepository<InvoiceItem> invoiceItemRepo,
            IRepository<Client> clientRepo,
            IFileService fileService,
            IMapper mapper)
        {
            _paymentLinkRepo = paymentLinkRepo;
            _invoiceRepo = invoiceRepo;
            _clientRepo = clientRepo;
            _invoiceItemRepo = invoiceItemRepo;
            _fileService = fileService;
            _mapper = mapper;
        }

        public async Task<GeneralResponse<PaymentLinkReadDTO>> CreateAsync(PaymentLinkCreateDTO dto, string userId)
        {
            if (dto == null)
                return new GeneralResponse<PaymentLinkReadDTO>(false, "Payment link data is required");


            var entity = _mapper.Map<PaymentLink>(dto);
            entity.UserId = userId;

            var exists = await _paymentLinkRepo.GetBySlugAsync(dto.Slug);
            if (exists != null)
            {
                return new GeneralResponse<PaymentLinkReadDTO>
                {
                    Success = false,
                    Message = "Slug already exists, please choose another name."
                };
            }

            entity.PaymentOptions = new PaymentOptions();
            entity.purchaseOptions = new PurchaseCompletionOptions();


            var resp = await _paymentLinkRepo.AddAsync(entity);
            if (!resp.Success)
                return new GeneralResponse<PaymentLinkReadDTO>(false, resp.Message);


            return new GeneralResponse<PaymentLinkReadDTO>(
                true,
                "payment link created successfully",
                _mapper.Map<PaymentLinkReadDTO>(resp.Data)
            );


        }

        public async Task<GeneralResponse<PaymentLinkReadDTO>> UpdateAsync(string id, PaymentLinkUpdateDTO dto, string userId)
        {
            if (string.IsNullOrWhiteSpace(id))
                return new GeneralResponse<PaymentLinkReadDTO> { Success = false, Message = "Payment link ID is required" };

            if (dto == null)
                return new GeneralResponse<PaymentLinkReadDTO> { Success = false, Message = "Payment link data is required" };

            var existing = await _paymentLinkRepo.GetByIdAsync(id, userId);
            if (existing == null)
                return new GeneralResponse<PaymentLinkReadDTO> { Success = false, Message = "Payment link not found" };

            _mapper.Map(dto, existing);
            existing.UpdatedAt = DateTime.UtcNow;

            var exists = await _paymentLinkRepo.GetBySlugAsync(dto.Slug);
            if (exists != null && exists.Id != id)
            {
                return new GeneralResponse<PaymentLinkReadDTO>
                {
                    Success = false,
                    Message = "Slug already exists, please choose another name."
                };
            }



            var result = await _paymentLinkRepo.UpdateAsync(existing);
            if (!result.Success)
                return new GeneralResponse<PaymentLinkReadDTO> { Success = false, Message = "Failed to update payment link" };

            return new GeneralResponse<PaymentLinkReadDTO>
            {
                Success = true,
                Message = "Payment link updated successfully",
                Data = _mapper.Map<PaymentLinkReadDTO>(result.Data)
            };
        }

        public async Task<GeneralResponse<IEnumerable<PaymentLinkReadDTO>>> UpdateRangeAsync(IEnumerable<PaymentLinkUpdateDTO> dtos, string userId)
        {
            if (dtos == null || !dtos.Any())
                return new GeneralResponse<IEnumerable<PaymentLinkReadDTO>> { Success = false, Message = "Payment link data is required" };

            var updatedLinks = new List<PaymentLink>();

            foreach (var dto in dtos)
            {
                //if (string.IsNullOrWhiteSpace(dto.Id))
                //    continue;

                //var existing = await _paymentLinkRepo.GetByIdAsync(dto.Id, userId);
                //if (existing == null) continue;

                //_mapper.Map(dto, existing);
                //existing.UpdatedAt = DateTime.UtcNow;

                //if (dto.Image != null)
                //{
                //    existing.Image = await _fileService.UpdateImageAsync(dto.Image, existing.Image, "paymentlinks");
                //}

                //var result = await _paymentLinkRepo.UpdateAsync(existing);
                //if (result.Success)
                //    updatedLinks.Add(result.Data);
            }

            return new GeneralResponse<IEnumerable<PaymentLinkReadDTO>>
            {
                Success = true,
                Message = updatedLinks.Any() ? "Payment links updated successfully" : "No payment links were updated",
                Data = _mapper.Map<IEnumerable<PaymentLinkReadDTO>>(updatedLinks)
            };
        }

        public async Task<GeneralResponse<bool>> DeleteAsync(string id, string userId)
        {
            if (string.IsNullOrWhiteSpace(id))
                return new GeneralResponse<bool> { Success = false, Message = "Payment link ID is required" };

            var result = await _paymentLinkRepo.DeleteAsync(id);
            return new GeneralResponse<bool>
            {
                Success = result.Success,
                Message = result.Success ? "Payment link deleted successfully" : result.Message,
                Data = result.Success
            };
        }

        public async Task<GeneralResponse<bool>> DeleteRangeAsync(IEnumerable<string> ids, string userId)
        {
            if (ids == null || !ids.Any())
                return new GeneralResponse<bool> { Success = false, Message = "Payment link IDs are required" };

            var results = new List<bool>();

            foreach (var id in ids)
            {
                if (string.IsNullOrWhiteSpace(id))
                    continue;

                var result = await _paymentLinkRepo.DeleteAsync(id);
                results.Add(result.Success);
            }

            return new GeneralResponse<bool>
            {
                Success = true,
                Message = "Batch delete executed",
                Data = results.All(r => r)
            };
        }

        public async Task<GeneralResponse<PaymentLinkReadDTO>> GetByIdAsync(string id, string userId)
        {
            if (string.IsNullOrWhiteSpace(id))
                return new GeneralResponse<PaymentLinkReadDTO> { Success = false, Message = "Payment link ID is required" };

            var entity = await _paymentLinkRepo.GetByIdAsync(id, userId, q =>q
            .Include(i =>i.PaymentLinkPayments).ThenInclude(l => l.Invoice)
                
                );
            if (entity == null)
                return new GeneralResponse<PaymentLinkReadDTO> { Success = false, Message = "Payment link not found" };

            return new GeneralResponse<PaymentLinkReadDTO>
            {
                Success = true,
                Message = "Payment link retrieved successfully",
                Data = _mapper.Map<PaymentLinkReadDTO>(entity)
            };
        }

        public async Task<GeneralResponse<IEnumerable<GetAllPaymentLinkDTO>>> GetAllAsync(string userId)
        {
            var entities = await _paymentLinkRepo.GetAllAsync(userId);

            return new GeneralResponse<IEnumerable<GetAllPaymentLinkDTO>>
            {
                Success = true,
                Message = entities.Any() ? "Payment links retrieved successfully" : "No payment links found",
                Data = _mapper.Map<IEnumerable<GetAllPaymentLinkDTO>>(entities)
            };
        }
        public async Task<GeneralResponse<PaymentLinkWithUserDTO>> GetBySlug(string slug)
        {
            var entity = await _paymentLinkRepo.GetBySlugAsync(slug,
            q => q.Include(pl => pl.User));
            if (entity == null)
                return new GeneralResponse<PaymentLinkWithUserDTO>(false, "payment link not found");

            return new GeneralResponse<PaymentLinkWithUserDTO>(true, "payment link retrieved successfully", _mapper.Map<PaymentLinkWithUserDTO>(entity));
        }

        public async Task<GeneralResponse<bool>> ActivatePaymentLinkAsync(string id, string userId)
        {

            if (string.IsNullOrWhiteSpace(id))
                return new GeneralResponse<bool> { Success = false, Message = "Payment link ID is required" };

            var entity = await _paymentLinkRepo.GetByIdAsync(id, userId);
            if (entity == null)
                return new GeneralResponse<bool> { Success = false, Message = "Payment link not found" };


            entity.IsActivated = !entity.IsActivated;
            await _paymentLinkRepo.UpdateAsync(entity);

            return new GeneralResponse<bool>(true, "payment link updated successfully", true);
        }

        public async Task<GeneralResponse<IEnumerable<PaymentLinkReadDTO>>> QueryAsync(Expression<Func<PaymentLink, bool>> predicate, string userId)
        {
            if (predicate == null)
                return new GeneralResponse<IEnumerable<PaymentLinkReadDTO>>
                {
                    Success = false,
                    Message = "Predicate is required"
                };

            var entities = await _paymentLinkRepo.QueryAsync(predicate);

            if (!string.IsNullOrEmpty(userId))
                entities = entities.Where(pl => pl.UserId == userId);

            return new GeneralResponse<IEnumerable<PaymentLinkReadDTO>>
            {
                Success = true,
                Message = "Query executed successfully",
                Data = _mapper.Map<IEnumerable<PaymentLinkReadDTO>>(entities)
            };
        }

        public async Task<int> CountAsync(string userId)
        {
            return await _paymentLinkRepo.CountAsync(userId);

        }
        public async Task<bool> ExistsAsync(string id, string userId)
        {
            if (string.IsNullOrWhiteSpace(id))
                return false;

            var entity = await _paymentLinkRepo.GetByIdAsync(id, userId);
            return entity != null;
        }


        #region Create a Payment

        public async Task<GeneralResponse<object>> CreatePaymentAsync(CreatePaymentDTO dto, string id ,string userId)
        {
            if (dto == null || string.IsNullOrWhiteSpace(userId))
                return new GeneralResponse<object> { Success = false, Message = "Payment link  data and UserId are required." };

            var paymentlink = await _paymentLinkRepo.GetByIdAsync(id, userId);

            if (paymentlink == null)
                return new GeneralResponse<object> { Success = false, Message = "Payment link not found" };

            //check

            if (paymentlink.MaxPaymentsNumber != null &&((paymentlink.MaxPaymentsNumber - paymentlink.PaymentsNumber) >= dto.PaymentsNumber ))

                return new GeneralResponse<object> { Success = false, Message = "Payment link expired" }; 
            
            if(paymentlink.ExpireDate != null &&( HelperFunctions.GetSaudiTime() > paymentlink.ExpireDate))

                return new GeneralResponse<object> { Success = false, Message = "Payment link expired" };

            //client

            string ClientId;
            var EmailExists = await _clientRepo.ExistsAsync(c => c.Email == dto.Client.Email && c.UserId == userId);
            if (EmailExists)
            {

                var client = (await _clientRepo.QueryAsync(c => c.UserId == userId && c.Email == dto.Client.Email && !c.IsDeleted)).First();
                _mapper.Map(dto.Client, client);

                var result = await _clientRepo.UpdateAsync(client);
                ClientId = result.Data.Id;

            }
            else
            {
                var entity = _mapper.Map<Client>(dto.Client);
                entity.UserId = userId;
                var result = await _clientRepo.AddAsync(entity);

                if (!result.Success) return new GeneralResponse<object>(false, result.Message);
                var dtoResult = _mapper.Map<ClientReadDTO>(result.Data);
                ClientId = dtoResult.Id;
            }


            //invoice

            var invoice = new Invoice();
            invoice.UserId = userId;
            invoice.ClientId = ClientId;
            invoice.Code = $"INV-{DateTime.UtcNow.Ticks}";
            invoice.InvoiceStatus = InvoiceStatus.Paid;
            invoice.InvoiceType = InvoiceType.PaymentLink;
            invoice.Value = paymentlink.Value * dto.PaymentsNumber;
            invoice.FinalValue = paymentlink.Value * dto.PaymentsNumber;
            invoice.LanguageId = "ar";

           

            invoice.PaymentLinkPayment = _mapper.Map<PaymentLinkPayments>(dto);
            invoice.PaymentLinkPayment.InvoiceId = invoice.Id;
            invoice.PaymentLinkPayment.PaymentLinkId = id;

         await _invoiceRepo.AddAsync(invoice);


            //update paymentlink
            paymentlink.PaymentsNumber += dto.PaymentsNumber;
            await _paymentLinkRepo.UpdateAsync(paymentlink);

            return new GeneralResponse<object>
            {
                Success = true,
                Message = "Order created successfully.",
                Data = (new
                {
                    InvoiceId = invoice.Id,
                    InvoiceCode = invoice.Code
                })

            };

        }
    
    #endregion
    }
}