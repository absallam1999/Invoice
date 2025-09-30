using AutoMapper;
using invoice.Core.DTO;
using invoice.Core.DTO.Invoice;
using invoice.Core.DTO.InvoiceItem;
using invoice.Core.DTO.PaymentLink;
using invoice.Core.Entities;
using invoice.Core.Enums;
using invoice.Core.Interfaces.Services;
using invoice.Repo;
using invoice.Repo.Data;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using System.Xml;

namespace invoice.Services
{
    public class PaymentLinkService : IPaymentLinkService
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly IRepository<Invoice> _invoiceRepo;
        private readonly IRepository<PaymentLink> _paymentLinkRepo;
        private readonly IFileService _fileService;
        private readonly IMapper _mapper;

        public PaymentLinkService(
            ApplicationDbContext dbContext,
            IRepository<PaymentLink> paymentLinkRepo,
            IRepository<Invoice> invoiceRepo,
            IFileService fileService,
            IMapper mapper)
        {
            _paymentLinkRepo = paymentLinkRepo;
            _invoiceRepo = invoiceRepo;
            _fileService = fileService;
            _dbContext = dbContext;
            _mapper = mapper;
        }

        public async Task<GeneralResponse<PaymentLinkReadDTO>> CreateAsync(PaymentLinkCreateDTO dto, string userId)
        {
            if (dto == null)
                return new GeneralResponse<PaymentLinkReadDTO>(false, "Payment link data is required");

            var strategy = _dbContext.Database.CreateExecutionStrategy();
            return await strategy.ExecuteAsync(async () =>
            {
                await using var tx = await _dbContext.Database.BeginTransactionAsync();
                try
                {
                    var invoiceDto = new InvoiceCreateDTO
                    {
                        ClientId = null,
                        LanguageId = dto.LanguageId,
                        InvoiceType = InvoiceType.Detailed,
                        
                        TermsConditions = string.IsNullOrWhiteSpace(dto.Terms)
                            ? "Auto-generated invoice for Payment Link"
                            : dto.Terms,
                        Tax = dto.Tax ?? false,
                        DiscountType = dto.DiscountType,
                        DiscountValue = dto.DiscountValue,
                        InvoiceItems = new List<InvoiceItemCreateDTO>
                        {
                            new InvoiceItemCreateDTO
                            {
                                ProductId = null,
                                Quantity = dto.PaymentsNumber > 0 ? dto.PaymentsNumber : 1,
                            }
                        }
                    };
                    var newInvoice = _mapper.Map<Invoice>(invoiceDto);
                    newInvoice.UserId = userId;
                    newInvoice.Code = $"INV-{DateTime.UtcNow.Ticks}";
                    newInvoice.InvoiceStatus = InvoiceStatus.Active;
                    newInvoice.Value = dto.Value;

                    decimal finalValue = dto.Value;

                    if (invoiceDto.DiscountValue.HasValue && invoiceDto.DiscountValue.Value > 0)
                    {
                        if (invoiceDto.DiscountType == DiscountType.Amount)
                        {
                            finalValue = dto.Value - invoiceDto.DiscountValue.Value;
                        }
                        else if (invoiceDto.DiscountType == DiscountType.Percentage)
                        {
                            finalValue = dto.Value - (dto.Value * (invoiceDto.DiscountValue.Value / 100m));
                        }
                    }

                    newInvoice.FinalValue = finalValue < 0 ? 0 : finalValue;

                    var invoiceResponse = await _invoiceRepo.AddAsync(newInvoice);
                    if (!invoiceResponse.Success)
                    {
                        await tx.RollbackAsync();
                        return new GeneralResponse<PaymentLinkReadDTO>(false,
                            $"Failed to auto-generate invoice: {invoiceResponse.Message}");
                    }

                    var paymentLink = _mapper.Map<PaymentLink>(dto);
                    paymentLink.InvoiceId = invoiceResponse.Data.Id;
                    paymentLink.CreatedAt = DateTime.UtcNow;
                    paymentLink.UpdatedAt = DateTime.UtcNow;
                    paymentLink.CreatedBy = userId;

                    if (dto.Image != null)
                    {
                        paymentLink.Image = await _fileService.UploadImageAsync(dto.Image, "paymentlinks");
                    }

                    var response = await _paymentLinkRepo.AddAsync(paymentLink);
                    if (!response.Success)
                    {
                        await tx.RollbackAsync();
                        return new GeneralResponse<PaymentLinkReadDTO>(false, "Failed to create payment link");
                    }

                    await tx.CommitAsync();

                    return new GeneralResponse<PaymentLinkReadDTO>(
                        true,
                        "Payment link created successfully with auto-generated invoice",
                        _mapper.Map<PaymentLinkReadDTO>(response.Data)
                    );
                }
                catch (Exception ex)
                {
                    await tx.RollbackAsync();
                    return new GeneralResponse<PaymentLinkReadDTO>(false,
                        $"Failed to create PaymentLink and Invoice: {ex.Message}{(ex.InnerException != null ? " --> " + ex.InnerException.Message : string.Empty)}");
                }
            });
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

            if (dto.Image != null)
            {
                existing.Image = await _fileService.UpdateImageAsync(dto.Image, existing.Image, "paymentlinks");
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
                if (string.IsNullOrWhiteSpace(dto.Id))
                    continue;

                var existing = await _paymentLinkRepo.GetByIdAsync(dto.Id, userId);
                if (existing == null) continue;

                _mapper.Map(dto, existing);
                existing.UpdatedAt = DateTime.UtcNow;

                if (dto.Image != null)
                {
                    existing.Image = await _fileService.UpdateImageAsync(dto.Image, existing.Image, "paymentlinks");
                }

                var result = await _paymentLinkRepo.UpdateAsync(existing);
                if (result.Success)
                    updatedLinks.Add(result.Data);
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

            var entity = await _paymentLinkRepo.GetByIdAsync(id, userId);
            if (entity == null)
                return new GeneralResponse<PaymentLinkReadDTO> { Success = false, Message = "Payment link not found" };

            return new GeneralResponse<PaymentLinkReadDTO>
            {
                Success = true,
                Message = "Payment link retrieved successfully",
                Data = _mapper.Map<PaymentLinkReadDTO>(entity)
            };
        }

        public async Task<GeneralResponse<IEnumerable<PaymentLinkReadDTO>>> GetAllAsync(string userId)
        {
            var entities = await _paymentLinkRepo.GetAllAsync(userId);

            return new GeneralResponse<IEnumerable<PaymentLinkReadDTO>>
            {
                Success = true,
                Message = entities.Any() ? "Payment links retrieved successfully" : "No payment links found",
                Data = _mapper.Map<IEnumerable<PaymentLinkReadDTO>>(entities)
            };
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
                entities = entities.Where(pl => pl.CreatedBy == userId);

            return new GeneralResponse<IEnumerable<PaymentLinkReadDTO>>
            {
                Success = true,
                Message = "Query executed successfully",
                Data = _mapper.Map<IEnumerable<PaymentLinkReadDTO>>(entities)
            };
        }

        public async Task<int> CountAsync(string userId)
        {
            var links = await _paymentLinkRepo.QueryAsync(pl => string.IsNullOrEmpty(userId) || pl.CreatedBy == userId);
            return links.Count();
        }

        public async Task<int> CountActiveAsync(string userId)
        {
            var links = await _paymentLinkRepo.QueryAsync(pl => pl.IsActive && (string.IsNullOrEmpty(userId) || pl.CreatedBy == userId));
            return links.Count();
        }

        public async Task<int> CountByInvoiceAsync(string invoiceId, string userId)
        {
            var links = await _paymentLinkRepo.QueryAsync(pl => pl.InvoiceId == invoiceId && (string.IsNullOrEmpty(userId) || pl.CreatedBy == userId));
            return links.Count();
        }

        public async Task<bool> ExistsAsync(string id, string userId)
        {
            if (string.IsNullOrWhiteSpace(id))
                return false;

            var entity = await _paymentLinkRepo.GetByIdAsync(id, userId);
            return entity != null;
        }
    }
}