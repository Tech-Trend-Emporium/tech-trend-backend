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
        public static async Task AddProductsIfNotExistAsync(List<ProductFromApi> products, AppDbContext dbContext)
        {
            // Get all existing product titles for quick lookup
            var existingTitles = await dbContext.Products.Select(p => p.Title).ToListAsync();

            // Get all categories and their IDs
            var categories = await dbContext.Categories.ToDictionaryAsync(c => c.Name, c => c.Id, StringComparer.OrdinalIgnoreCase);

            foreach (var product in products)
            {
                // Skip if product already exists by title
                if (existingTitles.Contains(product.Title, StringComparer.OrdinalIgnoreCase)) continue;

                // Find category ID or handle missing category as needed
                if (!categories.TryGetValue(product.Category, out int categoryId)) continue;

                // Create new Product entity
                var newProduct = new Product
                {
                    Title = product.Title,
                    Price = product.Price,
                    Description = product.Description,
                    RatingRate = product.Rating.Rate,
                    Count = product.Rating.Count,
                    CategoryId = categoryId,
                    CreatedAt = DateTime.UtcNow,
                    ImageUrl = product.Image,
                    Inventory = new Inventory
                    {
                        Total = 100,
                        Available = 100
                    }
                };

                dbContext.Products.Add(newProduct);
            }

            await dbContext.SaveChangesAsync();
        }
    }
}