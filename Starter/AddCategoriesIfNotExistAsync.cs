using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Infrastructure.DbContexts;
using Data.Entities;
using Microsoft.EntityFrameworkCore;
using Starter.Models;

namespace Starter
{
    public partial class SeedFromApi
    {
        public static async Task AddCategoriesIfNotExistAsync(List<ProductFromApi> products, AppDbContext dbContext)
        {
            var existingCategories = await dbContext.Categories.Select(c => c.Name).ToListAsync();
            var newCategories = products
                .Select(p => p.Category)
                .Where(cat => !string.IsNullOrWhiteSpace(cat))
                .Distinct()
                .Except(existingCategories, StringComparer.OrdinalIgnoreCase)
                .ToList();
            foreach (var categoryName in newCategories)
            {
                dbContext.Categories.Add(new Category
                {
                    Name = categoryName,
                    CreatedAt = DateTime.UtcNow
                });
            }

            if (newCategories.Count > 0) await dbContext.SaveChangesAsync();
        }
    }
}