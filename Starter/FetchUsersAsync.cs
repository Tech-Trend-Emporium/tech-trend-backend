using Starter.Models;
using System.Text.Json;

namespace Starter
{
    public partial class SeedFromApi
    {
        public static async Task<List<UserFromAPI>> FetchUsersAsync(CancellationToken ct = default)
{
            using var http = CreateHttpClient();
            var request = new HttpRequestMessage(HttpMethod.Get, "https://fakestoreapi.com/users")
            {
                Version = System.Net.HttpVersion.Version11,
                VersionPolicy = HttpVersionPolicy.RequestVersionOrLower
            };

            request.Headers.Accept.ParseAdd("application/json");
            request.Headers.UserAgent.ParseAdd("Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/120 Safari/537.36");

            using var response = await http.SendAsync(request, HttpCompletionOption.ResponseHeadersRead, ct);
            var body = await response.Content.ReadAsStringAsync(ct);

            if (!response.IsSuccessStatusCode)
            {
                Console.WriteLine($"[Seed] Error fetching users: {(int)response.StatusCode} {response.StatusCode}");
                Console.WriteLine($"[Seed] Response body (first 500): {body[..Math.Min(body.Length, 500)]}");
                return new();
            }

            var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
            return JsonSerializer.Deserialize<List<UserFromAPI>>(body, options) ?? new();
        }
    }
}
