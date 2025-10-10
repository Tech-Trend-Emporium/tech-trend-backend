using Starter.Models;
using System.Net.Http;
using System.Text.Json;


namespace Starter
{
    public partial class SeedFromApi
    {
        public static async Task<List<UserFromAPI>> FetchUsersAsync(CancellationToken ct = default)
        {
            using var http = CreateHttpClient();
            var request = new HttpRequestMessage(HttpMethod.Get, "https://fakestoreapi.com/users?limit")
            {
                Version = System.Net.HttpVersion.Version11,
                VersionPolicy = HttpVersionPolicy.RequestVersionOrLower
            };
            using var response = await http.SendAsync(request, HttpCompletionOption.ResponseHeadersRead, ct);
            var body = await response.Content.ReadAsStringAsync(ct);

            if (!response.IsSuccessStatusCode)
            {
                Console.WriteLine($"[Seed] Error fetching users: {response.StatusCode}");
                return new();
            }
            var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
            return JsonSerializer.Deserialize<List<UserFromAPI>>(body, options) ?? new();
        }
    }
}
