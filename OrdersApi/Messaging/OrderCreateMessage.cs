namespace OrdersApi.Messaging;

public record OrderCreateMessage(
    string Cliente,
    decimal Valor,
    DateTime DataPedido
);