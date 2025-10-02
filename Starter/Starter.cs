using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using Domain;
using Infrastructure.DbContexts;
using Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace Starter
{
    public class SeedFromApi
    {
        public class ProductFromApi
        {
            public string Title { get; set; }
            public decimal Price { get; set; }
            public string Description { get; set; }
            public string Category { get; set; }
            public string Image { get; set; }
        }
        public static async Task<List<ProductFromApi>> FetchProductsAsync()
        {
            var httpClient = new HttpClient();
            // 1. Send GET request
            var response = await httpClient.GetStringAsync("https://fakestoreapi.com/products");

            // 2. Deserialize JSON to List<Product>
            var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
            List<ProductFromApi>? products = JsonSerializer.Deserialize<List<ProductFromApi>>(response, options);

            if (products is null)
            {
                Console.WriteLine("No data retrieved from API.");
            }
            
            return products;
            
        }

        public static async Task AddCategoriesIfNotExistAsync(List<ProductFromApi> products, AppDbContext dbContext)
        {
            var existingCategories = await dbContext.Categories
                .Select(c => c.Name)
                .ToListAsync();

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

            if (newCategories.Count > 0)
            {
                await dbContext.SaveChangesAsync();
            }
        }

        public static async Task AddProductsIfNotExistAsync(List<ProductFromApi> products, AppDbContext dbContext)
        {
            // Get all existing product titles for quick lookup
            var existingTitles = await dbContext.Products
                .Select(p => p.Title)
                .ToListAsync();

            // Get all categories and their IDs
            var categories = await dbContext.Categories
                .ToDictionaryAsync(c => c.Name, c => c.Id, StringComparer.OrdinalIgnoreCase);

            foreach (var product in products)
            {
                // Skip if product already exists by title
                if (existingTitles.Contains(product.Title, StringComparer.OrdinalIgnoreCase))
                    continue;

                // Find category ID
                if (!categories.TryGetValue(product.Category, out int categoryId))
                    continue; // Or handle missing category as needed

                // Create new Product entity
                var newProduct = new Product
                {
                    Title = product.Title,
                    Price = product.Price,
                    Description = product.Description,
                    CategoryId = categoryId,
                    // Set other fields as needed, e.g., CreatedAt
                    CreatedAt = DateTime.UtcNow,
                    ImageUrl = product.Image

                };

                dbContext.Products.Add(newProduct);
            }

            await dbContext.SaveChangesAsync();
        }
    }
}
