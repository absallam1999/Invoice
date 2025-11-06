using AutoMapper;
using invoice.Core.DTO;
using invoice.Core.DTO.Invoice;
using invoice.Core.DTO.InvoiceItem;
using invoice.Core.DTO.Order;
using invoice.Core.Entities;
using invoice.Core.Enums;
using invoice.Core.Interfaces.Services;
using invoice.Repo;
using invoice.Repo.Data;
using Microsoft.EntityFrameworkCore;

namespace invoice.Services
{
    public class OrderService : IOrderService
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly IRepository<Store> _storeRepo;
        private readonly IRepository<Order> _orderRepo;
        private readonly IRepository<Client> _clientRepo;
        private readonly IRepository<Product> _productRepo;
        private readonly IInvoiceService _invoiceService;
        private readonly IMapper _mapper;

        public OrderService(
            ApplicationDbContext dbContext,
            IRepository<Order> orderRepo,
            IRepository<Client> clientRepo,
            IRepository<Store> storeRepo,
            IRepository<Product> productRepo,
            IInvoiceService invoiceService,
            IMapper mapper)
        {
            _mapper = mapper;
            _orderRepo = orderRepo;
            _clientRepo = clientRepo;
            _storeRepo = storeRepo;
            _productRepo = productRepo;
            _invoiceService = invoiceService;
            _dbContext = dbContext;
        }


        //public async Task<GeneralResponse<IEnumerable<OrderReadDTO>>> GetAllAsync(string userId)
        //{
        //    if (string.IsNullOrWhiteSpace(userId))
        //        return new GeneralResponse<IEnumerable<OrderReadDTO>> { Success = false, Message = "User ID is required" };

        //    try
        //    {
        //        var orders = await _orderRepo.GetAllAsync(
        //            userId,
        //          //  o => o.Store,
        //          //  o => o.Client,
        //            o => o.Invoice
        //          //  o => o.OrderItems
        //        );

        //        return new GeneralResponse<IEnumerable<OrderReadDTO>>
        //        {
        //            Success = true,
        //            Message = orders.Any() ? "Orders retrieved successfully" : "No orders found",
        //            Data = _mapper.Map<IEnumerable<OrderReadDTO>>(orders)
        //        };
        //    }
        //    catch (Exception ex)
        //    {
        //        return new GeneralResponse<IEnumerable<OrderReadDTO>>
        //        {
        //            Success = false,
        //            Message = $"Error retrieving orders: {ex.Message}"
        //        };
        //    }
        //}

        //public async Task<GeneralResponse<OrderReadDTO>> GetByIdAsync(string id, string userId)
        //{
        //    if (string.IsNullOrWhiteSpace(id) || string.IsNullOrWhiteSpace(userId))
        //        return new GeneralResponse<OrderReadDTO> { Success = false, Message = "ID and User ID are required" };

        //    try
        //    {
        //        var order = await _orderRepo.GetByIdAsync(id, userId, q => q
        //            .Include(o => o.Store)
        //            .Include(o => o.Client)
        //            .Include(o => o.Invoice)
        //            .Include(o => o.OrderItems).ThenInclude(oi => oi.Product));

        //        if (order == null)
        //            return new GeneralResponse<OrderReadDTO> { Success = false, Message = "Order not found" };

        //        return new GeneralResponse<OrderReadDTO>
        //        {
        //            Success = true,
        //            Message = "Order retrieved successfully",
        //            Data = _mapper.Map<OrderReadDTO>(order)
        //        };
        //    }
        //    catch (Exception ex)
        //    {
        //        return new GeneralResponse<OrderReadDTO>
        //        {
        //            Success = false,
        //            Message = $"Error retrieving order: {ex.Message}"
        //        };
        //    }
        //}

        //public async Task<GeneralResponse<OrderReadDTO>> CreateAsync(OrderCreateDTO dto, string userId)
        //{
        //    if (dto == null || string.IsNullOrWhiteSpace(userId))
        //        return new GeneralResponse<OrderReadDTO>(false, "Order data and UserId are required.");

        //    if (dto.OrderItems == null || !dto.OrderItems.Any())
        //        return new GeneralResponse<OrderReadDTO>(false, "At least one order item is required.");

        //    var strategy = _dbContext.Database.CreateExecutionStrategy();

        //    return await strategy.ExecuteAsync(async () =>
        //    {
        //        await using var transaction = await _dbContext.Database.BeginTransactionAsync();
        //        try
        //        {
        //            var order = _mapper.Map<Order>(dto);
        //            //order.ClientId = dto.ClientId;
        //         //   order.OrderStatus = OrderStatus.Pending;
        //         //   order.Code = $"ORD-{DateTime.UtcNow.Ticks}";
        //            order.CreatedAt = DateTime.UtcNow;
        //           // order.OrderItems = new List<OrderItem>();
        //            decimal totalAmount = 0;

