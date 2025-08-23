using AutoMapper;
using invoice.Core.DTO.Invoice;
using invoice.Core.DTO.Payment;
using invoice.Core.DTO.PaymentLink;
using invoice.Core.Interfaces.Services;
using invoice.Core.DTO;
using invoice.Core.Entites;
using invoice.Core.Enums;
using invoice.Repo;
using Microsoft.EntityFrameworkCore;

namespace invoice.Services
{
    public class InvoiceService : IInvoiceService
    {
        private readonly IRepository<InvoiceItem> _invoiceItemRepo;
        private readonly IRepository<Invoice> _invoiceRepo;
        private readonly IRepository<Client> _clientRepo;
        private readonly IRepository<Store> _storeRepo;
        private readonly IRepository<Payment> _paymentRepo;
        private readonly IRepository<PaymentLink> _paymentLinkRepo;
        private readonly IMapper _mapper;

        public InvoiceService(
            IRepository<InvoiceItem> invoiceItemRepo,
            IRepository<Invoice> invoiceRepo,
            IRepository<Client> clientRepo,
            IRepository<Store> storeRepo,
            IRepository<Payment> paymentRepo,
            IRepository<PaymentLink> paymentLinkRepo,
            IMapper mapper)
        {
            _invoiceItemRepo = invoiceItemRepo;
            _invoiceRepo = invoiceRepo;
            _clientRepo = clientRepo;
            _storeRepo = storeRepo;
            _paymentRepo = paymentRepo;
            _paymentLinkRepo = paymentLinkRepo;
            _mapper = mapper;
        }


        public async Task<GeneralResponse<IEnumerable<InvoiceReadDTO>>> GetAllAsync(string userId)
        {
            if (string.IsNullOrWhiteSpace(userId))
                return new GeneralResponse<IEnumerable<InvoiceReadDTO>> { Success = false, Message = "UserId is required.", Data = Enumerable.Empty<InvoiceReadDTO>() };

            var invoices = await _invoiceRepo.GetAllAsync(userId, x => x.Client, x => x.Store, x => x.InvoiceItems, x => x.Payments, x => x.PaymentLinks, x => x.PayInvoices);

            if (!invoices.Any())
                return new GeneralResponse<IEnumerable<InvoiceReadDTO>> { Success = false, Message = "No invoices found.", Data = Enumerable.Empty<InvoiceReadDTO>() };

            return new GeneralResponse<IEnumerable<InvoiceReadDTO>>
            {
                Success = true,
                Message = "Invoices retrieved successfully.",
                Data = _mapper.Map<IEnumerable<InvoiceReadDTO>>(invoices)
            };
        }

        public async Task<GeneralResponse<InvoiceReadDTO>> GetByIdAsync(string id, string userId)
        {
            if (string.IsNullOrWhiteSpace(id) || string.IsNullOrWhiteSpace(userId))
                return new GeneralResponse<InvoiceReadDTO> { Success = false, Message = "Id and UserId are required." };

            var invoice = await _invoiceRepo.GetByIdAsync(id, userId, q => q
                .Include(x => x.Client)
                .Include(x => x.Store)
                .Include(x => x.InvoiceItems)
                .Include(x => x.PayInvoices)
                .Include(x => x.PaymentLinks)
                .Include(x => x.Payments));

            if (invoice == null)
                return new GeneralResponse<InvoiceReadDTO> { Success = false, Message = $"Invoice with Id '{id}' not found." };

            return new GeneralResponse<InvoiceReadDTO>
            {
                Success = true,
                Message = "Invoice retrieved successfully.",
                Data = _mapper.Map<InvoiceReadDTO>(invoice)
            };
        }

        public async Task<GeneralResponse<InvoiceReadDTO>> GetByCodeAsync(string code, string userId)
        {
            if (string.IsNullOrWhiteSpace(code) || string.IsNullOrWhiteSpace(userId))
                return new GeneralResponse<InvoiceReadDTO> { Success = false, Message = "Code and UserId are required." };

            var invoices = await _invoiceRepo.GetAllAsync(userId, x => x.Client, x => x.Store, x => x.InvoiceItems, x => x.Payments, x => x.PaymentLinks, x => x.PayInvoices);
            var invoice = invoices.FirstOrDefault(i => i.Code.Equals(code, StringComparison.OrdinalIgnoreCase));

            if (invoice == null)
                return new GeneralResponse<InvoiceReadDTO> { Success = false, Message = $"Invoice with code '{code}' not found." };

            return new GeneralResponse<InvoiceReadDTO>
            {
                Success = true,
                Message = "Invoice retrieved successfully.",
                Data = _mapper.Map<InvoiceReadDTO>(invoice)
            };
        }

