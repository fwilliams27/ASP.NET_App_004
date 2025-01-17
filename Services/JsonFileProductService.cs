using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using ASP.NET_App_004.Models;
using Microsoft.AspNetCore.Hosting;

namespace ASP.NET_App_004.Services;

public class JsonFileProductService
{
    public JsonFileProductService(IWebHostEnvironment webHostEnvironment)
    {
        WebHostEnvironment = webHostEnvironment;
    }

    public IWebHostEnvironment WebHostEnvironment { get; }

    private string JsonFileName
    {
        get { return Path.Combine(WebHostEnvironment.WebRootPath, "data", "products.json"); }
    }

    public IEnumerable<Product> GetProducts()
    {
        if (!File.Exists(JsonFileName))
        {
            return Array.Empty<Product>();
        }

        using (var jsonFileReader = File.OpenText(JsonFileName))
        {
            var products = JsonSerializer.Deserialize<Product[]>(jsonFileReader.ReadToEnd(),
                new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

            return products ?? Array.Empty<Product>();
        }
    }
}
