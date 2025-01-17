using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using ASP.NET_App_004.Models;

namespace ASP.NET_App_004.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;

    public HomeController(ILogger<HomeController> logger)
    {
        _logger = logger;
    }

    public IActionResult Index()
    {
        ViewData["mydata"] = "You choose, Red Pill or Blue Pill"; //*This controller will show on the main page and provide an option for the user to select which webpage will show.
        return View();
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
