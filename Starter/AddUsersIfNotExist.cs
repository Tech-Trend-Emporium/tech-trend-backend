using Data.Entities;
using Domain.Enums;
using Infrastructure.DbContexts;
using Microsoft.AspNetCore.Identity;
using Starter.Models;

namespace Starter
{
    public partial class SeedFromApi{
        public static async Task AddUsersIfNotExistAsync(List<UserFromAPI> users, AppDbContext dbContext, IPasswordHasher<User> passwordHasher)
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
                    Username = user.Username,
                    Email = user.Email,
                    CreatedAt = DateTime.UtcNow,
                    Role = user.Id == 1 ? Role.ADMIN
                        : user.Id == 2 ? Role.EMPLOYEE
                        : Role.SHOPPER,
                    IsActive = true
                };
                var rawPassword = user.Password ?? "DefaultPassword123!";
                newUser.PasswordHash = passwordHasher.HashPassword(newUser, rawPassword);

                dbContext.Users.Add(newUser);
            }
            await dbContext.SaveChangesAsync();
        } 
    }
}