        public async Task<GeneralResponse<IEnumerable<InvoiceReadDTO>>> SearchAsync(string keyword, string userId)
        {
            if (string.IsNullOrWhiteSpace(userId))
                return new GeneralResponse<IEnumerable<InvoiceReadDTO>> { Success = false, Message = "UserId is required.", Data = Enumerable.Empty<InvoiceReadDTO>() };

            var invoices = await _invoiceRepo.GetAllAsync(userId, x => x.Client, x => x.Store, x => x.InvoiceItems, x => x.Payments, x => x.PaymentLinks, x => x.PayInvoices);

            var filtered = invoices.Where(i =>
                (!string.IsNullOrWhiteSpace(i.Code) && i.Code.Contains(keyword ?? "", StringComparison.OrdinalIgnoreCase)) ||
                (i.Client != null && i.Client.Name.Contains(keyword ?? "", StringComparison.OrdinalIgnoreCase)) ||
                (i.Store != null && i.Store.Name.Contains(keyword ?? "", StringComparison.OrdinalIgnoreCase))
            );

            return new GeneralResponse<IEnumerable<InvoiceReadDTO>>
            {
                Success = filtered.Any(),
                Message = filtered.Any() ? "Matching invoices found." : "No matching invoices found.",
                Data = _mapper.Map<IEnumerable<InvoiceReadDTO>>(filtered)
            };
        }

        public async Task<GeneralResponse<InvoiceReadDTO>> CreateAsync(InvoiceCreateDTO dto, string userId)
        {
            if (dto == null || string.IsNullOrWhiteSpace(userId))
                return new GeneralResponse<InvoiceReadDTO> { Success = false, Message = "Invoice data and UserId are required." };

            var client = string.IsNullOrWhiteSpace(dto.ClientId) ? null : await _clientRepo.GetByIdAsync(dto.ClientId, userId);
            if (!string.IsNullOrWhiteSpace(dto.ClientId) && client == null)
                return new GeneralResponse<InvoiceReadDTO> { Success = false, Message = $"Client '{dto.ClientId}' not found." };

            var store = string.IsNullOrWhiteSpace(dto.StoreId) ? null : await _storeRepo.GetByIdAsync(dto.StoreId, userId);
            if (!string.IsNullOrWhiteSpace(dto.StoreId) && store == null)
                return new GeneralResponse<InvoiceReadDTO> { Success = false, Message = $"Store '{dto.StoreId}' not found." };

            var invoice = _mapper.Map<Invoice>(dto);
            invoice.UserId = userId;
            invoice.Code = $"INV-{DateTime.UtcNow.Ticks}";

            await _invoiceRepo.AddAsync(invoice);

            if (dto.InvoiceItems != null && dto.InvoiceItems.Any())
            {
                foreach (var itemDto in dto.InvoiceItems)
                {
                    var item = _mapper.Map<InvoiceItem>(itemDto);
                    item.InvoiceId = invoice.Id;
                    await _invoiceItemRepo.AddAsync(item);
                    invoice.InvoiceItems.Add(item);
                }
            }

            RecalculateInvoiceTotals(invoice);

            return new GeneralResponse<InvoiceReadDTO>
            {
                Success = true,
                Message = "Invoice created successfully.",
                Data = _mapper.Map<InvoiceReadDTO>(invoice)
            };
        }

