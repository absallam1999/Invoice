using AutoMapper;
using invoice.Core.DTO;
using invoice.Core.DTO.Payment;
using invoice.Core.DTO.PaymentLink;
using invoice.Core.Entites;
using invoice.Core.Enums;
using invoice.Core.Interfaces.Services;
using invoice.Repo;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace invoice.Services
{
    public class PaymentLinkService : IPaymentLinkService
    {
        private readonly IRepository<Invoice> _invoiceRepo;
        private readonly IRepository<PaymentLink> _paymentLinkRepo;
        private readonly IPaymentGatewayService _gatewayService;
        private readonly IFileService _fileService;
        private readonly IMapper _mapper;

        public PaymentLinkService(
            IRepository<PaymentLink> paymentLinkRepo,
            IPaymentGatewayService gatewayService,
            IRepository<Invoice> invoiceRepo,
            IFileService fileService,
            IMapper mapper)
        {
            _paymentLinkRepo = paymentLinkRepo;
            _gatewayService = gatewayService;
            _invoiceRepo = invoiceRepo;
            _fileService = fileService;
            _mapper = mapper;
        }

        public async Task<GeneralResponse<PaymentLinkReadDTO>> CreateAsync(PaymentLinkCreateDTO dto, string userId)
        {
            if (dto == null)
                return new GeneralResponse<PaymentLinkReadDTO> { Success = false, Message = "Payment link data is required" };

            if (!string.IsNullOrEmpty(dto.InvoiceId))
            {
                var invoice = await _invoiceRepo.GetByIdAsync(dto.InvoiceId, userId);
                if (invoice == null)
                    return new GeneralResponse<PaymentLinkReadDTO> { Success = false, Message = "Invoice not found" };
            }

            var paymentLink = _mapper.Map<PaymentLink>(dto);
            paymentLink.CreatedAt = DateTime.UtcNow;
            paymentLink.UpdatedAt = DateTime.UtcNow;

            if (dto.Image != null)
            {
                paymentLink.Image = await _fileService.UploadImageAsync(dto.Image, "paymentlinks");
            }

            if (dto.GenerateGatewayLink && dto.Value > 0)
            {
                var paymentDto = new PaymentCreateDTO
                {
                    Name = dto.Description ?? "Custom Payment",
                    Currency = dto.Currency ?? "USD",
                    Cost = dto.Value,
                    Description = dto.Message,
                    Metadata = new Dictionary<string, string>
                {
                    { "payment_link_id", paymentLink.Id },
                    { "purpose", dto.Purpose ?? "custom_payment" }
                }
                };

                var gatewayResponse = await _gatewayService.CreatePaymentSessionAsync(paymentDto, PaymentType.Stripe);

                if (gatewayResponse.Success)
                {
                    paymentLink.Link = gatewayResponse.Data.PaymentUrl;
                    paymentLink.GatewaySessionId = gatewayResponse.Data.SessionId;
                }
            }

            var response = await _paymentLinkRepo.AddAsync(paymentLink);
            if (!response.Success)
                return new GeneralResponse<PaymentLinkReadDTO> { Success = false, Message = "Failed to create payment link" };

            return new GeneralResponse<PaymentLinkReadDTO>
            {
                Success = true,
                Message = "Payment link created successfully",
                Data = _mapper.Map<PaymentLinkReadDTO>(response.Data)
            };
        }

        public async Task<GeneralResponse<PaymentLinkReadDTO>> CreateCustomPaymentLinkAsync(CustomPaymentLinkCreateDTO dto, string userId)
        {
            if (dto == null)
                return new GeneralResponse<PaymentLinkReadDTO> { Success = false, Message = "Payment link data is required" };

            if (dto.Value <= 0)
                return new GeneralResponse<PaymentLinkReadDTO> { Success = false, Message = "Payment value must be greater than zero" };

            var paymentLink = new PaymentLink
            {
                Purpose = dto.Purpose,
                Value = dto.Value,
                Currency = dto.Currency ?? "USD",
                Description = dto.Description ?? $"Payment for {dto.Purpose}",
                Message = dto.Message,
                IsActive = true,
                ExpiresAt = dto.ExpiresAt,
                MaxUsageCount = dto.MaxUsageCount,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            if (dto.Image != null)
            {
                paymentLink.Image = await _fileService.UploadImageAsync(dto.Image, "paymentlinks");
            }

            var paymentDto = new PaymentCreateDTO
            {
                Name = dto.Description ?? $"Payment - {dto.Purpose}",
                Currency = dto.Currency ?? "USD",
                Cost = dto.Value,
                Description = dto.Message,
                Metadata = new Dictionary<string, string>
            {
                { "purpose", dto.Purpose },
                { "custom_payment", "true" },
                { "created_by", userId }
            }
            };

            var gatewayResponse = await _gatewayService.CreatePaymentSessionAsync(paymentDto, dto.PaymentType);
            if (!gatewayResponse.Success)
                return new GeneralResponse<PaymentLinkReadDTO> { Success = false, Message = "Failed to create payment gateway session" };

            paymentLink.Link = gatewayResponse.Data.PaymentUrl;
            paymentLink.GatewaySessionId = gatewayResponse.Data.SessionId;

            var response = await _paymentLinkRepo.AddAsync(paymentLink);
            if (!response.Success)
                return new GeneralResponse<PaymentLinkReadDTO> { Success = false, Message = "Failed to create payment link" };

            return new GeneralResponse<PaymentLinkReadDTO>
            {
                Success = true,
                Message = "Custom payment link created successfully",
                Data = _mapper.Map<PaymentLinkReadDTO>(response.Data)
            };
        }

        public async Task<GeneralResponse<PaymentLinkReadDTO>> UpdateAsync(string id, PaymentLinkUpdateDTO dto, string userId)
        {
            if (string.IsNullOrWhiteSpace(id))
                return new GeneralResponse<PaymentLinkReadDTO> { Success = false, Message = "Payment link ID is required" };

            if (dto == null)
                return new GeneralResponse<PaymentLinkReadDTO> { Success = false, Message = "Payment link data is required" };

            var existing = await _paymentLinkRepo.GetByIdAsync(id, userId, q => q.Include(p => p.Payments));
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

            var entity = await _paymentLinkRepo.GetByIdAsync(id, userId, q => q.Include(pl => pl.Payments));
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
            var entities = await _paymentLinkRepo.GetAllAsync(userId, pl => pl.Payments);

            return new GeneralResponse<IEnumerable<PaymentLinkReadDTO>>
            {
                Success = true,
                Message = entities.Any() ? "Payment links retrieved successfully" : "No payment links found",
                Data = _mapper.Map<IEnumerable<PaymentLinkReadDTO>>(entities)
            };
        }

        public async Task<GeneralResponse<IEnumerable<PaymentLinkReadDTO>>> GetCustomPaymentLinksAsync(string userId)
        {
            var entities = await _paymentLinkRepo.QueryAsync(
                pl => string.IsNullOrEmpty(pl.InvoiceId) && (string.IsNullOrEmpty(userId) || pl.CreatedBy == userId),
                pl => pl.Payments);

            return new GeneralResponse<IEnumerable<PaymentLinkReadDTO>>
            {
                Success = true,
                Message = entities.Any() ? "Custom payment links retrieved successfully" : "No custom payment links found",
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

            var entities = await _paymentLinkRepo.QueryAsync(predicate, pl => pl.Payments);

            if (!string.IsNullOrEmpty(userId))
                entities = entities.Where(pl => pl.CreatedBy == userId);

            return new GeneralResponse<IEnumerable<PaymentLinkReadDTO>>
            {
                Success = true,
                Message = "Query executed successfully",
                Data = _mapper.Map<IEnumerable<PaymentLinkReadDTO>>(entities)
            };
        }

        public async Task<GeneralResponse<PaymentLinkReadDTO>> GenerateLinkAsync(string invoiceId, decimal value, string userId)
        {
            if (string.IsNullOrWhiteSpace(invoiceId))
                return new GeneralResponse<PaymentLinkReadDTO> { Success = false, Message = "Invoice ID is required" };

            if (value <= 0)
                return new GeneralResponse<PaymentLinkReadDTO> { Success = false, Message = "Value must be greater than zero" };

            var invoice = await _invoiceRepo.GetByIdAsync(invoiceId, userId);
            if (invoice == null)
                return new GeneralResponse<PaymentLinkReadDTO> { Success = false, Message = "Invoice not found" };

            var paymentLink = new PaymentLink
            {
                InvoiceId = invoice.Id,
                Value = value,
                Currency = invoice.Currency ?? "USD",
                PaymentsNumber = "1",
                Description = $"Payment for invoice {invoice.Code}",
                Message = "Generated automatically",
                IsActive = true,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            var paymentDto = new PaymentCreateDTO
            {
                Name = $"Invoice Payment - {invoice.Code}",
                Currency = invoice.Currency ?? "USD",
                Cost = value,
                InvoiceId = invoiceId,
                Description = $"Payment for invoice {invoice.Code}",
                Metadata = new Dictionary<string, string>
                {
                    { "invoice_code", invoice.Code },
                    { "auto_generated", "true" }
                }
            };

            var gatewayResponse = await _gatewayService.CreatePaymentSessionAsync(paymentDto, PaymentType.Stripe);
            if (gatewayResponse.Success)
            {
                paymentLink.Link = gatewayResponse.Data.PaymentUrl;
                paymentLink.GatewaySessionId = gatewayResponse.Data.SessionId;
            }

            var result = await _paymentLinkRepo.AddAsync(paymentLink);
            if (!result.Success)
                return new GeneralResponse<PaymentLinkReadDTO> { Success = false, Message = "Failed to save payment link" };

            return new GeneralResponse<PaymentLinkReadDTO>
            {
                Success = true,
                Message = "Payment link generated successfully",
                Data = _mapper.Map<PaymentLinkReadDTO>(result.Data)
            };
        }

        public async Task<GeneralResponse<bool>> AttachPaymentAsync(string paymentLinkId, Payment payment, string userId)
        {
            if (string.IsNullOrWhiteSpace(paymentLinkId))
                return new GeneralResponse<bool> { Success = false, Message = "Payment link ID is required" };

            if (payment == null)
                return new GeneralResponse<bool> { Success = false, Message = "Payment data is required" };

            var paymentLink = await _paymentLinkRepo.GetByIdAsync(paymentLinkId, userId, q => q.Include(pl => pl.Payments));
            if (paymentLink == null)
                return new GeneralResponse<bool> { Success = false, Message = "Payment link not found", Data = false };

            if (paymentLink.MaxUsageCount.HasValue && paymentLink.Payments.Count >= paymentLink.MaxUsageCount.Value)
                return new GeneralResponse<bool> { Success = false, Message = "Payment link has reached maximum usage limit", Data = false };

            if (paymentLink.ExpiresAt.HasValue && paymentLink.ExpiresAt.Value < DateTime.UtcNow)
                return new GeneralResponse<bool> { Success = false, Message = "Payment link has expired", Data = false };

            payment.PaymentLinkId = paymentLink.Id;
            paymentLink.Payments.Add(payment);
            paymentLink.UpdatedAt = DateTime.UtcNow;

            var result = await _paymentLinkRepo.UpdateAsync(paymentLink);
            return new GeneralResponse<bool>
            {
                Success = result.Success,
                Message = result.Success ? "Payment attached successfully" : "Failed to attach payment",
                Data = result.Success
            };
        }

        public async Task<GeneralResponse<PaymentLinkReadDTO>> RefreshGatewayLinkAsync(string paymentLinkId, string userId)
        {
            if (string.IsNullOrWhiteSpace(paymentLinkId))
                return new GeneralResponse<PaymentLinkReadDTO> { Success = false, Message = "Payment link ID is required" };

            var paymentLink = await _paymentLinkRepo.GetByIdAsync(paymentLinkId, userId);
            if (paymentLink == null)
                return new GeneralResponse<PaymentLinkReadDTO> { Success = false, Message = "Payment link not found" };

            if (paymentLink.Value <= 0)
                return new GeneralResponse<PaymentLinkReadDTO> { Success = false, Message = "Payment link value must be greater than zero" };

            var paymentDto = new PaymentCreateDTO
            {
                Name = paymentLink.Description ?? "Payment Link Refresh",
                Currency = paymentLink.Currency ?? "USD",
                Cost = paymentLink.Value,
                Description = paymentLink.Message,
                Metadata = new Dictionary<string, string>
                {
                    { "payment_link_id", paymentLink.Id },
                    { "purpose", paymentLink.Purpose ?? "refresh" },
                    { "refreshed_at", DateTime.UtcNow.ToString("O") }
                }
            };

            var gatewayResponse = await _gatewayService.CreatePaymentSessionAsync(paymentDto, PaymentType.Stripe);
            if (!gatewayResponse.Success)
                return new GeneralResponse<PaymentLinkReadDTO> { Success = false, Message = "Failed to refresh payment gateway link" };

            paymentLink.Link = gatewayResponse.Data.PaymentUrl;
            paymentLink.GatewaySessionId = gatewayResponse.Data.SessionId;
            paymentLink.UpdatedAt = DateTime.UtcNow;

            var result = await _paymentLinkRepo.UpdateAsync(paymentLink);
            if (!result.Success)
                return new GeneralResponse<PaymentLinkReadDTO> { Success = false, Message = "Failed to update payment link" };

            return new GeneralResponse<PaymentLinkReadDTO>
            {
                Success = true,
                Message = "Payment gateway link refreshed successfully",
                Data = _mapper.Map<PaymentLinkReadDTO>(result.Data)
            };
        }

        public async Task<GeneralResponse<decimal>> GetTotalRevenueFromCustomLinksAsync(string userId)
        {
            var customLinks = await _paymentLinkRepo.QueryAsync(pl =>
                string.IsNullOrEmpty(pl.InvoiceId) && (string.IsNullOrEmpty(userId) || pl.CreatedBy == userId),
                q => q.Payments);

            var totalRevenue = customLinks
                .SelectMany(pl => pl.Payments)
                .Sum(p => p.Cost);

            return new GeneralResponse<decimal>
            {
                Success = true,
                Message = "Total revenue calculated successfully",
                Data = totalRevenue
            };
        }

        public async Task<GeneralResponse<bool>> ValidatePaymentLinkAsync(string paymentLinkId, string userId)
        {
            if (string.IsNullOrWhiteSpace(paymentLinkId))
                return new GeneralResponse<bool> { Success = false, Message = "Payment link ID is required" };

            var paymentLink = await _paymentLinkRepo.GetByIdAsync(paymentLinkId, userId, q => q.Include(pl => pl.Payments));
            if (paymentLink == null)
                return new GeneralResponse<bool> { Success = false, Message = "Payment link not found", Data = false };

            if (!paymentLink.IsActive)
                return new GeneralResponse<bool> { Success = false, Message = "Payment link is inactive", Data = false };

            if (paymentLink.ExpiresAt.HasValue && paymentLink.ExpiresAt.Value < DateTime.UtcNow)
                return new GeneralResponse<bool> { Success = false, Message = "Payment link has expired", Data = false };

            if (paymentLink.MaxUsageCount.HasValue && paymentLink.Payments.Count >= paymentLink.MaxUsageCount.Value)
                return new GeneralResponse<bool> { Success = false, Message = "Payment link has reached usage limit", Data = false };

            return new GeneralResponse<bool>
            {
                Success = true,
                Message = "Payment link is valid",
                Data = true
            };
        }

        public async Task<GeneralResponse<bool>> DeactivateExpiredLinksAsync(string userId)
        {
            var expiredLinks = await _paymentLinkRepo.QueryAsync(
                pl => pl.IsActive &&
                      ((pl.ExpiresAt.HasValue && pl.ExpiresAt.Value < DateTime.UtcNow) ||
                       (pl.MaxUsageCount.HasValue && pl.Payments.Count >= pl.MaxUsageCount.Value)) &&
                      (string.IsNullOrEmpty(userId) || pl.CreatedBy == userId),
                pl => pl.Payments);

            foreach (var link in expiredLinks)
            {
                link.IsActive = false;
                link.UpdatedAt = DateTime.UtcNow;
                await _paymentLinkRepo.UpdateAsync(link);
            }

            return new GeneralResponse<bool>
            {
                Success = true,
                Message = "Expired links deactivated successfully",
                Data = true
            };
        }

        public async Task<GeneralResponse<IEnumerable<PaymentLinkReadDTO>>> GetActiveLinksAsync(string userId)
        {
            var activeLinks = await _paymentLinkRepo.QueryAsync(
                pl => pl.IsActive && (string.IsNullOrEmpty(userId) || pl.CreatedBy == userId),
                pl => pl.Payments);

            return new GeneralResponse<IEnumerable<PaymentLinkReadDTO>>
            {
                Success = true,
                Message = activeLinks.Any() ? "Active links retrieved successfully" : "No active links found",
                Data = _mapper.Map<IEnumerable<PaymentLinkReadDTO>>(activeLinks)
            };
        }

        public async Task<GeneralResponse<IEnumerable<PaymentLinkReadDTO>>> GetExpiringSoonLinksAsync(int daysThreshold, string userId)
        {
            var thresholdDate = DateTime.UtcNow.AddDays(daysThreshold);

            var expiringLinks = await _paymentLinkRepo.QueryAsync(
                pl => pl.IsActive &&
                      pl.ExpiresAt.HasValue &&
                      pl.ExpiresAt.Value <= thresholdDate &&
                      pl.ExpiresAt.Value > DateTime.UtcNow &&
                      (string.IsNullOrEmpty(userId) || pl.CreatedBy == userId),
                pl => pl.Payments);

            return new GeneralResponse<IEnumerable<PaymentLinkReadDTO>>
            {
                Success = true,
                Message = expiringLinks.Any() ? "Expiring soon links retrieved successfully" : "No expiring links found",
                Data = _mapper.Map<IEnumerable<PaymentLinkReadDTO>>(expiringLinks)
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