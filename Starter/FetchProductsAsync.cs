using System.Text.Json;
using Starter.Models;

namespace Starter
{
    public partial class SeedFromApi
    {
        public static async Task<List<ProductFromApi>> FetchProductsAsync()
        {
            // 1. Send GET request
            var httpClient = new HttpClient();
            var response = await httpClient.GetStringAsync("https://fakestoreapi.com/products");

            // 2. Deserialize JSON to List<Product>
            var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
            List<ProductFromApi>? products = JsonSerializer.Deserialize<List<ProductFromApi>>(response, options);

            if (products is null) Console.WriteLine("No data retrieved from API.");
            
            return products;
        }
    }
}