        public async Task<GeneralResponse<IEnumerable<InvoiceReadDTO>>> CreateRangeAsync(IEnumerable<InvoiceCreateDTO> dtos, string userId)
        {
            if (dtos == null || !dtos.Any() || string.IsNullOrWhiteSpace(userId))
                return new GeneralResponse<IEnumerable<InvoiceReadDTO>> { Success = false, Message = "Invoice data and UserId are required.", Data = Enumerable.Empty<InvoiceReadDTO>() };

            var createdInvoices = new List<Invoice>();

            foreach (var dto in dtos)
            {
                var client = string.IsNullOrWhiteSpace(dto.ClientId) ? null : await _clientRepo.GetByIdAsync(dto.ClientId, userId);
                if (!string.IsNullOrWhiteSpace(dto.ClientId) && client == null) continue;

                var store = string.IsNullOrWhiteSpace(dto.StoreId) ? null : await _storeRepo.GetByIdAsync(dto.StoreId, userId);
                if (!string.IsNullOrWhiteSpace(dto.StoreId) && store == null) continue;

                var invoice = _mapper.Map<Invoice>(dto);
                invoice.UserId = userId;
                invoice.Code = $"INV-{DateTime.UtcNow.Ticks}";

                await _invoiceRepo.AddAsync(invoice);

                if (dto.InvoiceItems != null && dto.InvoiceItems.Any())
                {
                    foreach (var itemDto in dto.InvoiceItems)
                    {
                        var item = _mapper.Map<InvoiceItem>(itemDto);
                        item.InvoiceId = invoice.Id;
                        await _invoiceItemRepo.AddAsync(item);
                        invoice.InvoiceItems.Add(item);
                    }
                }

                RecalculateInvoiceTotals(invoice);
                createdInvoices.Add(invoice);
            }

            return new GeneralResponse<IEnumerable<InvoiceReadDTO>>
            {
                Success = createdInvoices.Any(),
                Message = createdInvoices.Any() ? "Invoices created successfully." : "No invoices were created.",
                Data = _mapper.Map<IEnumerable<InvoiceReadDTO>>(createdInvoices)
            };
        }

        public async Task<GeneralResponse<InvoiceReadDTO>> UpdateAsync(string id, InvoiceUpdateDTO dto, string userId)
        {
            var invoice = await _invoiceRepo.GetByIdAsync(id, userId, q => q
                .Include(x => x.Client)
                .Include(x => x.Store)
                .Include(x => x.InvoiceItems)
                .Include(x => x.Payments)
                .Include(x => x.PaymentLinks));

            if (invoice == null)
                return new GeneralResponse<InvoiceReadDTO> { Success = false, Message = $"Invoice with Id '{id}' not found." };

            if (!string.IsNullOrEmpty(dto.ClientId) && dto.ClientId != invoice.ClientId)
            {
                var client = await _clientRepo.GetByIdAsync(dto.ClientId, userId);
                if (client == null)
                    return new GeneralResponse<InvoiceReadDTO> { Success = false, Message = $"Client '{dto.ClientId}' not found." };
                invoice.ClientId = dto.ClientId;
            }

            if (!string.IsNullOrEmpty(dto.StoreId) && dto.StoreId != invoice.StoreId)
            {
                var store = await _storeRepo.GetByIdAsync(dto.StoreId, userId);
                if (store == null)
                    return new GeneralResponse<InvoiceReadDTO> { Success = false, Message = $"Store '{dto.StoreId}' not found." };
                invoice.StoreId = dto.StoreId;
            }

            _mapper.Map(dto, invoice);

            if (dto.InvoiceItems != null)
            {
                var itemsToDelete = invoice.InvoiceItems
                    .Where(i => !dto.InvoiceItems.Any(d => d.Id == i.Id))
                    .ToList();

                foreach (var item in itemsToDelete)
                {
                    invoice.InvoiceItems.Remove(item);
                    await _invoiceItemRepo.DeleteAsync(item.Id);
                }

                foreach (var itemDto in dto.InvoiceItems)
                {
                    var existingItem = invoice.InvoiceItems.FirstOrDefault(i => i.Id == itemDto.Id);
                    if (existingItem != null)
                    {
                        _mapper.Map(itemDto, existingItem);
                        await _invoiceItemRepo.UpdateAsync(existingItem);
                    }
                    else
                    {
                        var newItem = _mapper.Map<InvoiceItem>(itemDto);
                        newItem.InvoiceId = invoice.Id;
                        await _invoiceItemRepo.AddAsync(newItem);
                        invoice.InvoiceItems.Add(newItem);
                    }
                }

                invoice.Value = invoice.InvoiceItems.Sum(i => i.UnitPrice * i.Quantity);
                if (invoice.DiscountType.HasValue && invoice.DiscountValue.HasValue)
                {
                    invoice.FinalValue = invoice.DiscountType switch
                    {
                        DiscountType.Amount => invoice.Value - invoice.DiscountValue.Value,
                        DiscountType.Percentage => invoice.Value * (1 - invoice.DiscountValue.Value / 100),
                        _ => invoice.Value
                    };
                }
                else
                {
                    invoice.FinalValue = invoice.Value;
                }
            }

            await _invoiceRepo.UpdateAsync(invoice);

            return new GeneralResponse<InvoiceReadDTO>
            {
                Success = true,
                Message = "Invoice updated successfully",
                Data = _mapper.Map<InvoiceReadDTO>(invoice)
            };
        }

