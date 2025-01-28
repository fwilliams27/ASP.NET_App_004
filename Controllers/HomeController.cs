using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using ASP.NET_App_004.Models;
using ASP.NET_App_004.Services;

namespace ASP.NET_App_004.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    public JsonFileProductService _productService;
    public IEnumerable<Product> Products { get; private set; }

    public HomeController(ILogger<HomeController> logger, JsonFileProductService productService)
    {
        _logger = logger;
        _productService = productService;
        Products = new List<Product>(); // Initialize with an empty list or fetch products from the service
    }

    public IActionResult Index()
    {
        // Fetch products using the JsonFileProductService
        Products = _productService.GetProducts();
        
        // Pass products to the view
        ViewData["mydata"] = "You choose, Red Pill or Blue Pill";
        return View(Products);
    }

    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
