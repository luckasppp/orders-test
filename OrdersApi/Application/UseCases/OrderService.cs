using MassTransit;
using OrdersApi.Domain.Entities;
using OrdersApi.Domain.Repositories;
using OrdersApi.Shared.Dtos;
using OrdersApi.Infrastructure.Messaging;
using OrdersApi.Infrastructure.Cache;
using OrdersApi.Application.UseCases;

namespace OrdersApi.Application.UseCases;

public class OrderService : IOrderService
{
    private readonly IOrderRepository _repository;
    private readonly IPublishEndpoint _publishEndpoint;
    private readonly IOrderCacheRepository _cache;

    public OrderService(
        IOrderRepository repository,
        IPublishEndpoint publishEndpoint,
        IOrderCacheRepository cache)
    {
        _repository = repository;
        _publishEndpoint = publishEndpoint;
        _cache = cache;
    }

    public async Task<List<OrderResponseDto>> GetAllAsync()
    {
        var orders = await _repository.GetAllAsync();
        return orders.Select(MapToDto).ToList();
    }

    public async Task<OrderResponseDto?> GetByIdAsync(int id)
    {
        var cached = await _cache.GetByIdAsync(id);
        if (cached != null)
            return MapToDto(cached);

        var order = await _repository.GetByIdAsync(id);
        if (order == null) return null;

        await _cache.SetAsync(new CachedOrder
        {
            Id = order.Id,
            Cliente = order.Cliente,
            Valor = order.Valor,
            DataPedido = order.DataPedido
        });

        return MapToDto(order);
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

    private static OrderResponseDto MapToDto(CachedOrder cached) =>
        new(cached.Id, cached.Cliente, cached.Valor, cached.DataPedido);
}