        public async Task<GeneralResponse<IEnumerable<InvoiceReadDTO>>> UpdateRangeAsync(IEnumerable<InvoiceUpdateDTO> dtos, string userId)
        {
            var updatedInvoices = new List<Invoice>();

            foreach (var dto in dtos)
            {
                var invoice = await _invoiceRepo.GetByIdAsync(dto.Id, userId, q => q
                    .Include(x => x.Client)
                    .Include(x => x.Store)
                    .Include(x => x.InvoiceItems)
                    .Include(x => x.Payments)
                    .Include(x => x.PaymentLinks));

                if (invoice == null)
                    continue;

                if (!string.IsNullOrEmpty(dto.ClientId) && dto.ClientId != invoice.ClientId)
                {
                    var client = await _clientRepo.GetByIdAsync(dto.ClientId, userId);
                    if (client == null)
                        continue;
                    invoice.ClientId = dto.ClientId;
                }

                if (!string.IsNullOrEmpty(dto.StoreId) && dto.StoreId != invoice.StoreId)
                {
                    var store = await _storeRepo.GetByIdAsync(dto.StoreId, userId);
                    if (store == null)
                        continue;
                    invoice.StoreId = dto.StoreId;
                }

                _mapper.Map(dto, invoice);

                if (dto.InvoiceItems != null)
                {
                    var itemsToDelete = invoice.InvoiceItems
                        .Where(i => !dto.InvoiceItems.Any(d => d.Id == i.Id))
                        .ToList();

                    foreach (var item in itemsToDelete)
                    {
                        invoice.InvoiceItems.Remove(item);
                        await _invoiceItemRepo.DeleteAsync(item.Id);
                    }

                    foreach (var itemDto in dto.InvoiceItems)
                    {
                        var existingItem = invoice.InvoiceItems.FirstOrDefault(i => i.Id == itemDto.Id);
                        if (existingItem != null)
                        {
                            _mapper.Map(itemDto, existingItem);
                            await _invoiceItemRepo.UpdateAsync(existingItem);
                        }
                        else
                        {
                            var newItem = _mapper.Map<InvoiceItem>(itemDto);
                            newItem.InvoiceId = invoice.Id;
                            await _invoiceItemRepo.AddAsync(newItem);
                            invoice.InvoiceItems.Add(newItem);
                        }
                    }

                    invoice.Value = invoice.InvoiceItems.Sum(i => i.UnitPrice * i.Quantity);
                    if (invoice.DiscountType.HasValue && invoice.DiscountValue.HasValue)
                    {
                        invoice.FinalValue = invoice.DiscountType switch
                        {
                            DiscountType.Amount => invoice.Value - invoice.DiscountValue.Value,
                            DiscountType.Percentage => invoice.Value * (1 - invoice.DiscountValue.Value / 100),
                            _ => invoice.Value
                        };
                    }
                    else
                    {
                        invoice.FinalValue = invoice.Value;
                    }
                }

                updatedInvoices.Add(invoice);
            }

            if (!updatedInvoices.Any())
                return new GeneralResponse<IEnumerable<InvoiceReadDTO>>
                {
                    Success = false,
                    Message = "No invoices were updated.",
                    Data = Enumerable.Empty<InvoiceReadDTO>()
                };

            await _invoiceRepo.UpdateRangeAsync(updatedInvoices);

            return new GeneralResponse<IEnumerable<InvoiceReadDTO>>
            {
                Success = true,
                Message = "Invoices updated successfully.",
                Data = _mapper.Map<IEnumerable<InvoiceReadDTO>>(updatedInvoices)
            };
        }

        public async Task<GeneralResponse<bool>> DeleteAsync(string id, string userId)
        {
            var invoice = await _invoiceRepo.GetByIdAsync(id, userId, q => q
                                            .Include(x => x.Client)
                                            .Include(x => x.Store)
                                            .Include(x => x.InvoiceItems)
                                            .Include(x => x.PayInvoices)
                                            .Include(x => x.PaymentLinks)
                                            .Include(x => x.Payments));

            if (invoice == null)
            {
                return new GeneralResponse<bool>
                {
                    Success = false,
                    Message = $"Invoice with Id '{id}' not found.",
                    Data = false
                };
            }

            await _invoiceRepo.DeleteAsync(invoice.Id);

            return new GeneralResponse<bool>
            {
                Success = true,
                Message = "Invoice deleted successfully.",
                Data = true
            };
        }

