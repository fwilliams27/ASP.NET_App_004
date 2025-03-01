using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using ASP.NET_App_004.Models;
using Microsoft.AspNetCore.Hosting;
using System.Linq;

namespace ASP.NET_App_004.Services;

public class JsonFileProductService
{
    private readonly IWebHostEnvironment _webHostEnvironment;

    public JsonFileProductService(IWebHostEnvironment webHostEnvironment)
    {
        _webHostEnvironment = webHostEnvironment;
    }

    private string JsonFileName => Path.Combine(_webHostEnvironment.WebRootPath, "products.json");

    public IEnumerable<Product> GetProducts()
    {
        if (!File.Exists(JsonFileName))
        {
            return Array.Empty<Product>();
        }

        using var jsonFileReader = File.OpenText(JsonFileName);
        return JsonSerializer.Deserialize<Product[]>(jsonFileReader.ReadToEnd(),
               new JsonSerializerOptions { PropertyNameCaseInsensitive = true })
               ?? Array.Empty<Product>();
    }

    public void AddRating(string productId, int rating)
    {
        var products = GetProducts().ToList();
        var query = products.FirstOrDefault(x => x.Id == productId);

        if (query != null)
        {
            if (query.Ratings == null)
            {
                query.Ratings = new int[] { rating };
            }
            else
            {
                var ratings = query.Ratings.ToList();
                ratings.Add(rating);
                query.Ratings = ratings.ToArray();
            }

            using (var outputStream = File.OpenWrite(JsonFileName))
            {
                JsonSerializer.Serialize<IEnumerable<Product>>(
                    new Utf8JsonWriter(outputStream, new JsonWriterOptions
                    {
                        SkipValidation = true,
                        Indented = true
                    }),
                    products
                );
            }
        }
    }

    // Added UpdateProduct method to persist changes to a product
    public void UpdateProduct(Product updatedProduct)
    {
        var products = GetProducts().ToList();
        var existingProduct = products.FirstOrDefault(p => p.Id == updatedProduct.Id);

        if (existingProduct != null)
        {
            var index = products.IndexOf(existingProduct);
            products[index] = updatedProduct;

            using (var outputStream = File.OpenWrite(JsonFileName))
            {
                JsonSerializer.Serialize<IEnumerable<Product>>(
                    new Utf8JsonWriter(outputStream, new JsonWriterOptions
                    {
                        SkipValidation = true,
                        Indented = true
                    }),
                    products
                );
            }
        }
    }
}