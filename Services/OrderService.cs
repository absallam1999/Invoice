using AutoMapper;
using invoice.Core.DTO;
using invoice.Core.DTO.Order;
using invoice.Core.Entites;
using invoice.Core.Interfaces.Services;
using invoice.Repo;
using Microsoft.EntityFrameworkCore;

namespace invoice.Services
{
    public class OrderService : IOrderService
    {
        private readonly IRepository<Order> _orderRepo;
        private readonly IRepository<Client> _clientRepo;
        private readonly IMapper _mapper;

        public OrderService(IRepository<Order> orderRepo, IRepository<Client> clientRepo, IMapper mapper)
        {
            _orderRepo = orderRepo;
            _clientRepo = clientRepo;
            _mapper = mapper;
        }

        public async Task<GeneralResponse<IEnumerable<OrderReadDTO>>> GetAllAsync(string userId)
        {
            try
            {
                var orders = await _orderRepo.GetQueryable()
                    .Include(o => o.Store)
                    .Include(o => o.Client)
                    .Include(o => o.Invoice)
                    .Include(o => o.OrderItems).ThenInclude(oi => oi.Product)
                    .Where(o => o.Client.UserId == userId)
                    .ToListAsync();

                var dto = _mapper.Map<IEnumerable<OrderReadDTO>>(orders);
                return new GeneralResponse<IEnumerable<OrderReadDTO>>(true, "Orders retrieved successfully.", dto);
            }
            catch (Exception ex)
            {
                return new GeneralResponse<IEnumerable<OrderReadDTO>>(false, $"Error retrieving orders: {ex.Message}");
            }
        }

        public async Task<GeneralResponse<OrderReadDTO>> GetByIdAsync(string id, string userId)
        {
            try
            {
                var order = await _orderRepo.GetQueryable()
                    .Include(o => o.Store)
                    .Include(o => o.Client)
                    .Include(o => o.Invoice)
                    .Include(o => o.OrderItems).ThenInclude(oi => oi.Product)
                    .FirstOrDefaultAsync(o => o.Id == id && o.Client.UserId == userId);

                if (order == null)
                    return new GeneralResponse<OrderReadDTO>(false, "Order not found or access denied.");

                return new GeneralResponse<OrderReadDTO>(true, "Order retrieved successfully.", _mapper.Map<OrderReadDTO>(order));
            }
            catch (Exception ex)
            {
                return new GeneralResponse<OrderReadDTO>(false, $"Error retrieving order: {ex.Message}");
            }
        }

        public async Task<GeneralResponse<OrderReadDTO>> AddAsync(OrderCreateDTO dto, string userId)
        {
            try
            {
                var client = await _clientRepo.GetQueryable()
                    .FirstOrDefaultAsync(c => c.Id == dto.ClientId && c.UserId == userId);

                if (client == null)
                    return new GeneralResponse<OrderReadDTO>(false, "Invalid client or access denied.");

                var order = _mapper.Map<Order>(dto);
                order.ClientId = client.Id;
                order.CreatedAt = DateTime.UtcNow;

                var response = await _orderRepo.AddAsync(order);

                return new GeneralResponse<OrderReadDTO>(
                    response.Success,
                    response.Message,
                    _mapper.Map<OrderReadDTO>(response.Data)
                );
            }
            catch (Exception ex)
            {
                return new GeneralResponse<OrderReadDTO>(false, $"Failed to create order: {ex.Message}");
            }
        }

        public async Task<GeneralResponse<OrderReadDTO>> UpdateAsync(OrderUpdateDTO dto, string userId)
        {
            try
            {
                var existingOrder = await _orderRepo.GetQueryable()
                    .Include(o => o.OrderItems)
                    .Include(o => o.Client)
                    .FirstOrDefaultAsync(o => o.Id == dto.Id && o.Client.UserId == userId);

                if (existingOrder == null)
                    return new GeneralResponse<OrderReadDTO>(false, "Order not found or access denied.");

                _mapper.Map(dto, existingOrder);
                existingOrder.UpdatedAt = DateTime.UtcNow;

                var response = await _orderRepo.UpdateAsync(existingOrder);

                return new GeneralResponse<OrderReadDTO>(
                    response.Success,
                    response.Message,
                    _mapper.Map<OrderReadDTO>(response.Data)
                );
            }
            catch (Exception ex)
            {
                return new GeneralResponse<OrderReadDTO>(false, $"Failed to update order: {ex.Message}");
            }
        }

