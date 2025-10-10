using System.Text.Json;
using System.Net.Http;
using Starter.Models;
using System.Net;

namespace Starter
{
    public partial class SeedFromApi
    {
        public static async Task<List<ProductFromApi>> FetchProductsAsync(CancellationToken ct = default)
        {
            using var http = CreateHttpClient();

            var request = new HttpRequestMessage(HttpMethod.Get, "https://fakestoreapi.com/products?limit=40")
            {
                Version = HttpVersion.Version11,
                VersionPolicy = HttpVersionPolicy.RequestVersionOrLower
            };

            using var response = await http.SendAsync(request, HttpCompletionOption.ResponseHeadersRead, ct);
            var body = await response.Content.ReadAsStringAsync(ct);

            if (!response.IsSuccessStatusCode)
            {
                Console.WriteLine($"[Seed] Error fetching products: {response.StatusCode}");
                return new();
            }
            
            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };

            return JsonSerializer.Deserialize<List<ProductFromApi>>(body, options) ?? new();
        }
    }
}