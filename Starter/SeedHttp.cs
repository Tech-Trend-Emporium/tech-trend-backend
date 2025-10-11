using System.Net;

namespace Starter
{
    public partial class SeedFromApi
    {
        private static HttpClient CreateHttpClient()
        {
            var handler = new SocketsHttpHandler
            {
                AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate,
                AllowAutoRedirect = true,
                PooledConnectionLifetime = TimeSpan.FromMinutes(2),
            };

            var http = new HttpClient(handler)
            {
                Timeout = TimeSpan.FromSeconds(30)
            };

            // Look like a normal app client
            http.DefaultRequestHeaders.UserAgent.ParseAdd("TechTrendEmporium/1.0 (+seed)");
            http.DefaultRequestHeaders.Accept.ParseAdd("application/json");
            http.DefaultRequestHeaders.AcceptEncoding.ParseAdd("gzip");

            return http;
        }

        private static string Short(string? s, int max = 500) 
            => string.IsNullOrEmpty(s) ? "" : (s.Length <= max ? s : s[..max] + "...");
    }
        
}
