using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using Domain;
using Infrastructure.DbContexts;
using Data.Entities;
using Microsoft.EntityFrameworkCore;
using Starter.Models;

namespace Starter
{
    public class SeedFromApi
    {
        public static async Task<List<ProductFromApi>> FetchProductsAsync()
        {
            // 1. Send GET request
            var httpClient = new HttpClient();
            var response = await httpClient.GetStringAsync("https://fakestoreapi.com/products");

            // 2. Deserialize JSON to List<Product>
            var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
            List<ProductFromApi>? products = JsonSerializer.Deserialize<List<ProductFromApi>>(response, options);

            if (products is null) Console.WriteLine("No data retrieved from API.");
            
            return products;
        }

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
                    ImageUrl = product.Image
                };

                dbContext.Products.Add(newProduct);
            }

            await dbContext.SaveChangesAsync();
        }
    }
}
