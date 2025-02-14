using ASP.NET_App_004.Services;
using System.Text.Json;
using ASP.NET_App_004.Components;   // Importing the Blazor component
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();
builder.Services.AddControllersWithViews();
builder.Services.AddTransient<JsonFileProductService>(); // Registering JsonFileProductService
builder.Services.AddServerSideBlazor(); // Registering Blazor
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
app.UseAuthorization();

// Map Razor Pages and Controllers
app.MapRazorPages();
app.MapBlazorHub();
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}"
);

// Adding MapGet for "/products" endpoint
app.MapGet("/products", (HttpContext context, JsonFileProductService productService) =>
{
    var products = productService.GetProducts();
    var json = JsonSerializer.Serialize(products);
    return context.Response.WriteAsync(json);
});

app.Run();
