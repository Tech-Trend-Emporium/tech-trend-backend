using Starter.Models;

namespace Starter
{
    public partial class SeedFromApi
    {
        private static readonly List<UserFromAPI> BuiltInUsers = new()
        {
            new() { Id = 1,  Email = "john@gmail.com",     Username = "johnd",    Password = "m38rmF$"      },
            new() { Id = 2,  Email = "morrison@gmail.com", Username = "mor_2314", Password = "83r5^_"       },
            new() { Id = 3,  Email = "kevin@gmail.com",    Username = "kevinryan",Password = "kev02937@"    },
            new() { Id = 4,  Email = "don@gmail.com",      Username = "donero",   Password = "ewedon"       },
            new() { Id = 5,  Email = "derek@gmail.com",    Username = "derek",    Password = "jklg*_56"     },
            new() { Id = 6,  Email = "david_r@gmail.com",  Username = "david_r",  Password = "3478*#54"     },
            new() { Id = 7,  Email = "miriam@gmail.com",   Username = "snyder",   Password = "f238&@*$"     },
            new() { Id = 8,  Email = "william@gmail.com",  Username = "hopkins",  Password = "William56$hj" },
            new() { Id = 9,  Email = "kate@gmail.com",     Username = "kate_h",   Password = "kfejk@*_"     },
            new() { Id = 10, Email = "jimmie@gmail.com",   Username = "jimmie_k", Password = "klein*#%*"    },
        };

        public static Task<List<UserFromAPI>> FetchUsersAsync(CancellationToken ct = default)
        {
            var normalized = BuiltInUsers
                .Select(u => new UserFromAPI
                {
                    Id       = u.Id,
                    Email    = u.Email?.Trim().ToLowerInvariant() ?? "",
                    Username = u.Username?.Trim().ToLowerInvariant() ?? "",
                    Password = u.Password ?? "DefaultPassword123!"
                })
                .ToList();

            Console.WriteLine($"[Seed] Using built-in users: {normalized.Count}");
            return Task.FromResult(normalized);
        }
    }
}