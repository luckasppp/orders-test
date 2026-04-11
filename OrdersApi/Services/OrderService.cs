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

    public async Task<List<Order>> GetAllAsync()
    {
        return await _repository.GetAllAsync();
    }

    public async Task<Order?> GetByIdAsync(int id)
    {
        return await _repository.GetByIdAsync(id);
    }

    public async Task<Order> CreateAsync(CreateOrderDto dto)
    {
        var order = new Order
        {
            Cliente = dto.Cliente,
            Valor = dto.Valor,
            DataPedido = DateTime.UtcNow
        };

        await _repository.AddAsync(order);

        return order;
    }
}