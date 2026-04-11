using OrdersApi.Dtos;
using OrdersApi.Models;

namespace OrdersApi.Services;

public interface IOrderService
{
    Task<List<Order>> GetAllAsync();
    Task<Order?> GetByIdAsync(int id);
    Task<Order> CreateAsync(CreateOrderDto dto);
}