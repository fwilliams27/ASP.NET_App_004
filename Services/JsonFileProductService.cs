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
        try
        {
            if (!File.Exists(JsonFileName))
            {
                Console.WriteLine($"Products file not found at: {JsonFileName}");
                return Array.Empty<Product>();
            }

            using var jsonFileReader = File.OpenText(JsonFileName);
            var jsonContent = jsonFileReader.ReadToEnd();
            if (string.IsNullOrEmpty(jsonContent))
            {
                Console.WriteLine($"Products file is empty: {JsonFileName}");
                return Array.Empty<Product>();
            }

            var products = JsonSerializer.Deserialize<Product[]>(jsonContent,
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
            Console.WriteLine($"Successfully deserialized {products?.Length ?? 0} products from {JsonFileName}");
            return products ?? Array.Empty<Product>();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error reading products.json: {ex.Message}\n{ex.StackTrace}");
            return Array.Empty<Product>();
        }
    }

    public void AddRating(string productId, int rating)
    {
        try
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
                Console.WriteLine($"Successfully added rating {rating} to product {productId}");
            }
            else
            {
                Console.WriteLine($"Product with ID {productId} not found for rating update.");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error adding rating to product {productId}: {ex.Message}\n{ex.StackTrace}");
        }
    }

    public void UpdateProduct(Product updatedProduct)
    {
        try
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
                Console.WriteLine($"Successfully updated product {updatedProduct.Id}");
            }
            else
            {
                Console.WriteLine($"Product with ID {updatedProduct.Id} not found for update.");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error updating product {updatedProduct?.Id}: {ex.Message}\n{ex.StackTrace}");
        }
    }
}