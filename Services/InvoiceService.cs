using AutoMapper;
using invoice.Core.DTO;
using invoice.Core.DTO.Invoice;
using invoice.Core.DTO.PayInvoice;
using invoice.Core.DTO.Payment;
using invoice.Core.DTO.PaymentLink;
using invoice.Core.Entites;
using invoice.Core.Enums;
using invoice.Core.Interfaces.Services;
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
        private readonly IRepository<Product> _productRepo;
        private readonly IRepository<PayInvoice> _payInvoiceRepo;
        private readonly IRepository<ApplicationUser> _applicationUserRepo;
        private readonly IMapper _mapper;

        public InvoiceService(
            IRepository<InvoiceItem> invoiceItemRepo,
            IRepository<Invoice> invoiceRepo,
            IRepository<Client> clientRepo,
            IRepository<Store> storeRepo,
            IRepository<Payment> paymentRepo,
            IRepository<PaymentLink> paymentLinkRepo,
            IRepository<Product> productRepo,
            IRepository<PayInvoice> payInvoiceRepo,
            IRepository<ApplicationUser> applicationUserRepo,
            IMapper mapper
        )
        {
            _invoiceItemRepo = invoiceItemRepo;
            _invoiceRepo = invoiceRepo;
            _clientRepo = clientRepo;
            _storeRepo = storeRepo;
            _paymentRepo = paymentRepo;
            _paymentLinkRepo = paymentLinkRepo;
            _productRepo = productRepo;
            _payInvoiceRepo = payInvoiceRepo;
            _applicationUserRepo = applicationUserRepo;
            _mapper = mapper;
        }

        public async Task<GeneralResponse<IEnumerable<GetAllInvoiceDTO>>> GetAllAsync(string userId)
        {
            if (string.IsNullOrWhiteSpace(userId))
                return new GeneralResponse<IEnumerable<GetAllInvoiceDTO>>
                {
                    Success = false,
                    Message = "UserId is required.",
                    Data = Enumerable.Empty<GetAllInvoiceDTO>(),
                };

            var invoices = await _invoiceRepo.GetAllAsync(userId, x => x.Client);

            if (!invoices.Any())
                return new GeneralResponse<IEnumerable<GetAllInvoiceDTO>>
                {
                    Success = false,
                    Message = "No invoices found.",
                    Data = Enumerable.Empty<GetAllInvoiceDTO>(),
                };

            return new GeneralResponse<IEnumerable<GetAllInvoiceDTO>>
            {
                Success = true,
                Message = "Invoices retrieved successfully.",
                Data = _mapper.Map<IEnumerable<GetAllInvoiceDTO>>(invoices),
            };
        }

        public async Task<GeneralResponse<InvoiceReadDTO>> GetByIdAsync(string id, string userId)
        {
            if (string.IsNullOrWhiteSpace(id) || string.IsNullOrWhiteSpace(userId))
                return new GeneralResponse<InvoiceReadDTO>
                {
                    Success = false,
                    Message = "Id and UserId are required.",
                };

            var invoice = await _invoiceRepo.GetByIdAsync(
                id,
                userId,
                q =>
                    q.Include(x => x.Client)
                        .Include(x => x.InvoiceItems)
                        .ThenInclude(i => i.Product)
                        .ThenInclude(p => p.Category)
                        .Include(x => x.PayInvoice)
                        .ThenInclude(p => p.PaymentMethod)
                        .Include(x => x.User)
                        .ThenInclude(u => u.Tax)
                        .Include(x => x.Payments)
                        .Include(x => x.Language)
            );

            if (invoice == null)
                return new GeneralResponse<InvoiceReadDTO>
                {
                    Success = false,
                    Message = $"Invoice with Id '{id}' not found.",
                };
            return new GeneralResponse<InvoiceReadDTO>
            {
                Success = true,
                Message = "Invoice retrieved successfully.",
                Data = _mapper.Map<InvoiceReadDTO>(invoice),
            };
        }

        public async Task<GeneralResponse<InvoiceReadDTO>> GetByCodeAsync(
            string code,
            string userId
        )
        {
            if (string.IsNullOrWhiteSpace(code) || string.IsNullOrWhiteSpace(userId))
                return new GeneralResponse<InvoiceReadDTO>
                {
                    Success = false,
                    Message = "Code and UserId are required.",
                };

            var invoices = await _invoiceRepo.GetAllAsync(
                userId,
                x => x.Client,
                x => x.Store,
                x => x.InvoiceItems,
                x => x.Payments,
                x => x.PayInvoice,
                x => x.Language,
                x => x.User
            );
            var invoice = invoices.FirstOrDefault(i =>
                i.Code.Equals(code, StringComparison.OrdinalIgnoreCase)
            );

            if (invoice == null)
                return new GeneralResponse<InvoiceReadDTO>
                {
                    Success = false,
                    Message = $"Invoice with code '{code}' not found.",
                };

            return new GeneralResponse<InvoiceReadDTO>
            {
                Success = true,
                Message = "Invoice retrieved successfully.",
                Data = _mapper.Map<InvoiceReadDTO>(invoice),
            };
        }

        public async Task<GeneralResponse<IEnumerable<InvoiceReadDTO>>> SearchAsync(
            string keyword,
            string userId
        )
        {
            if (string.IsNullOrWhiteSpace(userId))
                return new GeneralResponse<IEnumerable<InvoiceReadDTO>>
                {
                    Success = false,
                    Message = "UserId is required.",
                    Data = Enumerable.Empty<InvoiceReadDTO>(),
                };

            var invoices = await _invoiceRepo.GetAllAsync(
                userId,
                x => x.Client,
                x => x.Store,
                x => x.InvoiceItems,
                x => x.Payments,
                x => x.PayInvoice,
                x => x.Language,
                x => x.User
            );

            var filtered = invoices.Where(i =>
                (
                    !string.IsNullOrWhiteSpace(i.Code)
                    && i.Code.Contains(keyword ?? "", StringComparison.OrdinalIgnoreCase)
                )
                || (
                    i.Client != null
                    && i.Client.Name.Contains(keyword ?? "", StringComparison.OrdinalIgnoreCase)
                )
                || (
                    i.Store != null
                    && i.Store.Name.Contains(keyword ?? "", StringComparison.OrdinalIgnoreCase)
                )
            );

            return new GeneralResponse<IEnumerable<InvoiceReadDTO>>
            {
                Success = filtered.Any(),
                Message = filtered.Any()
                    ? "Matching invoices found."
                    : "No matching invoices found.",
                Data = _mapper.Map<IEnumerable<InvoiceReadDTO>>(filtered),
            };
        }

        public async Task<GeneralResponse<InvoiceReadDTO>> CreateAsync(
            InvoiceCreateDTO dto,
            string userId
        )
        {
            if (dto == null || string.IsNullOrWhiteSpace(userId))
                return new GeneralResponse<InvoiceReadDTO>
                {
                    Success = false,
                    Message = "Invoice data and UserId are required.",
                };

            var user = await _applicationUserRepo.GetByIdAsync(userId);

            var invoice = _mapper.Map<Invoice>(dto);
            invoice.UserId = userId;
            invoice.Code = $"INV-{DateTime.UtcNow.Ticks}";
            invoice.InvoiceStatus = InvoiceStatus.Active;

            if (string.IsNullOrWhiteSpace(dto.ClientId))
            {
                invoice.ClientId = null;
            }
            invoice.Value = 0;

            invoice.InvoiceItems = new List<InvoiceItem>();

            if (dto.InvoiceItems != null)
            {
                foreach (var item in dto.InvoiceItems)
                {
                    var product = await _productRepo.GetByIdAsync(item.ProductId, userId);
                    if (product == null)
                        return new GeneralResponse<InvoiceReadDTO>
                        {
                            Success = false,
                            Message = $"Product {item.ProductId} not found",
                        };

                    if (product.Quantity != null)
                    {
                        if (product.Quantity < item.Quantity)
                            return new GeneralResponse<InvoiceReadDTO>
                            {
                                Success = false,
                                Message = $"Product Quantity not Enough for {product.Name}",
                            };

                        product.Quantity -= item.Quantity;
                        await _productRepo.UpdateAsync(product);
                    }

                    invoice.InvoiceItems.Add(
                        new InvoiceItem
                        {
                            ProductId = product.Id,
                            Quantity = item.Quantity,
                            UnitPrice = product.Price,
                        }
                    );

                    invoice.Value += product.Price * item.Quantity;
                }
            }
            invoice.FinalValue = invoice.Value;

            if (dto.DiscountType == DiscountType.Amount)
            {
                invoice.FinalValue -= dto.DiscountValue ?? 0;
                invoice.DiscountType = DiscountType.Amount;
                invoice.DiscountValue = dto.DiscountValue;
            }
            else if (dto.DiscountType == DiscountType.Percentage)
            {
                invoice.FinalValue -= (invoice.Value * (dto.DiscountValue ?? 0) / 100);
                invoice.DiscountType = DiscountType.Percentage;
                invoice.DiscountValue = dto.DiscountValue;
            }
            if (invoice.FinalValue < 0)
                invoice.FinalValue = 0;

            if (dto.Tax && user.Tax?.Value > 0)
            {
                var taxRate = user.Tax.Value / 100m;
                invoice.FinalValue += invoice.FinalValue * taxRate;
            }

            await _invoiceRepo.AddAsync(invoice);

            return new GeneralResponse<InvoiceReadDTO>
            {
                Success = true,
                Message = "Invoice created successfully.",
                Data = _mapper.Map<InvoiceReadDTO>(invoice),
            };
        }

        public async Task<GeneralResponse<IEnumerable<InvoiceReadDTO>>> CreateRangeAsync(
            IEnumerable<InvoiceCreateDTO> dtos,
            string userId
        )
        {
            if (dtos == null || !dtos.Any() || string.IsNullOrWhiteSpace(userId))
            {
                return new GeneralResponse<IEnumerable<InvoiceReadDTO>>
                {
                    Success = false,
                    Message = "Invoice data and UserId are required.",
                    Data = Enumerable.Empty<InvoiceReadDTO>(),
                };
            }

            var user = await _applicationUserRepo.GetByIdAsync(userId);

            var createdInvoices = new List<Invoice>();
            var skippedInvoices = new List<string>();

            foreach (var dto in dtos)
            {
                if (!string.IsNullOrWhiteSpace(dto.ClientId))
                {
                    var client = await _clientRepo.GetByIdAsync(dto.ClientId, userId);
                    if (client == null)
                    {
                        skippedInvoices.Add($"Invoice skipped: Client '{dto.ClientId}' not found.");
                        continue;
                    }
                }

                if (!string.IsNullOrWhiteSpace(dto.StoreId))
                {
                    var store = await _storeRepo.GetByIdAsync(dto.StoreId, userId);
                    if (store == null)
                    {
                        skippedInvoices.Add($"Invoice skipped: Store '{dto.StoreId}' not found.");
                        continue;
                    }
                }

                var invoice = _mapper.Map<Invoice>(dto);
                invoice.UserId = userId;
                invoice.Code = $"INV-{DateTime.UtcNow.Ticks}";
                invoice.InvoiceStatus = InvoiceStatus.Active;
                invoice.Value = 0m;
                invoice.FinalValue = 0m;
                invoice.InvoiceItems = new List<InvoiceItem>();

                invoice.CreatedAt = DateTime.UtcNow;
                invoice.UpdatedAt = DateTime.UtcNow;

                var invoiceHasError = false;

                if (dto.InvoiceItems != null && dto.InvoiceItems.Any())
                {
                    foreach (var itemDto in dto.InvoiceItems)
                    {
                        if (string.IsNullOrWhiteSpace(itemDto.ProductId))
                        {
                            skippedInvoices.Add(
                                $"Invoice '{invoice.Code}' skipped: InvoiceItem missing ProductId."
                            );
                            invoiceHasError = true;
                            break;
                        }

                        var product = await _productRepo.GetByIdAsync(itemDto.ProductId, userId);
                        if (product == null)
                        {
                            skippedInvoices.Add(
                                $"Invoice '{invoice.Code}' skipped: Product '{itemDto.ProductId}' not found."
                            );
                            invoiceHasError = true;
                            break;
                        }

                        if (product.Quantity != null)
                        {
                            if (product.Quantity < itemDto.Quantity)
                            {
                                skippedInvoices.Add(
                                    $"Invoice '{invoice.Code}' skipped: Not enough quantity for product '{product.Name}'."
                                );
                                invoiceHasError = true;
                                break;
                            }

                            product.Quantity -= itemDto.Quantity;
                            var prodUpdate = await _productRepo.UpdateAsync(product);
                            if (!prodUpdate.Success)
                            {
                                skippedInvoices.Add(
                                    $"Invoice '{invoice.Code}' skipped: Failed updating product '{product.Id}' quantity."
                                );
                                invoiceHasError = true;
                                break;
                            }
                        }

                        var invoiceItem = new InvoiceItem
                        {
                            ProductId = product.Id,
                            Quantity = itemDto.Quantity,
                            UnitPrice = product.Price,
                        };

                        invoice.InvoiceItems.Add(invoiceItem);

                        invoice.Value += product.Price * itemDto.Quantity;
                    }
                }

                if (invoiceHasError)
                {
                    continue;
                }

                invoice.FinalValue = invoice.Value;

                if (dto.DiscountType == DiscountType.Amount)
                {
                    invoice.FinalValue -= dto.DiscountValue ?? 0m;
                    invoice.DiscountType = DiscountType.Amount;
                    invoice.DiscountValue = dto.DiscountValue;
                }
                else if (dto.DiscountType == DiscountType.Percentage)
                {
                    invoice.FinalValue -= invoice.Value * (dto.DiscountValue ?? 0m) / 100m;
                    invoice.DiscountType = DiscountType.Percentage;
                    invoice.DiscountValue = dto.DiscountValue;
                }

                if (invoice.FinalValue < 0m)
                    invoice.FinalValue = 0m;

                if (dto.Tax && user?.Tax?.Value > 0)
                {
                    var taxRate = user.Tax.Value / 100m;
                    invoice.FinalValue += invoice.FinalValue * taxRate;
                }

                RecalculateInvoiceTotals(invoice);

                var addResult = await _invoiceRepo.AddAsync(invoice);
                if (!addResult.Success)
                {
                    skippedInvoices.Add(
                        $"Invoice '{invoice.Code}' skipped: failed to save ({addResult.Message})."
                    );
                    continue;
                }

                createdInvoices.Add(invoice);
            }

            var success = createdInvoices.Any();
            var message = success
                ? $"Created {createdInvoices.Count} invoices successfully."
                : "No invoices were created.";

            if (skippedInvoices.Any())
                message +=
                    $" Skipped {skippedInvoices.Count} invoices. Details: {string.Join(" | ", skippedInvoices)}";

            return new GeneralResponse<IEnumerable<InvoiceReadDTO>>
            {
                Success = success,
                Message = message,
                Data = _mapper.Map<IEnumerable<InvoiceReadDTO>>(createdInvoices),
            };
        }

        public async Task<GeneralResponse<InvoiceReadDTO>> UpdateAsync(
            string id,
            InvoiceUpdateDTO dto,
            string userId
        )
        {
            var user = await _applicationUserRepo.GetByIdAsync(userId);

            var invoice = await _invoiceRepo.GetByIdAsync(
                id,
                userId,
                q =>
                    q.Include(x => x.Client)
                        .Include(x => x.InvoiceItems)
                        .ThenInclude(i => i.Product)
                        .ThenInclude(p => p.Category)
                        //.Include(x => x.Payments)
                        .Include(x => x.User)
                        .ThenInclude(u => u.Tax)
                        .Include(x => x.Language)
                        .Include(x => x.PayInvoice)
                        .ThenInclude(p => p.PaymentMethod)
            );

            if (invoice == null)
                return new GeneralResponse<InvoiceReadDTO>
                {
                    Success = false,
                    Message = $"Invoice with Id '{id}' not found.",
                };

            _mapper.Map(dto, invoice);
            invoice.Value = 0;
            if (string.IsNullOrWhiteSpace(dto.ClientId))
            {
                invoice.ClientId = null;
            }
            foreach (var oldItem in invoice.InvoiceItems.ToList())
            {
                var product = await _productRepo.GetByIdAsync(oldItem.ProductId, userId);
                if (product?.Quantity != null)
                {
                    product.Quantity += oldItem.Quantity;
                    await _productRepo.UpdateAsync(product);
                }

                await _invoiceItemRepo.DeleteAsync(oldItem.Id);
            }

            invoice.InvoiceItems.Clear();

            if (dto.InvoiceItems != null && dto.InvoiceItems.Any())
            {
                foreach (var itemDto in dto.InvoiceItems)
                {
                    var product = await _productRepo.GetByIdAsync(itemDto.ProductId, userId);
                    if (product == null)
                        return new GeneralResponse<InvoiceReadDTO>
                        {
                            Success = false,
                            Message = $"Product {itemDto.ProductId} not found",
                        };

                    if (product.Quantity != null && product.Quantity < itemDto.Quantity)
                        return new GeneralResponse<InvoiceReadDTO>
                        {
                            Success = false,
                            Message = $"Product Quantity not Enough for {product.Name}",
                        };

                    if (product.Quantity != null)
                    {
                        product.Quantity -= itemDto.Quantity;
                        await _productRepo.UpdateAsync(product);
                    }

                    var newItem = new InvoiceItem
                    {
                        ProductId = product.Id,
                        Quantity = itemDto.Quantity,
                        UnitPrice = product.Price,
                        InvoiceId = invoice.Id,
                    };

                    await _invoiceItemRepo.AddAsync(newItem);

                    invoice.Value += product.Price * itemDto.Quantity;
                }
            }

            invoice.Currency = dto.Currency;
            invoice.FinalValue = invoice.Value;
            if (dto.DiscountType == DiscountType.Amount)
            {
                invoice.FinalValue -= dto.DiscountValue ?? 0;
                invoice.DiscountType = DiscountType.Amount;
                invoice.DiscountValue = dto.DiscountValue;
            }
            else if (dto.DiscountType == DiscountType.Percentage)
            {
                invoice.FinalValue -= (invoice.Value * (dto.DiscountValue ?? 0) / 100);
                invoice.DiscountType = DiscountType.Percentage;
                invoice.DiscountValue = dto.DiscountValue;
            }

            if (invoice.FinalValue < 0)
                invoice.FinalValue = 0;

            if (dto.Tax && user.Tax?.Value > 0)
            {
                var taxRate = user.Tax.Value / 100m;
                invoice.FinalValue += invoice.FinalValue * taxRate;
            }

            await _invoiceRepo.UpdateAsync(invoice);

            return new GeneralResponse<InvoiceReadDTO>
            {
                Success = true,
                Message = "Invoice updated successfully",
                Data = _mapper.Map<InvoiceReadDTO>(invoice),
            };
        }

        public async Task<GeneralResponse<InvoiceReadDTO>> UpdateStatusAsync(
            string id,
            InvoiceStatus status,
            string userId = null
        )
        {
            try
            {
                if (string.IsNullOrEmpty(id))
                {
                    return new GeneralResponse<InvoiceReadDTO>
                    {
                        Success = false,
                        Message = "Invoice ID is required",
                    };
                }

                var invoice = await _invoiceRepo.GetByIdAsync(id);
                if (invoice == null)
                {
                    return new GeneralResponse<InvoiceReadDTO>
                    {
                        Success = false,
                        Message = $"Invoice {id} not found",
                    };
                }

                invoice.InvoiceStatus = status;
                invoice.UpdatedAt = DateTime.UtcNow;

                if (status == InvoiceStatus.Paid)
                {
                    invoice.PayInvoice.PaidAt = DateTime.UtcNow;
                }
                else if (status == InvoiceStatus.Refunded)
                {
                    invoice.UpdatedAt = DateTime.UtcNow;
                }

                var result = await _invoiceRepo.UpdateAsync(invoice);
                if (!result.Success)
                {
                    return new GeneralResponse<InvoiceReadDTO>
                    {
                        Success = false,
                        Message = result.Message,
                    };
                }

                var invoiceDto = _mapper.Map<InvoiceReadDTO>(invoice);

                return new GeneralResponse<InvoiceReadDTO>
                {
                    Success = true,
                    Message = $"Invoice status updated to {status}",
                    Data = invoiceDto,
                };
            }
            catch (Exception ex)
            {
                return new GeneralResponse<InvoiceReadDTO>
                {
                    Success = false,
                    Message = $"Error updating invoice status: {ex.Message}",
                };
            }
        }

        public async Task<GeneralResponse<IEnumerable<InvoiceReadDTO>>> UpdateRangeAsync(
            IEnumerable<InvoiceUpdateDTO> dtos,
            string userId
        )
        {
            var updatedInvoices = new List<Invoice>();

            //foreach (var dto in dtos)
            //{
            //    var invoice = await _invoiceRepo.GetByIdAsync(dto.Id, userId, q => q
            //        .Include(x => x.Client)
            //        .Include(x => x.Store)
            //        .Include(x => x.InvoiceItems)
            //        .Include(x => x.Payments)
            //        .Include(x => x.PaymentLinks));

            //    if (invoice == null)
            //        continue;

            //    if (!string.IsNullOrEmpty(dto.ClientId) && dto.ClientId != invoice.ClientId)
            //    {
            //        var client = await _clientRepo.GetByIdAsync(dto.ClientId, userId);
            //        if (client == null)
            //            continue;
            //        invoice.ClientId = dto.ClientId;
            //    }

            //    if (!string.IsNullOrEmpty(dto.StoreId) && dto.StoreId != invoice.StoreId)
            //    {
            //        var store = await _storeRepo.GetByIdAsync(dto.StoreId, userId);
            //        if (store == null)
            //            continue;
            //        invoice.StoreId = dto.StoreId;
            //    }

            //    _mapper.Map(dto, invoice);

            //    if (dto.InvoiceItems != null)
            //    {
            //        var itemsToDelete = invoice.InvoiceItems
            //            .Where(i => !dto.InvoiceItems.Any(d => d.Id == i.Id))
            //            .ToList();

            //        foreach (var item in itemsToDelete)
            //        {
            //            invoice.InvoiceItems.Remove(item);
            //            await _invoiceItemRepo.DeleteAsync(item.Id);
            //        }

            //        foreach (var itemDto in dto.InvoiceItems)
            //        {
            //            var existingItem = invoice.InvoiceItems.FirstOrDefault(i => i.Id == itemDto.Id);
            //            if (existingItem != null)
            //            {
            //                _mapper.Map(itemDto, existingItem);
            //                await _invoiceItemRepo.UpdateAsync(existingItem);
            //            }
            //            else
            //            {
            //                var newItem = _mapper.Map<InvoiceItem>(itemDto);
            //                newItem.InvoiceId = invoice.Id;
            //                await _invoiceItemRepo.AddAsync(newItem);
            //                invoice.InvoiceItems.Add(newItem);
            //            }
            //        }

            //        invoice.Value = invoice.InvoiceItems.Sum(i => i.UnitPrice * i.Quantity);
            //        if (invoice.DiscountType.HasValue && invoice.DiscountValue.HasValue)
            //        {
            //            invoice.FinalValue = invoice.DiscountType switch
            //            {
            //                DiscountType.Amount => invoice.Value - invoice.DiscountValue.Value,
            //                DiscountType.Percentage => invoice.Value * (1 - invoice.DiscountValue.Value / 100),
            //                _ => invoice.Value
            //            };
            //        }
            //        else
            //        {
            //            invoice.FinalValue = invoice.Value;
            //        }
            //    }

            //    updatedInvoices.Add(invoice);
            //}

            //if (!updatedInvoices.Any())
            //    return new GeneralResponse<IEnumerable<InvoiceReadDTO>>
            //    {
            //        Success = false,
            //        Message = "No invoices were updated.",
            //        Data = Enumerable.Empty<InvoiceReadDTO>()
            //    };

            //await _invoiceRepo.UpdateRangeAsync(updatedInvoices);

            return new GeneralResponse<IEnumerable<InvoiceReadDTO>>
            {
                Success = true,
                Message = "Invoices updated successfully.",
                Data = _mapper.Map<IEnumerable<InvoiceReadDTO>>(updatedInvoices),
            };
        }

        public async Task<GeneralResponse<bool>> DeleteAsync(string id, string userId)
        {
            var invoice = await _invoiceRepo.GetByIdAsync(id, userId);

            if (invoice == null)
            {
                return new GeneralResponse<bool>
                {
                    Success = false,
                    Message = $"Invoice with Id '{id}' not found.",
                    Data = false,
                };
            }

            await _invoiceRepo.DeleteAsync(invoice.Id);

            return new GeneralResponse<bool>
            {
                Success = true,
                Message = "Invoice deleted successfully.",
                Data = true,
            };
        }

        public async Task<GeneralResponse<bool>> RefundAsync(string id, string userId)
        {
            var invoice = await _invoiceRepo.GetByIdAsync(
                id,
                userId,
                q => q.Include(x => x.InvoiceItems)
            );

            if (invoice == null)
            {
                return new GeneralResponse<bool>
                {
                    Success = false,
                    Message = $"Invoice with Id '{id}' not found.",
                    Data = false,
                };
            }
            foreach (var item in invoice.InvoiceItems)
            {
                var product = await _productRepo.GetByIdAsync(item.ProductId, userId);
                if (product != null && product.Quantity != null)
                {
                    product.Quantity += item.Quantity;
                    await _productRepo.UpdateAsync(product);
                }
            }

            invoice.InvoiceStatus = InvoiceStatus.Refunded;
            await _invoiceRepo.UpdateAsync(invoice);

            return new GeneralResponse<bool>
            {
                Success = true,
                Message = "Invoice refunded and product quantities updated successfully.",
                Data = true,
            };
        }

        public async Task<GeneralResponse<bool>> PayAsync(
            string id,
            PayInvoiceCreateDTO dto,
            string userId
        )
        {
            var invoice = await _invoiceRepo.GetByIdAsync(id, userId);
            if (invoice == null)
            {
                return new GeneralResponse<bool>
                {
                    Success = false,
                    Message = $"Invoice with Id '{id}' not found.",
                    Data = false,
                };
            }

            invoice.InvoiceStatus = InvoiceStatus.Paid;

            var payInvoice = new PayInvoice
            {
                PaidAt = dto.PayAt ?? DateTime.UtcNow,
                Amount = dto.Amount ?? invoice.FinalValue,
                Currency = string.IsNullOrEmpty(dto.Currency) ? "USD" : dto.Currency,
                Status = PaymentStatus.Completed,
                PaymentMethodId = dto.PaymentMethodId ?? "cash",
                InvoiceId = id,
                PaymentGatewayType = dto.PaymentGatewayType ?? PaymentType.Cash,
                PaymentGatewayResponse = dto.PaymentGatewayResponse ?? "Manual payment recorded.",
                PaymentSessionId = dto.PaymentSessionId,
                RefundAmount = null,
                RefundedAt = null,
            };

            await _invoiceRepo.UpdateAsync(invoice);
            await _payInvoiceRepo.AddAsync(payInvoice);

            return new GeneralResponse<bool>
            {
                Success = true,
                Message = "Invoice paid successfully.",
                Data = true,
            };
        }

        public async Task<GeneralResponse<bool>> DeleteRangeAsync(
            IEnumerable<string> ids,
            string userId
        )
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
                    Data = false,
                };
            }

            await _invoiceRepo.DeleteRangeAsync(validIds);

            return new GeneralResponse<bool>
            {
                Success = true,
                Message = "Invoices deleted successfully.",
                Data = true,
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

        public async Task<GeneralResponse<IEnumerable<InvoiceReadDTO>>> GetByClientAsync(
            string clientId,
            string userId
        )
        {
            var invoices = await _invoiceRepo.GetAllAsync(
                userId,
                x => x.Client,
                x => x.Store,
                x => x.InvoiceItems,
                x => x.Payments,
                x => x.PayInvoice,
                x => x.Language,
                x => x.User
            );
            var filtered = invoices.Where(i => i.ClientId == clientId);

            return new GeneralResponse<IEnumerable<InvoiceReadDTO>>
            {
                Success = filtered.Any(),
                Message = filtered.Any()
                    ? "Invoices retrieved."
                    : "No invoices found for this client.",
                Data = _mapper.Map<IEnumerable<InvoiceReadDTO>>(filtered),
            };
        }

        public async Task<GeneralResponse<IEnumerable<InvoiceReadDTO>>> GetByStoreAsync(
            string storeId,
            string userId
        )
        {
            var invoices = await _invoiceRepo.GetAllAsync(
                userId,
                x => x.Client,
                x => x.Store,
                x => x.InvoiceItems,
                x => x.Payments,
                x => x.PayInvoice,
                x => x.Language,
                x => x.User
            );
            var filtered = invoices.Where(i => i.StoreId == storeId);

            return new GeneralResponse<IEnumerable<InvoiceReadDTO>>
            {
                Success = filtered.Any(),
                Message = filtered.Any()
                    ? "Invoices retrieved."
                    : "No invoices found for this store.",
                Data = _mapper.Map<IEnumerable<InvoiceReadDTO>>(filtered),
            };
        }

        public async Task<GeneralResponse<IEnumerable<InvoiceReadDTO>>> GetByStatusAsync(
            InvoiceStatus status,
            string userId
        )
        {
            var invoices = await _invoiceRepo.GetAllAsync(
                userId,
                x => x.Client,
                x => x.Store,
                x => x.InvoiceItems,
                x => x.Payments,
                x => x.PayInvoice
            );
            var filtered = invoices.Where(i => i.InvoiceStatus == status);

            return new GeneralResponse<IEnumerable<InvoiceReadDTO>>
            {
                Success = filtered.Any(),
                Message = filtered.Any()
                    ? "Invoices retrieved."
                    : "No invoices found for this status.",
                Data = _mapper.Map<IEnumerable<InvoiceReadDTO>>(filtered),
            };
        }

        public async Task<GeneralResponse<IEnumerable<InvoiceReadDTO>>> GetByTypeAsync(
            InvoiceType type,
            string userId
        )
        {
            var today = DateTime.Today;
            var tomorrow = today.AddDays(1);

            var invoices = await _invoiceRepo.GetAllAsync(
                userId,
                x => x.Client,
                x => x.Store,
                x => x.InvoiceItems,
                x => x.Payments,
                x => x.PayInvoice
            );
            var filtered = invoices.Where(i =>
                i.InvoiceType == type && (i.CreatedAt >= today && i.CreatedAt < tomorrow)
            );

            return new GeneralResponse<IEnumerable<InvoiceReadDTO>>
            {
                Success = filtered.Any(),
                Message = filtered.Any()
                    ? "Invoices retrieved."
                    : "No invoices found for this type.",
                Data = _mapper.Map<IEnumerable<InvoiceReadDTO>>(filtered),
            };
        }

        public async Task<GeneralResponse<InvoiceReadDTO>> RecalculateInvoiceTotalsAsync(
            string invoiceId,
            string userId
        )
        {
            if (string.IsNullOrWhiteSpace(invoiceId))
                return new GeneralResponse<InvoiceReadDTO>
                {
                    Success = false,
                    Message = "Invoice ID is required",
                };

            var invoice = await _invoiceRepo.GetByIdAsync(
                invoiceId,
                userId,
                q => q.Include(i => i.InvoiceItems).Include(i => i.Payments)
            );

            if (invoice == null)
                return new GeneralResponse<InvoiceReadDTO>
                {
                    Success = false,
                    Message = "Invoice not found",
                };

            RecalculateInvoiceTotals(invoice);

            invoice.UpdatedAt = DateTime.UtcNow;

            var result = await _invoiceRepo.UpdateAsync(invoice);
            if (!result.Success)
                return new GeneralResponse<InvoiceReadDTO>
                {
                    Success = false,
                    Message = "Failed to recalculate invoice totals",
                };

            return new GeneralResponse<InvoiceReadDTO>
            {
                Success = true,
                Message = "Invoice totals recalculated successfully",
                Data = _mapper.Map<InvoiceReadDTO>(result.Data),
            };
        }

        public async Task<GeneralResponse<decimal>> GetInvoiceRevenueAsync(
            string invoiceId,
            string userId
        )
        {
            if (string.IsNullOrWhiteSpace(invoiceId))
                return new GeneralResponse<decimal>
                {
                    Success = false,
                    Message = "Invoice ID is required",
                };

            var invoice = await _invoiceRepo.GetByIdAsync(
                invoiceId,
                userId,
                q => q.Include(i => i.Payments)
            );

            if (invoice == null)
                return new GeneralResponse<decimal>
                {
                    Success = false,
                    Message = "Invoice not found",
                };

            var Payments = invoice.Payments?.Sum(p => p.Cost) ?? 0m;

            return new GeneralResponse<decimal>
            {
                Success = true,
                Message = "Invoice revenue calculated successfully",
                Data = Payments,
            };
        }

        public async Task<
            GeneralResponse<IEnumerable<PaymentLinkReadDTO>>
        > GetInvoicePaymentLinkAsync(string invoiceId, string userId)
        {
            if (string.IsNullOrWhiteSpace(invoiceId))
                return new GeneralResponse<IEnumerable<PaymentLinkReadDTO>>
                {
                    Success = false,
                    Message = "Invoice ID is required",
                };

            var paymentLinks = await _paymentLinkRepo.QueryAsync(pl =>
                pl.InvoiceId == invoiceId
                && (string.IsNullOrEmpty(userId) || pl.CreatedBy == userId)
            );

            return new GeneralResponse<IEnumerable<PaymentLinkReadDTO>>
            {
                Success = true,
                Message = paymentLinks.Any()
                    ? "Payment links retrieved successfully"
                    : "No payment links found for this invoice",
                Data = _mapper.Map<IEnumerable<PaymentLinkReadDTO>>(paymentLinks),
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
                Data = total,
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
                Data = total,
            };
        }

        public async Task<GeneralResponse<bool>> AddPaymentAsync(
            string invoiceId,
            PaymentCreateDTO paymentDto,
            string userId
        )
        {
            var invoice = await _invoiceRepo.GetByIdAsync(invoiceId, userId);
            if (invoice == null)
            {
                return new GeneralResponse<bool>
                {
                    Success = false,
                    Message = $"Invoice with Id '{invoiceId}' not found.",
                    Data = false,
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
                Data = true,
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
                    Data = false,
                };
            }

            invoice.InvoiceStatus = InvoiceStatus.Paid;
            await _invoiceRepo.UpdateAsync(invoice);

            return new GeneralResponse<bool>
            {
                Success = true,
                Message = "Invoice marked as paid.",
                Data = true,
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
                    Data = false,
                };
            }

            invoice.InvoiceStatus = InvoiceStatus.Cancelled;
            await _invoiceRepo.UpdateAsync(invoice);

            return new GeneralResponse<bool>
            {
                Success = true,
                Message = "Invoice cancelled successfully.",
                Data = true,
            };
        }

        private void RecalculateInvoiceTotals(Invoice invoice)
        {
            if (invoice == null)
                return;

            var itemsTotal =
                invoice.InvoiceItems?.Where(item => item != null).Sum(i => i.UnitPrice * i.Quantity)
                ?? 0m;

            invoice.Value = itemsTotal;

            decimal finalValue = invoice.Value;
            if (invoice.DiscountType.HasValue && invoice.DiscountValue.HasValue)
            {
                finalValue = invoice.DiscountType.Value switch
                {
                    DiscountType.Amount => Math.Max(0, invoice.Value - invoice.DiscountValue.Value),
                    DiscountType.Percentage => invoice.Value
                        * Math.Max(0, (1 - invoice.DiscountValue.Value / 100m)),
                    _ => invoice.Value,
                };
            }

            if (invoice.Tax && invoice.User?.Tax != null && invoice.User.Tax.Value > 0)
            {
                decimal taxRate = invoice.User.Tax.Value / 100m;
                finalValue += finalValue * taxRate;
            }

            invoice.FinalValue = Math.Round(finalValue, 2);

            var Payments = invoice.Payments?.Where(p => p != null).Sum(p => p.Cost) ?? 0m;

            if (invoice.FinalValue == 0m)
            {
                invoice.InvoiceStatus = InvoiceStatus.Paid;
            }
            else if (Payments >= invoice.FinalValue)
            {
                invoice.InvoiceStatus = InvoiceStatus.Paid;
            }
            else if (
                invoice.InvoiceStatus != InvoiceStatus.Cancelled
                && invoice.InvoiceStatus != InvoiceStatus.Refunded
            )
            {
                invoice.InvoiceStatus = InvoiceStatus.Active;
            }

            invoice.UpdatedAt = DateTime.UtcNow;
        }
    }
}
