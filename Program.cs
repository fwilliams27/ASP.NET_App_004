using ASP.NET_App_004.Services;
using System.Text.Json;
using ASP.NET_App_004.Components;   // Importing the Blazor component
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.AspNetCore.Components.Server.Circuits; // For CircuitHandler
using Microsoft.AspNetCore.SignalR; // For HubFilter

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();
builder.Services.AddControllersWithViews();
builder.Services.AddTransient<JsonFileProductService>(); // Registering JsonFileProductService
builder.Services.AddServerSideBlazor(); // Registering Blazor

// Configure CircuitOptions for better error handling
builder.Services.AddServerSideBlazor().AddCircuitOptions(options =>
{
    options.DisconnectedCircuitRetentionPeriod = TimeSpan.FromMinutes(5); // Retain circuit for 5 minutes after disconnection
    options.DisconnectedCircuitMaxRetained = 100; // Maximum number of retained circuits
    options.DetailedErrors = true; // Enable detailed errors for debugging
    options.JSInteropDefaultCallTimeout = TimeSpan.FromMinutes(3); // Increase JS interop timeout to 3 minutes
});

// Enable detailed logging
builder.Services.AddLogging(logging =>
{
    logging.AddConsole();
    logging.SetMinimumLevel(LogLevel.Debug); // Set minimum log level to Debug for detailed logs
    logging.AddFilter("Microsoft.AspNetCore.SignalR", LogLevel.Debug); // Ensure SignalR logs are captured
    logging.AddFilter("Microsoft.AspNetCore.Http.Connections", LogLevel.Debug); // Ensure WebSocket logs are captured
    logging.AddFilter("Microsoft.AspNetCore.Components", LogLevel.Debug); // Add logging for Blazor components
});

// Add a CircuitHandler to catch unhandled exceptions
builder.Services.AddScoped<CircuitHandler>(sp => new CustomCircuitHandler());

// Add a HubFilter to log WebSocket connection events
builder.Services.AddSignalR().AddHubOptions<Microsoft.AspNetCore.SignalR.Hub>(options =>
{
    options.AddFilter<CustomHubFilter>();
    options.EnableDetailedErrors = true; // Enable detailed SignalR errors
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
app.UseWebSockets(); // Explicitly enable WebSocket support for Blazor Server
app.UseAuthorization();

// Map Blazor Hub first to prioritize Blazor routing
app.MapBlazorHub();

// Map Razor Pages and Controllers, excluding /Products to avoid conflict
app.MapRazorPages();
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}"
);

// Adding MapGet for "/products" endpoint with error handling
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

// Custom CircuitHandler to catch unhandled exceptions
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

// Custom HubFilter to log WebSocket connection events
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