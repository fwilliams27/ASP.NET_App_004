using System.Diagnostics; 
using Microsoft.AspNetCore.Mvc;
using ASP.NET_App_004.Models;
using ASP.NET_App_004.Services;

namespace ASP.NET_App_004.Controllers;

[Route("[controller]")]
public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly JsonFileProductService _productService;

    public HomeController(ILogger<HomeController> logger, JsonFileProductService productService)
    {
        _logger = logger;
        _productService = productService;
    }

    [HttpGet]
    public IActionResult Index()
    {
        var products = _productService.GetProducts();
        ViewData["mydata"] = "You choose, Red Pill or Blue Pill";
        return View(products);
    }

    [HttpGet("privacy")]
    public IActionResult Privacy()
    {
        return View();
    }

    [HttpGet("products")]
    public IEnumerable<Product> GetProducts()
    {
        return _productService.GetProducts();
    }

    [HttpGet("products/rate")]
    public IActionResult RateProduct([FromQuery] string ProductId, [FromQuery] int Rating)
    {
        _productService.AddRating(ProductId, Rating);
        return Ok();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}  