using Microsoft.EntityFrameworkCore;
using OrdersApi.Data;
using OrdersApi.Repositories;
using OrdersApi.Services;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

// Registra os controllers no container de DI
builder.Services.AddControllers();

// Registra o DbContext do EF Core, apontando pro banco de dados SQL Server
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("Default")));

// Registra o repositório como Scoped (uma instância por requisição HTTP)
builder.Services.AddScoped<IOrderRepository, OrderRepository>();
builder.Services.AddScoped<IOrderService, OrderService>();

// Gera documento OpenAPI nativo do .NET 10
builder.Services.AddOpenApi();

var app = builder.Build();

// Em desenvolvimento, expõe o documento OpenAPI e a UI do Scalar
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();              // serve o JSON em /openapi/v1.json
    app.MapScalarApiReference();   // serve a UI em /scalar/v1
}

app.MapControllers();

app.Run();