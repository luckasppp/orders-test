using MassTransit;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Driver;
using Moq;
using OrdersApi.Models;
using OrdersApi.Repositories;
using OrdersApi.Data;

namespace OrdersApi.Tests.Integration;

public class CustomWebApplicationFactory : WebApplicationFactory<Program>
{
    private static readonly InMemoryDatabaseRoot _databaseRoot = new();

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureServices(services =>
        {
            // Remove TODOS os serviços relacionados ao DbContext e ao provider SqlServer
            var descriptorsToRemove = services
                .Where(d =>
                    d.ServiceType == typeof(DbContextOptions<AppDbContext>) ||
                    d.ServiceType == typeof(DbContextOptions) ||
                    d.ServiceType == typeof(AppDbContext) ||
                    (d.ServiceType.FullName?.Contains("EntityFrameworkCore") == true &&
                     d.ImplementationType?.FullName?.Contains("SqlServer") == true))
                .ToList();

            foreach (var descriptor in descriptorsToRemove)
            {
                services.Remove(descriptor);
            }

            // Registra DbContext in-memory com raiz compartilhada + service provider isolado
            services.AddDbContext<AppDbContext>(options =>
            {
                options.UseInMemoryDatabase("OrdersTestDb", _databaseRoot);
                options.UseInternalServiceProvider(
                    new ServiceCollection()
                        .AddEntityFrameworkInMemoryDatabase()
                        .BuildServiceProvider());
            });

            // Substitui MassTransit por test harness
            services.AddMassTransitTestHarness();

            // Remove hosted service do MassTransit real
            var busHostedService = services.SingleOrDefault(
                d => d.ImplementationType?.FullName?.Contains("MassTransitHostedService") == true);
            if (busHostedService is not null)
            {
                services.Remove(busHostedService);
            }

            // Remove MongoDB e cache reais
            var mongoDescriptors = services
                .Where(d =>
                    d.ServiceType == typeof(IMongoClient) ||
                    d.ServiceType == typeof(IOrderCacheRepository))
                .ToList();

            foreach (var descriptor in mongoDescriptors)
            {
                services.Remove(descriptor);
            }

            // Substitui cache por mock que nunca encontra nada
            var cacheMock = new Mock<IOrderCacheRepository>();
            cacheMock
                .Setup(c => c.GetByIdAsync(It.IsAny<int>()))
                .ReturnsAsync((CachedOrder?)null);
            cacheMock
                .Setup(c => c.SetAsync(It.IsAny<CachedOrder>()))
                .Returns(Task.CompletedTask);

            services.AddSingleton(cacheMock.Object);
        });

        builder.UseEnvironment("Testing");
    }
}