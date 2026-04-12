using System.Net.Http.Json;

namespace OrdersWeb.Services;

public class OrdersApiClient
{
    private readonly HttpClient _http;

    public OrdersApiClient(HttpClient http)
    {
        _http = http;
    }

    public async Task<List<OrderResponse>> GetAllAsync()
    {
        var orders = await _http.GetFromJsonAsync<List<OrderResponse>>("orders");
        return orders ?? new List<OrderResponse>();
    }

    public async Task<OrderResponse?> GetByIdAsync(int id)
    {
        try
        {
            return await _http.GetFromJsonAsync<OrderResponse>($"orders/{id}");
        }
        catch (HttpRequestException ex) when (ex.StatusCode == System.Net.HttpStatusCode.NotFound)
        {
            return null;
        }
    }

    public async Task CreateAsync(CreateOrderRequest request)
    {
        var response = await _http.PostAsJsonAsync("orders", request);
        response.EnsureSuccessStatusCode();
    }
}

public record OrderResponse(int Id, string Cliente, decimal Valor, DateTime DataPedido);
public record CreateOrderRequest(string Cliente, decimal Valor);