namespace OrdersApi.Dtos;

public record OrderResponseDto(
    int Id,
    string Cliente,
    decimal Valor,
    DateTime DataPedido
);