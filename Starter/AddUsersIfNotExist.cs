using Data.Entities;
using Domain.Enums;
using Infrastructure.DbContexts;
using Microsoft.AspNetCore.Identity;
using Starter.Models;
using Microsoft.EntityFrameworkCore;

namespace Starter
{
    public partial class SeedFromApi
    {
        public static async Task AddUsersIfNotExistAsync(List<UserFromAPI> users, AppDbContext dbContext, IPasswordHasher<User> passwordHasher)
        {
            var existingUsernames = await dbContext.Users
                .Select(u => u.Username.ToLower())
                .ToListAsync();

            var existingEmails = await dbContext.Users
                .Select(u => u.Email.ToLower())
                .ToListAsync();

            var usernameSet = existingUsernames.ToHashSet();
            var emailSet    = existingEmails.ToHashSet();

            foreach (var user in users)
            {
                var username = (user.Username ?? "").Trim().ToLowerInvariant();
                var email    = (user.Email ?? "").Trim().ToLowerInvariant();

                if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(email))
                    continue;

                if (usernameSet.Contains(username) || emailSet.Contains(email))
                    continue;

                var newUser = new User
                {
                    Username  = username,
                    Email     = email,
                    CreatedAt = DateTime.UtcNow,
                    Role = user.Id == 1 ? Role.ADMIN
                        : user.Id == 2 ? Role.EMPLOYEE
                        : Role.SHOPPER,
                    IsActive = true
                };

                var rawPassword = string.IsNullOrWhiteSpace(user.Password)
                    ? "DefaultPassword123!"
                    : user.Password;

                newUser.PasswordHash = passwordHasher.HashPassword(newUser, rawPassword);

                dbContext.Users.Add(newUser);

                usernameSet.Add(username);
                emailSet.Add(email);
            }

            await dbContext.SaveChangesAsync();
        }
    }
}