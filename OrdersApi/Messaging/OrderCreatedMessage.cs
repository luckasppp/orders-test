namespace OrdersApi.Messaging;

public record OrderCreatedMessage(
    string Cliente,
    decimal Valor,
    DateTime DataPedido
);