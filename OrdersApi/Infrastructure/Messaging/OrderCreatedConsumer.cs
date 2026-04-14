using MassTransit;
using OrdersApi.Domain.Entities;
using OrdersApi.Domain.Repositories;

namespace OrdersApi.Infrastructure.Messaging;

public class OrderCreatedConsumer : IConsumer<OrderCreatedMessage>
{
    private readonly IOrderRepository _repository;
    private readonly ILogger<OrderCreatedConsumer> _logger;

    public OrderCreatedConsumer(
        IOrderRepository repository,
        ILogger<OrderCreatedConsumer> logger)
    {
        _repository = repository;
        _logger = logger;
    }

    public async Task Consume(ConsumeContext<OrderCreatedMessage> context)
    {
        var message = context.Message;

        _logger.LogInformation(
            "Processando pedido do cliente {Cliente} no valor de {Valor:C}",
            message.Cliente, message.Valor);

        var order = new Order
        {
            Cliente = message.Cliente,
            Valor = message.Valor,
            DataPedido = message.DataPedido
        };

        await _repository.AddAsync(order);

        _logger.LogInformation(
            "Pedido {OrderId} persistido com sucesso",
            order.Id);
    }
}