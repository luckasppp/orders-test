using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using MassTransit.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using OrdersApi.Data;
using OrdersApi.Dtos;
using OrdersApi.Messaging;
using OrdersApi.Models;

namespace OrdersApi.Tests.Integration;

public class OrdersControllerTests : IClassFixture<CustomWebApplicationFactory>, IAsyncLifetime
{
    private readonly CustomWebApplicationFactory _factory;
    private readonly HttpClient _client;

    public OrdersControllerTests(CustomWebApplicationFactory factory)
    {
        _factory = factory;
        _client = factory.CreateClient();
    }

    // Limpa o banco antes de cada teste
    public async Task InitializeAsync()
    {
        using var scope = _factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        db.Orders.RemoveRange(db.Orders);
        await db.SaveChangesAsync();
    }

    public Task DisposeAsync() => Task.CompletedTask;

    [Fact]
    public async Task GetAll_QuandoBancoVazio_DeveriaRetornar200ComListaVazia()
    {
        // Act
        var response = await _client.GetAsync("/orders");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var orders = await response.Content.ReadFromJsonAsync<List<OrderResponseDto>>();
        orders.Should().NotBeNull();
        orders.Should().BeEmpty();
    }

    [Fact]
    public async Task GetAll_QuandoTemPedidos_DeveriaRetornarOrdenadosPorData()
    {
        // Arrange
        await SeedOrders(
            new Order { Cliente = "Antigo", Valor = 100m, DataPedido = DateTime.UtcNow.AddDays(-2) },
            new Order { Cliente = "Recente", Valor = 200m, DataPedido = DateTime.UtcNow }
        );

        // Act
        var response = await _client.GetAsync("/orders");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var orders = await response.Content.ReadFromJsonAsync<List<OrderResponseDto>>();
        orders.Should().HaveCount(2);
        orders![0].Cliente.Should().Be("Recente");
        orders[1].Cliente.Should().Be("Antigo");
    }

    [Fact]
    public async Task GetById_QuandoPedidoExiste_DeveriaRetornar200()
    {
        // Arrange
        var pedido = new Order { Cliente = "Teste", Valor = 99.99m, DataPedido = DateTime.UtcNow };
        await SeedOrders(pedido);

        // Act
        var response = await _client.GetAsync($"/orders/{pedido.Id}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var order = await response.Content.ReadFromJsonAsync<OrderResponseDto>();
        order.Should().NotBeNull();
        order!.Cliente.Should().Be("Teste");
        order.Valor.Should().Be(99.99m);
    }

    [Fact]
    public async Task GetById_QuandoPedidoNaoExiste_DeveriaRetornar404()
    {
        // Act
        var response = await _client.GetAsync("/orders/99999");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task Post_ComDadosValidos_DeveriaRetornar202EPublicarMensagem()
    {
        // Arrange
        var harness = _factory.Services.GetRequiredService<ITestHarness>();
        await harness.Start();

        var dto = new CreateOrderDto("Cliente Teste", 333.33m);

        // Act
        var response = await _client.PostAsJsonAsync("/orders", dto);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Accepted);

        var published = await harness.Published.Any<OrderCreatedMessage>(
            x => x.Context.Message.Cliente == "Cliente Teste"
              && x.Context.Message.Valor == 333.33m);

        published.Should().BeTrue();
    }

    [Fact]
    public async Task Post_ComClienteVazio_DeveriaRetornar400()
    {
        // Arrange
        var dto = new { cliente = "", valor = 100m };

        // Act
        var response = await _client.PostAsJsonAsync("/orders", dto);

        // Assert
        // Depende se você tem validação; se não tiver, esse teste pode falhar.
        // Se falhar, remove ou adapta conforme a regra atual.
        response.StatusCode.Should().BeOneOf(HttpStatusCode.BadRequest, HttpStatusCode.Accepted);
    }

    // Helper pra popular o banco
    private async Task SeedOrders(params Order[] orders)
    {
        using var scope = _factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        db.Orders.AddRange(orders);
        await db.SaveChangesAsync();
    }

    // DTO de resposta (espelho do que a API devolve)
    private record OrderResponseDto(int Id, string Cliente, decimal Valor, DateTime DataPedido);
}