using OrdersApi.Shared.Dtos;

namespace OrdersApi.Application.UseCases;

public interface IOrderService
{
    Task<List<OrderResponseDto>> GetAllAsync();
    Task<OrderResponseDto?> GetByIdAsync(int id);
    Task CreateAsync(CreateOrderDto dto);
}