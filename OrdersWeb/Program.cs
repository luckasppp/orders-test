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
        var baseAddress = builder.Configuration["OrdersApi:BaseAddress"]
                        ?? "http://localhost:5000/";
        client.BaseAddress = new Uri(baseAddress);
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
    Environment.ExitCode = 1;
    throw;
}
finally
{ 
    Log.CloseAndFlush();
}