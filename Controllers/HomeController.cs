using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using ASP.NET_App_004.Models;
using ASP.NET_App_004.Services;

namespace ASP.NET_App_004.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly JsonFileProductService _productService;

    public HomeController(ILogger<HomeController> logger, JsonFileProductService productService)
    {
        _logger = logger;
        _productService = productService;
    }

    public IActionResult Index()
    {
        ViewData["mydata"] = "You choose, Red Pill or Blue Pill";
        return View();
    }

    public IActionResult Products()
    {
        var products = _productService.GetProducts();
        return View(products);
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
