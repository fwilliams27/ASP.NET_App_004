using System.Text.Json;
using System.Text.Json.Serialization;

namespace ASP.NET_App_004.Models;

public class Product
{
    public string Id { get; set; } = string.Empty;
    public string Maker { get; set; } = string.Empty;

    [JsonPropertyName("img")] // Recommended in the tutorial to easily map images from the .json file to your webpage.
    public string Image { get; set; } = string.Empty;

    public string Url { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public int[] Ratings { get; set; } = Array.Empty<int>();

    public override string ToString() => JsonSerializer.Serialize<Product>(this);
}