        public async Task<GeneralResponse<OrderReadDTO>> DeleteAsync(string id, string userId)
        {
            try
            {
                var existingOrder = await _orderRepo.GetQueryable()
                    .Include(o => o.Client)
                    .FirstOrDefaultAsync(o => o.Id == id && o.Client.UserId == userId);

                if (existingOrder == null)
                    return new GeneralResponse<OrderReadDTO>(false, "Order not found or access denied.");

                var response = await _orderRepo.DeleteAsync(id);

                return new GeneralResponse<OrderReadDTO>(
                    response.Success,
                    response.Message,
                    _mapper.Map<OrderReadDTO>(response.Data)
                );
            }
            catch (Exception ex)
            {
                return new GeneralResponse<OrderReadDTO>(false, $"Failed to delete order: {ex.Message}");
            }
        }

        public async Task<GeneralResponse<IEnumerable<OrderReadDTO>>> AddRangeAsync(IEnumerable<OrderCreateDTO> dtos, string userId)
        {
            try
            {
                var clientIds = dtos.Select(d => d.ClientId).Distinct().ToList();
                var validClients = await _clientRepo.GetQueryable()
                    .Where(c => clientIds.Contains(c.Id) && c.UserId == userId)
                    .ToListAsync();

                if (validClients.Count != clientIds.Count)
                    return new GeneralResponse<IEnumerable<OrderReadDTO>>(false, "One or more clients are invalid or access denied.");

                var orders = _mapper.Map<IEnumerable<Order>>(dtos);
                foreach (var order in orders)
                {
                    order.CreatedAt = DateTime.UtcNow;
                }

                var response = await _orderRepo.AddRangeAsync(orders);

                return new GeneralResponse<IEnumerable<OrderReadDTO>>(
                    response.Success,
                    response.Message,
                    _mapper.Map<IEnumerable<OrderReadDTO>>(response.Data)
                );
            }
            catch (Exception ex)
            {
                return new GeneralResponse<IEnumerable<OrderReadDTO>>(false, $"Failed to create orders: {ex.Message}");
            }
        }

        public async Task<GeneralResponse<IEnumerable<OrderReadDTO>>> UpdateRangeAsync(IEnumerable<OrderUpdateDTO> dtos, string userId)
        {
            try
            {
                var ids = dtos.Select(d => d.Id).ToList();
                var existingOrders = await _orderRepo.GetQueryable()
                    .Include(o => o.OrderItems)
                    .Include(o => o.Client)
                    .Where(o => ids.Contains(o.Id) && o.Client.UserId == userId)
                    .ToListAsync();

                if (existingOrders.Count != ids.Count)
                    return new GeneralResponse<IEnumerable<OrderReadDTO>>(false, "One or more orders not found or access denied.");

                _mapper.Map(dtos, existingOrders);

                foreach (var order in existingOrders)
                {
                    order.UpdatedAt = DateTime.UtcNow;
                }

                var response = await _orderRepo.UpdateRangeAsync(existingOrders);

                return new GeneralResponse<IEnumerable<OrderReadDTO>>(
                    response.Success,
                    response.Message,
                    _mapper.Map<IEnumerable<OrderReadDTO>>(response.Data)
                );
            }
            catch (Exception ex)
            {
                return new GeneralResponse<IEnumerable<OrderReadDTO>>(false, $"Failed to update orders: {ex.Message}");
            }
        }

        public async Task<GeneralResponse<IEnumerable<OrderReadDTO>>> DeleteRangeAsync(IEnumerable<string> ids, string userId)
        {
            try
            {
                var existingOrders = await _orderRepo.GetQueryable()
                    .Include(o => o.Client)
                    .Where(o => ids.Contains(o.Id) && o.Client.UserId == userId)
                    .ToListAsync();

                if (!existingOrders.Any())
                    return new GeneralResponse<IEnumerable<OrderReadDTO>>(false, "No orders found for deletion or access denied.");

                var response = await _orderRepo.DeleteRangeAsync(ids);

                return new GeneralResponse<IEnumerable<OrderReadDTO>>(
                    response.Success,
                    response.Message,
                    _mapper.Map<IEnumerable<OrderReadDTO>>(response.Data)
                );
            }
            catch (Exception ex)
            {
                return new GeneralResponse<IEnumerable<OrderReadDTO>>(false, $"Failed to delete orders: {ex.Message}");
            }
        }
    }
}
