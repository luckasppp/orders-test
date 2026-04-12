using OrdersApi.Dtos;

namespace OrdersApi.Services;

public interface IOrderService
{
    Task<List<OrderResponseDto>> GetAllAsync();
    Task<OrderResponseDto?> GetByIdAsync(int id);
    Task CreateAsync(CreateOrderDto dto);
}