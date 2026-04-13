using FluentAssertions;
using MassTransit;
using Moq;
using OrdersApi.Dtos;
using OrdersApi.Messaging;
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
}