using Starter.Models;
using System.Text.Json;


namespace Starter
{
    public partial class SeedFromApi
    {
        public static async Task<List<UserFromAPI>> FetchUsersAsync()
        {
            var httpClient = new HttpClient();
            var response = await httpClient.GetStringAsync("https://fakestoreapi.com/users");

            var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
            List<UserFromAPI>? users = JsonSerializer.Deserialize<List<UserFromAPI>>(response, options);

            if (users is null) Console.WriteLine("No data retrieved from API.");

            return users;
        }
    }
}
