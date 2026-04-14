using OrdersApi.Domain.Entities;

namespace OrdersApi.Domain.Repositories;

public interface IOrderRepository
{
    Task<List<Order>> GetAllAsync();
    Task<Order?> GetByIdAsync(int id);
    Task AddAsync(Order order);
}