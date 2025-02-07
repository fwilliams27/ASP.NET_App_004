using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc; 
using System.Text.Json;
using ASP.NET_App_004.Models;
using ASP.NET_App_004.Services;

namespace ASP.NET_App_004.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class ProductsController : ControllerBase
    {
        public JsonFileProductService _productService;
        public IEnumerable<Product> Products { get; private set; }

        public ProductsController(JsonFileProductService productService)
        {
            _productService = productService;
            Products = new List<Product>(); // Initialize with an empty list or fetch products from the service
        }

        [HttpGet]
        public IActionResult Get()
        {
            // Fetch products using the JsonFileProductService
            Products = _productService.GetProducts();
            
            // Pass products to
            return Ok(Products);
        }

        [HttpGet("Rate")]
        public IActionResult Get([FromQuery] string id, [FromQuery] int rating)
        {
            // Add rating using the JsonFileProductService
            _productService.AddRating(id, rating);
            
            return Ok();
        }
    } 
} 