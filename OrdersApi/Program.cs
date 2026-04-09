using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

// Registra os controllers no container de DI
builder.Services.AddControllers();

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