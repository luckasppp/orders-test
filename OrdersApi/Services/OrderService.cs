using OrdersApi.Dtos;
using OrdersApi.Models;
using OrdersApi.Repositories;

namespace OrdersApi.Services;

public class OrderService : IOrderService
{
    private readonly IOrderRepository _repository;

    public OrderService(IOrderRepository repository)
    {
        _repository = repository;
    }

    public async Task<List<OrderResponseDto>> GetAllAsync()
    {
        var orders = await _repository.GetAllAsync();
        return orders.Select(MapToDto).ToList();
    }

    public async Task<OrderResponseDto?> GetByIdAsync(int id)
    {
        var order = await _repository.GetByIdAsync(id);
        return order == null ? null : MapToDto(order);
    }

    public async Task<OrderResponseDto> CreateAsync(CreateOrderDto dto)
    {
        var order = new Order
        {
            Cliente = dto.Cliente,
            Valor = dto.Valor,
            DataPedido = DateTime.UtcNow
        };

        await _repository.AddAsync(order);

        return MapToDto(order);
    }

    private static OrderResponseDto MapToDto(Order order) =>
        new(order.Id, order.Cliente, order.Valor, order.DataPedido);
}