        public async Task<GeneralResponse<bool>> DeleteRangeAsync(IEnumerable<string> ids, string userId)
        {
            var validIds = new List<string>();

            foreach (var id in ids)
            {
                var invoice = await _invoiceRepo.GetByIdAsync(id, userId);
                if (invoice != null)
                    validIds.Add(invoice.Id);
            }

            if (!validIds.Any())
            {
                return new GeneralResponse<bool>
                {
                    Success = false,
                    Message = "No invoices were deleted.",
                    Data = false
                };
            }

            await _invoiceRepo.DeleteRangeAsync(validIds);

            return new GeneralResponse<bool>
            {
                Success = true,
                Message = "Invoices deleted successfully.",
                Data = true
            };
        }


        public async Task<bool> ExistsAsync(string id, string userId)
        {
            var invoice = await _invoiceRepo.GetByIdAsync(id, userId);
            return invoice != null;
        }

        public async Task<int> CountAsync(string userId)
        {
            var invoices = await _invoiceRepo.GetAllAsync(userId);
            return invoices.Count();
        }

        public async Task<GeneralResponse<IEnumerable<InvoiceReadDTO>>> GetByClientAsync(string clientId, string userId)
        {
            var invoices = await _invoiceRepo.GetAllAsync(userId, x => x.Client, x => x.Store, x => x.InvoiceItems, x => x.Payments, x => x.PaymentLinks, x => x.PayInvoices);
            var filtered = invoices.Where(i => i.ClientId == clientId);

            return new GeneralResponse<IEnumerable<InvoiceReadDTO>>
            {
                Success = filtered.Any(),
                Message = filtered.Any() ? "Invoices retrieved." : "No invoices found for this client.",
                Data = _mapper.Map<IEnumerable<InvoiceReadDTO>>(filtered)
            };
        }

        public async Task<GeneralResponse<IEnumerable<InvoiceReadDTO>>> GetByStoreAsync(string storeId, string userId)
        {
            var invoices = await _invoiceRepo.GetAllAsync(userId, x => x.Client, x => x.Store, x => x.InvoiceItems, x => x.Payments, x => x.PaymentLinks, x => x.PayInvoices);
            var filtered = invoices.Where(i => i.StoreId == storeId);

            return new GeneralResponse<IEnumerable<InvoiceReadDTO>>
            {
                Success = filtered.Any(),
                Message = filtered.Any() ? "Invoices retrieved." : "No invoices found for this store.",
                Data = _mapper.Map<IEnumerable<InvoiceReadDTO>>(filtered)
            };
        }

        public async Task<GeneralResponse<IEnumerable<InvoiceReadDTO>>> GetByStatusAsync(InvoiceStatus status, string userId)
        {
            var invoices = await _invoiceRepo.GetAllAsync(userId, x => x.Client, x => x.Store, x => x.InvoiceItems, x => x.Payments, x => x.PaymentLinks, x => x.PayInvoices);
            var filtered = invoices.Where(i => i.InvoiceStatus == status);

            return new GeneralResponse<IEnumerable<InvoiceReadDTO>>
            {
                Success = filtered.Any(),
                Message = filtered.Any() ? "Invoices retrieved." : "No invoices found for this status.",
                Data = _mapper.Map<IEnumerable<InvoiceReadDTO>>(filtered)
            };
        }

        public async Task<GeneralResponse<IEnumerable<InvoiceReadDTO>>> GetByTypeAsync(InvoiceType type, string userId)
        {
            var invoices = await _invoiceRepo.GetAllAsync(userId, x => x.Client, x => x.Store, x => x.InvoiceItems, x => x.Payments, x => x.PaymentLinks, x => x.PayInvoices);
            var filtered = invoices.Where(i => i.InvoiceType == type);

            return new GeneralResponse<IEnumerable<InvoiceReadDTO>>
            {
                Success = filtered.Any(),
                Message = filtered.Any() ? "Invoices retrieved." : "No invoices found for this type.",
                Data = _mapper.Map<IEnumerable<InvoiceReadDTO>>(filtered)
            };
        }