        //            foreach (var itemDto in dto.OrderItems)
        //            {
        //                var product = await _productRepo.GetByIdAsync(itemDto.ProductId, userId);
        //                if (product == null)
        //                    return new GeneralResponse<OrderReadDTO>(false, $"Product {itemDto.ProductId} not found");

        //                var orderItem = new OrderItem
        //                {
        //                    ProductId = product.Id,
        //                    Quantity = itemDto.Quantity,
        //                    UnitPrice = product.Price,
        //                };

        //                order.OrderItems.Add(orderItem);
        //                totalAmount += orderItem.UnitPrice * orderItem.Quantity;
        //            }
        //            order.TotalAmount = totalAmount;

        //            var invoiceDto = new InvoiceCreateDTO
        //            {
        //                StoreId = dto.StoreId,
        //                ClientId = dto.ClientId,
        //                Currency = dto.Currency,
        //                LanguageId = dto.LanguageId,
        //                InvoiceType = InvoiceType.Detailed,
        //                TermsConditions = "Auto-generated invoice for order",
        //                Tax = dto.Tax,
        //                DiscountType = dto.DiscountType,
        //                DiscountValue = dto.DiscountValue,
        //                InvoiceItems = dto.OrderItems.Select(o => new InvoiceItemCreateDTO
        //                {
        //                    ProductId = o.ProductId,
        //                    Quantity = o.Quantity
        //                }).ToList()
        //            };

        //            var invoiceResponse = await _invoiceService.CreateAsync(invoiceDto, userId);
        //            if (!invoiceResponse.Success)
        //            {
        //                await transaction.RollbackAsync();
        //                return new GeneralResponse<OrderReadDTO>(false, $"Invoice failed: {invoiceResponse.Message}");
        //            }

        //            order.InvoiceId = invoiceResponse.Data.Id;
        //            await _orderRepo.AddAsync(order);

        //            await transaction.CommitAsync();

        //            return new GeneralResponse<OrderReadDTO>(
        //                true,
        //                "Order created successfully with new Invoice.",
        //                _mapper.Map<OrderReadDTO>(order)
        //            );
        //        }
        //        catch (Exception ex)
        //        {
        //            await transaction.RollbackAsync();

        //            var errorMessage = ex.InnerException?.Message ?? ex.Message;
        //            return new GeneralResponse<OrderReadDTO>(
        //                false,
        //                $"Failed to create Order and Invoice: {errorMessage}"
        //            );
        //        }
        //    });
        //}

        //public async Task<GeneralResponse<OrderReadDTO>> UpdateAsync(string id, OrderUpdateDTO dto, string userId)
        //{
        //    if (string.IsNullOrWhiteSpace(id))
        //        return new GeneralResponse<OrderReadDTO> { Success = false, Message = "Order ID is required" };

        //    if (dto == null)
        //        return new GeneralResponse<OrderReadDTO> { Success = false, Message = "Order data is required" };

        //    try
        //    {
        //        var existingOrder = await _orderRepo.GetByIdAsync(id, userId, q => q.Include(o => o.OrderItems));
        //        if (existingOrder == null)
        //            return new GeneralResponse<OrderReadDTO> { Success = false, Message = "Order not found" };

        //        if (!string.IsNullOrEmpty(dto.ClientId) && dto.ClientId != existingOrder.ClientId)
        //        {
        //            var client = await _clientRepo.GetByIdAsync(dto.ClientId, userId);
        //            if (client == null)
        //                return new GeneralResponse<OrderReadDTO> { Success = false, Message = "Client not found" };
        //        }

        //        if (!string.IsNullOrEmpty(dto.StoreId) && dto.StoreId != existingOrder.StoreId)
        //        {
        //            var store = await _storeRepo.GetByIdAsync(dto.StoreId, userId);
        //            if (store == null)
        //                return new GeneralResponse<OrderReadDTO> { Success = false, Message = "Store not found" };
        //        }

        //        _mapper.Map(dto, existingOrder);
        //        existingOrder.UpdatedAt = DateTime.UtcNow;

        //        if (existingOrder.OrderItems != null && existingOrder.OrderItems.Any())
        //        {
        //            existingOrder.TotalAmount = existingOrder.OrderItems.Sum(oi => oi.UnitPrice * oi.Quantity);
        //        }

