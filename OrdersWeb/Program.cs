using OrdersWeb.Services;
using OrdersWeb.Components;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

// Registra o ApiClient como Typed HttpClient apontando pra API
builder.Services.AddHttpClient<OrdersApiClient>(client =>
{
    client.BaseAddress = new Uri("http://localhost:5000/");
});

var app = builder.Build();

app.UseStaticFiles();
app.UseAntiforgery();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();