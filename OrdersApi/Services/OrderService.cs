using MassTransit;
using OrdersApi.Dtos;
using OrdersApi.Messaging;
using OrdersApi.Models;
using OrdersApi.Repositories;

namespace OrdersApi.Services;

public class OrderService : IOrderService
{
    private readonly IOrderRepository _repository;
    private readonly IPublishEndpoint _publishEndpoint;

    public OrderService(IOrderRepository repository, IPublishEndpoint publishEndpoint)
    {
        _repository = repository;
        _publishEndpoint = publishEndpoint;
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

    public async Task CreateAsync(CreateOrderDto dto)
    {
        var message = new OrderCreatedMessage(
            Cliente: dto.Cliente,
            Valor: dto.Valor,
            DataPedido: DateTime.UtcNow
        );

        await _publishEndpoint.Publish(message);
    }

    private static OrderResponseDto MapToDto(Order order) =>
        new(order.Id, order.Cliente, order.Valor, order.DataPedido);
}