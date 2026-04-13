using OrdersWeb.Components;
using OrdersWeb.Services;
using Serilog;

Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Information()
    .MinimumLevel.Override("Microsoft.AspNetCore", Serilog.Events.LogEventLevel.Warning)
    .Enrich.FromLogContext()
    .WriteTo.Console(outputTemplate:
        "[{Timestamp:HH:mm:ss} {Level:u3}] {Message:lj} {Properties:j}{NewLine}{Exception}")
    .WriteTo.File("logs/orders-web-.log",
        rollingInterval: RollingInterval.Day)
    .CreateLogger();

try
{
    Log.Information("Iniciando OrdersWeb");

    var builder = WebApplication.CreateBuilder(args);

    builder.Host.UseSerilog();

    builder.Services.AddRazorComponents()
        .AddInteractiveServerComponents();

    builder.Services.AddHttpClient<OrdersApiClient>(client =>
    {
        client.BaseAddress = new Uri("http://localhost:5000/");
    });

    var app = builder.Build();

    app.UseSerilogRequestLogging();

    app.UseStaticFiles();
    app.UseAntiforgery();

    app.MapRazorComponents<App>()
        .AddInteractiveServerRenderMode();

    app.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "OrdersWeb encerrou inesperadamente");
}
finally
{
    Log.CloseAndFlush();
}