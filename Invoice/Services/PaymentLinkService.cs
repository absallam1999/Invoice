using AutoMapper;
using invoice.Core.DTO;
using invoice.Core.DTO.Client;
using invoice.Core.DTO.PaymentLink;
using invoice.Core.Entities;
using invoice.Core.Entities.utils;
using invoice.Core.Enums;
using invoice.Core.Interfaces.Services;
using invoice.Helpers;
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
        private readonly IRepository<ApplicationUser> _ApplicationUserRepo;
        private readonly IMapper _mapper;

        public PaymentLinkService(
            IRepository<PaymentLink> paymentLinkRepo,
            IRepository<Invoice> invoiceRepo,
            IRepository<InvoiceItem> invoiceItemRepo,
            IRepository<Client> clientRepo,
            IRepository<ApplicationUser> applicationUserRepo,
            IFileService fileService,
            IMapper mapper)
        {
            _paymentLinkRepo = paymentLinkRepo;
            _invoiceRepo = invoiceRepo;
            _clientRepo = clientRepo;
            _ApplicationUserRepo= applicationUserRepo;
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
            entity.ExpireDate = GetSaudiTime.Now().AddDays(7);

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
            var user = await _ApplicationUserRepo.GetByIdAsync(userId);

            _mapper.Map(dto, existing);
            existing.UpdatedAt = GetSaudiTime.Now();

            if (user.TabAccountId == null)
            {
                dto.PaymentOptions.PayPal = false;
            }

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

            var transaction = await _paymentLinkRepo.BeginTransactionAsync();

            try
            {
                var existing = await _paymentLinkRepo.GetByIdAsync(id, userId, q => q);
                if (existing == null)
                {
                    await _paymentLinkRepo.RollbackTransactionAsync(transaction);
                    return new GeneralResponse<bool> { Success = false, Message = "Payment link not found", Data = false };
                }

                existing.DeletedAt = GetSaudiTime.Now();
                await _paymentLinkRepo.UpdateAsync(existing);

                var result = await _paymentLinkRepo.DeleteAsync(id);
                if (!result.Success)
                    throw new Exception(result.Message);

                await _paymentLinkRepo.CommitTransactionAsync(transaction);

                return new GeneralResponse<bool>
                {
                    Success = true,
                    Message = "Payment link deleted successfully",
                    Data = true
                };
            }
            catch (Exception ex)
            {
                await _paymentLinkRepo.RollbackTransactionAsync(transaction);
                return new GeneralResponse<bool>
                {
                    Success = false,
                    Message = "Error deleting payment link: " + ex.Message,
                    Data = false
                };
            }
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
            q => q.Include(pl => pl.User)
            .Include(pl=>pl.User.Tax));
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

        public async Task<GeneralResponse<object>> CreatePaymentAsync(CreatePaymentDTO dto, string id, string userId)
        {
            if (dto == null || string.IsNullOrWhiteSpace(userId))
                return new GeneralResponse<object> { Success = false, Message = "Payment link data and UserId are required." };

            var transaction = await _invoiceRepo.BeginTransactionAsync();

            try
            {
                var paymentlink = await _paymentLinkRepo.GetByIdAsync(id, userId);
                var user = await _ApplicationUserRepo.GetByIdAsync(userId, include: q => q.Include(u => u.Tax));

                if (paymentlink == null) throw new Exception("Payment link not found");

                // check
                if (paymentlink.MaxPaymentsNumber != null &&
                    (paymentlink.MaxPaymentsNumber - paymentlink.PaymentsNumber) < dto.PaymentsNumber)
                    throw new Exception("There are not enough payments left");

                if (paymentlink.ExpireDate != null && GetSaudiTime.Now() > paymentlink.ExpireDate)
                    throw new Exception("Payment link expired");

                // client
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
                    if (!result.Success) throw new Exception(result.Message);
                    ClientId = result.Data.Id;
                }

                // invoice
                var invoice = new Invoice
                {
                    UserId = userId,
                    ClientId = ClientId,
                    Code = $"INV-{DateTime.UtcNow.Ticks}",
                    InvoiceStatus = InvoiceStatus.Paid,
                    InvoiceType = InvoiceType.PaymentLink,
                    Value = paymentlink.Value * dto.PaymentsNumber,
                    FinalValue = paymentlink.Value * dto.PaymentsNumber,
                    LanguageId = "ar",
                    PaymentLinkPayment = _mapper.Map<PaymentLinkPayments>(dto)
                };
                invoice.PaymentLinkPayment.InvoiceId = invoice.Id;
                invoice.PaymentLinkPayment.PaymentLinkId = id;

                // tax
                if (paymentlink.PaymentOptions.Tax && user.Tax?.Value > 0)
                {
                    var taxRate = user.Tax.Value / 100m;
                    invoice.FinalValue += invoice.FinalValue * taxRate;
                    invoice.TaxId = user.Tax.Id;
                    invoice.HaveTax = true;
                }

                await _invoiceRepo.AddAsync(invoice);

                paymentlink.PaymentsNumber += dto.PaymentsNumber;
                await _paymentLinkRepo.UpdateAsync(paymentlink);

                await _invoiceRepo.CommitTransactionAsync(transaction);

                return new GeneralResponse<object>
                {
                    Success = true,
                    Message = "Payment created successfully.",
                    Data = new
                    {
                        InvoiceId = invoice.Id,
                        InvoiceCode = invoice.Code
                    }
                };
            }
            catch (Exception ex)
            {
              
                await _invoiceRepo.RollbackTransactionAsync(transaction);
                return new GeneralResponse<object>
                {
                    Success = false,
                    Message = "Error creating payment: " + ex.Message,
                    Data = null
                };
            }
        }

        #endregion
    }
}