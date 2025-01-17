using System.Text.Json;
using System.Text.Json.Serialization;

namespace ASP.NET_App_004.Models; //This code was added due to simplicity of calling the large product.js file//

public class Product
{
    public string Id { get; set; }
    public string Maker { get; set; }
    
    [JsonPropertyName("img")] // Recommended in the tutorial to easily map thev images from the .json file to your webpage.
    public string Image { get; set; }
    public string Url { get; set; }
    public string Title { get; set; }
    public string Description { get; set; }
    public int[] Ratings { get; set; }
    
    public override string ToString() => JsonSerializer.Serialize<Product>(this);
    
}