using ASP.NET_App_004.Services;
using System.Text.Json;
using ASP.NET_App_004.Components;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.AspNetCore.Components.Server.Circuits;
using Microsoft.AspNetCore.SignalR;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();
builder.Services.AddControllersWithViews();
builder.Services.AddTransient<JsonFileProductService>();
builder.Services.AddServerSideBlazor().AddCircuitOptions(options =>
{
    options.DisconnectedCircuitRetentionPeriod = TimeSpan.FromMinutes(5);
    options.DisconnectedCircuitMaxRetained = 100;
    options.DetailedErrors = true;
    options.JSInteropDefaultCallTimeout = TimeSpan.FromMinutes(3);
});

builder.Services.AddLogging(logging =>
{
    logging.AddConsole();
    logging.SetMinimumLevel(LogLevel.Debug);
    logging.AddFilter("Microsoft.AspNetCore.SignalR", LogLevel.Debug);
    logging.AddFilter("Microsoft.AspNetCore.Http.Connections", LogLevel.Debug);
    logging.AddFilter("Microsoft.AspNetCore.Components", LogLevel.Debug);
});

builder.Services.AddScoped<CircuitHandler>(sp => new CustomCircuitHandler());

builder.Services.AddSignalR().AddHubOptions<Microsoft.AspNetCore.SignalR.Hub>(options =>
{
    options.AddFilter<CustomHubFilter>();
    options.EnableDetailedErrors = true;
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseWebSockets();
app.UseAuthorization();

// Map Blazor Hub to enable Blazor Server functionality
app.MapBlazorHub();

// Map fallback to _Host to load Blazor pages when no other route matches
app.MapFallbackToPage("/_Host");

app.MapRazorPages();
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}"
);

// Optional API endpoint for products
app.MapGet("/products", (HttpContext context, JsonFileProductService productService) =>
{
    try
    {
        var products = productService.GetProducts();
        var json = JsonSerializer.Serialize(products);
        Console.WriteLine($"Successfully fetched {products.Count()} products from /products endpoint.");
        return context.Response.WriteAsync(json);
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Error in /products endpoint: {ex.Message}\n{ex.StackTrace}");
        context.Response.StatusCode = 500;
        return context.Response.WriteAsync("{\"error\": \"Internal server error while fetching products.\"}");
    }
});

app.Run();

public class CustomCircuitHandler : CircuitHandler
{
    public override async Task OnCircuitOpenedAsync(Circuit circuit, CancellationToken cancellationToken)
    {
        Console.WriteLine($"Circuit opened for {circuit.Id}");
        try
        {
            await base.OnCircuitOpenedAsync(circuit, cancellationToken);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Exception in OnCircuitOpenedAsync for circuit {circuit.Id}: {ex.Message}\n{ex.StackTrace}");
            throw;
        }
    }

    public override async Task OnCircuitClosedAsync(Circuit circuit, CancellationToken cancellationToken)
    {
        Console.WriteLine($"Circuit closed for {circuit.Id}");
        try
        {
            await base.OnCircuitClosedAsync(circuit, cancellationToken);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Exception in OnCircuitClosedAsync for circuit {circuit.Id}: {ex.Message}\n{ex.StackTrace}");
            throw;
        }
    }

    public override async Task OnConnectionUpAsync(Circuit circuit, CancellationToken cancellationToken)
    {
        Console.WriteLine($"Connection up for circuit {circuit.Id}");
        try
        {
            await base.OnConnectionUpAsync(circuit, cancellationToken);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Exception in OnConnectionUpAsync for circuit {circuit.Id}: {ex.Message}\n{ex.StackTrace}");
            throw;
        }
    }

    public override async Task OnConnectionDownAsync(Circuit circuit, CancellationToken cancellationToken)
    {
        Console.WriteLine($"Connection down for circuit {circuit.Id}");
        try
        {
            await base.OnConnectionDownAsync(circuit, cancellationToken);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Exception in OnConnectionDownAsync for circuit {circuit.Id}: {ex.Message}\n{ex.StackTrace}");
            throw;
        }
    }
}

public class CustomHubFilter : IHubFilter
{
    public async ValueTask<object?> InvokeMethodAsync(HubInvocationContext invocationContext, Func<HubInvocationContext, ValueTask<object?>> next)
    {
        Console.WriteLine($"Hub method invoked: {invocationContext.HubMethodName}");
        try
        {
            return await next(invocationContext);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error in hub method {invocationContext.HubMethodName}: {ex.Message}\n{ex.StackTrace}");
            throw;
        }
    }

    public Task OnConnectedAsync(HubConnectionContext connectionContext, Func<HubConnectionContext, Task> next)
    {
        Console.WriteLine($"Client connected: {connectionContext.ConnectionId}");
        return next(connectionContext);
    }

    public Task OnDisconnectedAsync(HubConnectionContext connectionContext, Exception? exception, Func<HubConnectionContext, Exception?, Task> next)
    {
        Console.WriteLine($"Client disconnected: {connectionContext.ConnectionId}, Exception: {exception?.Message}\n{exception?.StackTrace}");
        return next(connectionContext, exception);
    }
}