        //        var response = await _orderRepo.UpdateAsync(existingOrder);
        //        if (!response.Success)
        //            return new GeneralResponse<OrderReadDTO> { Success = false, Message = "Failed to update order" };

        //        return new GeneralResponse<OrderReadDTO>
        //        {
        //            Success = true,
        //            Message = "Order updated successfully",
        //            Data = _mapper.Map<OrderReadDTO>(response.Data)
        //        };
        //    }
        //    catch (Exception ex)
        //    {
        //        return new GeneralResponse<OrderReadDTO>
        //        {
        //            Success = false,
        //            Message = $"Failed to update order: {ex.Message}"
        //        };
        //    }
        //}

        //public async Task<GeneralResponse<bool>> DeleteAsync(string id, string userId)
        //{
        //    if (string.IsNullOrWhiteSpace(id))
        //        return new GeneralResponse<bool> { Success = false, Message = "Order ID is required" };

        //    try
        //    {
        //        var result = await _orderRepo.DeleteAsync(id);
        //        return new GeneralResponse<bool>
        //        {
        //            Success = result.Success,
        //            Message = result.Success ? "Order deleted successfully" : result.Message,
        //            Data = result.Success
        //        };
        //    }
        //    catch (Exception ex)
        //    {
        //        return new GeneralResponse<bool>
        //        {
        //            Success = false,
        //            Message = $"Failed to delete order: {ex.Message}"
        //        };
        //    }
        //}

        //public async Task<GeneralResponse<IEnumerable<OrderReadDTO>>> CreateRangeAsync(IEnumerable<OrderCreateDTO> dtos, string userId)
        //{
        //    if (dtos == null || !dtos.Any())
        //        return new GeneralResponse<IEnumerable<OrderReadDTO>> { Success = false, Message = "Order data is required" };

        //    try
        //    {
        //        var orders = new List<Order>();

        //        foreach (var dto in dtos)
        //        {
        //            var order = _mapper.Map<Order>(dto);
        //            order.CreatedAt = DateTime.UtcNow;
        //            order.UpdatedAt = DateTime.UtcNow;
        //            orders.Add(order);
        //        }

        //        var response = await _orderRepo.AddRangeAsync(orders);
        //        if (!response.Success)
        //            return new GeneralResponse<IEnumerable<OrderReadDTO>> { Success = false, Message = "Failed to create orders" };

        //        return new GeneralResponse<IEnumerable<OrderReadDTO>>
        //        {
        //            Success = true,
        //            Message = "Orders created successfully",
        //            Data = _mapper.Map<IEnumerable<OrderReadDTO>>(response.Data)
        //        };
        //    }
        //    catch (Exception ex)
        //    {
        //        return new GeneralResponse<IEnumerable<OrderReadDTO>>
        //        {
        //            Success = false,
        //            Message = $"Failed to create orders: {ex.Message}"
        //        };
        //    }
        //}

        //public async Task<GeneralResponse<IEnumerable<OrderReadDTO>>> UpdateRangeAsync(IEnumerable<OrderUpdateDTO> dtos, string userId)
        //{
        //    if (dtos == null || !dtos.Any())
        //        return new GeneralResponse<IEnumerable<OrderReadDTO>> { Success = false, Message = "Order data is required" };

        //    try
        //    {
        //        var updatedOrders = new List<Order>();

        //        foreach (var dto in dtos)
        //        {
        //            if (string.IsNullOrWhiteSpace(dto.Id))
        //                continue;

        //            var existingOrder = await _orderRepo.GetByIdAsync(dto.Id, userId);
        //            if (existingOrder == null)
        //                continue;

        //            _mapper.Map(dto, existingOrder);
        //            existingOrder.UpdatedAt = DateTime.UtcNow;

        //            var result = await _orderRepo.UpdateAsync(existingOrder);
        //            if (result.Success)
        //                updatedOrders.Add(result.Data);
        //        }

        //        return new GeneralResponse<IEnumerable<OrderReadDTO>>
        //        {
        //            Success = true,
        //            Message = updatedOrders.Any() ? "Orders updated successfully" : "No orders were updated",
        //            Data = _mapper.Map<IEnumerable<OrderReadDTO>>(updatedOrders)
        //        };
        //    }
        //    catch (Exception ex)
        //    {
        //        return new GeneralResponse<IEnumerable<OrderReadDTO>>
        //        {
        //            Success = false,
        //            Message = $"Failed to update orders: {ex.Message}"
        //        };
        //    }
        //}