        public async Task<GeneralResponse<decimal>> GetTotalValueAsync(string userId)
        {
            var invoices = await _invoiceRepo.GetAllAsync(userId);
            var total = invoices.Sum(i => i.Value);

            return new GeneralResponse<decimal>
            {
                Success = true,
                Message = "Total value calculated successfully.",
                Data = total
            };
        }

        public async Task<GeneralResponse<decimal>> GetTotalFinalValueAsync(string userId)
        {
            var invoices = await _invoiceRepo.GetAllAsync(userId);
            var total = invoices.Sum(i => i.FinalValue);

            return new GeneralResponse<decimal>
            {
                Success = true,
                Message = "Total final value calculated successfully.",
                Data = total
            };
        }

        public async Task<GeneralResponse<bool>> AddPaymentAsync(string invoiceId, PaymentCreateDTO paymentDto, string userId)
        {
            var invoice = await _invoiceRepo.GetByIdAsync(invoiceId, userId);
            if (invoice == null)
            {
                return new GeneralResponse<bool>
                {
                    Success = false,
                    Message = $"Invoice with Id '{invoiceId}' not found.",
                    Data = false
                };
            }

            var payment = _mapper.Map<Payment>(paymentDto);
            payment.InvoiceId = invoiceId;
            payment.UserId = userId;

            await _paymentRepo.AddAsync(payment);

            return new GeneralResponse<bool>
            {
                Success = true,
                Message = "Payment added successfully.",
                Data = true
            };
        }

        public async Task<GeneralResponse<bool>> GeneratePaymentLinkAsync(
            PaymentLinkCreateDTO dto, string userId)
        {
            var invoice = await _invoiceRepo.GetByIdAsync(dto.InvoiceId, userId);
            if (invoice == null)
            {
                return new GeneralResponse<bool>
                {
                    Success = false,
                    Message = $"Invoice with Id '{dto.InvoiceId}' not found.",
                    Data = false
                };
            }

            var paymentLink = _mapper.Map<PaymentLink>(dto);
            paymentLink.CreatedAt = DateTime.UtcNow;

            await _paymentLinkRepo.AddAsync(paymentLink);

            invoice.PaymentLinks.Add(paymentLink);
            await _invoiceRepo.UpdateAsync(invoice);

            return new GeneralResponse<bool>
            {
                Success = true,
                Message = "Payment link generated successfully.",
                Data = true
            };
        }

        public async Task<GeneralResponse<bool>> MarkAsPaidAsync(string invoiceId, string userId)
        {
            var invoice = await _invoiceRepo.GetByIdAsync(invoiceId, userId);
            if (invoice == null)
            {
                return new GeneralResponse<bool>
                {
                    Success = false,
                    Message = $"Invoice with Id '{invoiceId}' not found.",
                    Data = false
                };
            }

            invoice.InvoiceStatus = InvoiceStatus.Paid;
            await _invoiceRepo.UpdateAsync(invoice);

            return new GeneralResponse<bool>
            {
                Success = true,
                Message = "Invoice marked as paid.",
                Data = true
            };
        }

        public async Task<GeneralResponse<bool>> CancelAsync(string invoiceId, string userId)
        {
            var invoice = await _invoiceRepo.GetByIdAsync(invoiceId, userId);
            if (invoice == null)
            {
                return new GeneralResponse<bool>
                {
                    Success = false,
                    Message = $"Invoice with Id '{invoiceId}' not found.",
                    Data = false
                };
            }

            invoice.InvoiceStatus = InvoiceStatus.Cancelled;
            await _invoiceRepo.UpdateAsync(invoice);

            return new GeneralResponse<bool>
            {
                Success = true,
                Message = "Invoice cancelled successfully.",
                Data = true
            };
        }

        private void RecalculateInvoiceTotals(Invoice invoice)
        {
            invoice.Value = invoice.InvoiceItems?.Sum(i => i.UnitPrice * i.Quantity) ?? 0;
            invoice.FinalValue = invoice.DiscountType.HasValue && invoice.DiscountValue.HasValue
                ? invoice.DiscountType switch
                {
                    DiscountType.Amount => invoice.Value - invoice.DiscountValue.Value,
                    DiscountType.Percentage => invoice.Value * (1 - invoice.DiscountValue.Value / 100),
                    _ => invoice.Value
                }
                : invoice.Value;
        }
    }
}
