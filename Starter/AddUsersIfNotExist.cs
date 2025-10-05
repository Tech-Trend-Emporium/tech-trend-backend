using Infrastructure.DbContexts;
using Starter.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Domain.Enums;
using System.Threading.Tasks;

namespace Starter
{
    public partial class SeedFromApi{
        public static async Task AddUsersIfNotExistAsync(List<UserFromAPI> users, AppDbContext dbContext)
        {
            var existingUsernames = dbContext.Users.Select(u => u.Username).ToHashSet(StringComparer.OrdinalIgnoreCase);
            var existingEmails = dbContext.Users.Select(u => u.Email).ToHashSet(StringComparer.OrdinalIgnoreCase);
            foreach (var user in users)
            {
                if (existingUsernames.Contains(user.Username) || existingEmails.Contains(user.Email))
                {
                    continue; // Skip if username or email already exists
                }
                var newUser = new Data.Entities.User
                {
                    Id = user.Id,
                    Username = user.Username,
                    Email = user.Email,
                    PasswordHash = Convert.ToBase64String(Encoding.UTF8.GetBytes(user.Password ?? "DefaultPassword123!")), // Replace with proper hashing
                    CreatedAt = DateTime.UtcNow,
                    Role =  user.Id == 1 ? Role.ADMIN : Role.SHOPPER, // First user as ADMIN, others as SHOPPER

                };
                dbContext.Users.Add(newUser);
            }
            await dbContext.SaveChangesAsync();
        } 
    }
}