        //public async Task<GeneralResponse<bool>> DeleteRangeAsync(IEnumerable<string> ids, string userId)
        //{
        //    if (ids == null || !ids.Any())
        //        return new GeneralResponse<bool> { Success = false, Message = "Order IDs are required" };

        //    try
        //    {
        //        var results = new List<bool>();

        //        foreach (var id in ids)
        //        {
        //            if (string.IsNullOrWhiteSpace(id))
        //                continue;

        //            var result = await _orderRepo.DeleteAsync(id);
        //            results.Add(result.Success);
        //        }

        //        return new GeneralResponse<bool>
        //        {
        //            Success = true,
        //            Message = "Batch delete executed",
        //            Data = results.All(r => r)
        //        };
        //    }
        //    catch (Exception ex)
        //    {
        //        return new GeneralResponse<bool>
        //        {
        //            Success = false,
        //            Message = $"Failed to delete orders: {ex.Message}"
        //        };
        //    }
        //}

        //public async Task<GeneralResponse<IEnumerable<OrderReadDTO>>> GetByClientAsync(string clientId, string userId)
        //{
        //    if (string.IsNullOrWhiteSpace(clientId))
        //        return new GeneralResponse<IEnumerable<OrderReadDTO>> { Success = false, Message = "Client ID is required" };

        //    try
        //    {
        //        var orders = await _orderRepo.QueryAsync(o =>
        //            o.ClientId == clientId && (string.IsNullOrEmpty(userId) || o.Client.UserId == userId),
        //            q => q.Include(o => o.Store).Include(o => o.Client).Include(o => o.OrderItems));

        //        return new GeneralResponse<IEnumerable<OrderReadDTO>>
        //        {
        //            Success = true,
        //            Message = orders.Any() ? "Orders retrieved successfully" : "No orders found for this client",
        //            Data = _mapper.Map<IEnumerable<OrderReadDTO>>(orders)
        //        };
        //    }
        //    catch (Exception ex)
        //    {
        //        return new GeneralResponse<IEnumerable<OrderReadDTO>>
        //        {
        //            Success = false,
        //            Message = $"Error retrieving orders: {ex.Message}"
        //        };
        //    }
        //}

        //public async Task<GeneralResponse<IEnumerable<OrderReadDTO>>> GetByStatusAsync(OrderStatus status, string userId)
        //{
        //    try
        //    {
        //        var orders = await _orderRepo.QueryAsync(o =>
        //            o.OrderStatus == status && (string.IsNullOrEmpty(userId) || o.Client.UserId == userId),
        //            q => q.Include(o => o.Store).Include(o => o.Client).Include(o => o.OrderItems));

        //        return new GeneralResponse<IEnumerable<OrderReadDTO>>
        //        {
        //            Success = true,
        //            Message = orders.Any() ? "Orders retrieved successfully" : "No orders found with this status",
        //            Data = _mapper.Map<IEnumerable<OrderReadDTO>>(orders)
        //        };
        //    }
        //    catch (Exception ex)
        //    {
        //        return new GeneralResponse<IEnumerable<OrderReadDTO>>
        //        {
        //            Success = false,
        //            Message = $"Error retrieving orders: {ex.Message}"
        //        };
        //    }
        //}

        //public async Task<GeneralResponse<decimal>> GetTotalRevenueAsync(string userId)
        //{
        //    try
        //    {
        //        var orders = await _orderRepo.GetAllAsync(userId);
        //        var totalRevenue = orders.Where(o => o.OrderStatus == OrderStatus.Delivered).Sum(o => o.TotalAmount);

        //        return new GeneralResponse<decimal>
        //        {
        //            Success = true,
        //            Message = "Total revenue calculated successfully",
        //            Data = totalRevenue
        //        };
        //    }
        //    catch (Exception ex)
        //    {
        //        return new GeneralResponse<decimal>
        //        {
        //            Success = false,
        //            Message = $"Error calculating revenue: {ex.Message}"
        //        };
        //    }
        //}

        //public async Task<bool> ExistsAsync(string id, string userId)
        //{
        //    if (string.IsNullOrWhiteSpace(id))
        //        return false;

        //    var entity = await _orderRepo.GetByIdAsync(id, userId);
        //    return entity != null;
        //}

        //public async Task<int> CountAsync(string userId)
        //{
        //    var entities = await _orderRepo.GetAllAsync(userId);
        //    return entities.Count();
        //}

        //public async Task<int> CountByStatusAsync(OrderStatus status, string userId)
        //{
        //    var entities = await _orderRepo.QueryAsync(o =>
        //        o.OrderStatus == status && (string.IsNullOrEmpty(userId) || o.Client.UserId == userId));
        //    return entities.Count();
        //}
    }
}