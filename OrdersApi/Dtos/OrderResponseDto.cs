namespace OrdersApi.Dtos;

public record OrderResponseDto(
    int Id,
    string Cliente,
    decimal valor,
    DateTime DataPedido
);