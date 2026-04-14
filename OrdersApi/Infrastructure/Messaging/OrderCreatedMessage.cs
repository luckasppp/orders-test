namespace OrdersApi.Infrastructure.Messaging;

public record OrderCreatedMessage(
    string Cliente,
    decimal Valor,
    DateTime DataPedido
);