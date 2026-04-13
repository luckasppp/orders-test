using FluentAssertions;
using MassTransit;
using Moq;
using OrdersApi.Dtos;
using OrdersApi.Messaging;
using OrdersApi.Models;
using OrdersApi.Repositories;
using OrdersApi.Services;

namespace OrdersApi.Tests.Unit;

public class OrderServiceTests
{
    private readonly Mock<IOrderRepository> _repositoryMock;
    private readonly Mock<IPublishEndpoint> _publishEndpointMock;
    private readonly OrderService _service;

    public OrderServiceTests()
    {
        _repositoryMock = new Mock<IOrderRepository>();
        _publishEndpointMock = new Mock<IPublishEndpoint>();
        _service = new OrderService(_repositoryMock.Object, _publishEndpointMock.Object);
    }

    [Fact]
    public async Task CreateAsync_DeveriaPublicarMensagemNoBus()
    {
        // Arrange
        var dto = new CreateOrderDto("João Silva", 150.00m);

        // Act
        await _service.CreateAsync(dto);

        // Assert
        _publishEndpointMock.Verify(
            p => p.Publish(
                It.Is<OrderCreatedMessage>(m =>
                    m.Cliente == "João Silva" && m.Valor == 150.00m),
                It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task CreateAsync_DeveriaDefinirDataPedidoComoAtual()
    {
        // Arrange
        var dto = new CreateOrderDto("Maria", 200.00m);
        var antes = DateTime.UtcNow;

        OrderCreatedMessage? mensagemPublicada = null;
        _publishEndpointMock
            .Setup(p => p.Publish(It.IsAny<OrderCreatedMessage>(), It.IsAny<CancellationToken>()))
            .Callback<OrderCreatedMessage, CancellationToken>((msg, _) => mensagemPublicada = msg);

        // Act
        await _service.CreateAsync(dto);

        // Assert
        mensagemPublicada.Should().NotBeNull();
        mensagemPublicada!.DataPedido.Should().BeOnOrAfter(antes);
        mensagemPublicada.DataPedido.Should().BeOnOrBefore(DateTime.UtcNow);
    }

    [Fact]
    public async Task CreateAsync_NaoDeveriaSalvarDiretamenteNoRepositorio()
    {
        // Arrange
        var dto = new CreateOrderDto("Pedro", 99.99m);

        // Act
        await _service.CreateAsync(dto);

        // Assert
        _repositoryMock.Verify(
            r => r.AddAsync(It.IsAny<Order>()),
            Times.Never);
    }

    [Fact]
    public async Task GetAllAsync_QuandoNaoTemPedidos_DeveriaRetornarListaVazia()
    {
        // Arrange
        _repositoryMock
            .Setup(r => r.GetAllAsync())
            .ReturnsAsync(new List<Order>());

        // Act
        var resultado = await _service.GetAllAsync();

        // Assert
        resultado.Should().NotBeNull();
        resultado.Should().BeEmpty();
    }

    [Fact]
    public async Task GetAllAsync_QuandoTemPedidos_DeveriaRetornarDtosMapeados()
    {
        // Arrange
        var pedidos = new List<Order>
        {
            new() { Id = 1, Cliente = "João", Valor = 100m, DataPedido = DateTime.UtcNow },
            new() { Id = 2, Cliente = "Maria", Valor = 200m, DataPedido = DateTime.UtcNow }
        };

        _repositoryMock
            .Setup(r => r.GetAllAsync())
            .ReturnsAsync(pedidos);

        // Act
        var resultado = await _service.GetAllAsync();

        // Assert
        resultado.Should().HaveCount(2);
        resultado[0].Cliente.Should().Be("João");
        resultado[0].Valor.Should().Be(100m);
        resultado[1].Cliente.Should().Be("Maria");
    }

    [Fact]
    public async Task GetByIdAsync_QuandoPedidoExiste_DeveriaRetornarDto()
    {
        // Arrange
        var pedido = new Order
        {
            Id = 42,
            Cliente = "Ana",
            Valor = 500m,
            DataPedido = DateTime.UtcNow
        };

        _repositoryMock
            .Setup(r => r.GetByIdAsync(42))
            .ReturnsAsync(pedido);

        // Act
        var resultado = await _service.GetByIdAsync(42);

        // Assert
        resultado.Should().NotBeNull();
        resultado!.Id.Should().Be(42);
        resultado.Cliente.Should().Be("Ana");
        resultado.Valor.Should().Be(500m);
    }

    [Fact]
    public async Task GetByIdAsync_QuandoPedidoNaoExiste_DeveriaRetornarNull()
    {
        // Arrange
        _repositoryMock
            .Setup(r => r.GetByIdAsync(It.IsAny<int>()))
            .ReturnsAsync((Order?)null);

        // Act
        var resultado = await _service.GetByIdAsync(999);

        // Assert
        resultado.Should().BeNull();
    }
}