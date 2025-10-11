using Data.Entities;
using General.Dto.Product;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace General.Mappers
{
    public static class ProductMapper
    {
        public static Product ToEntity(CreateProductRequest dto, int categoryId)
        {
            if (dto is null) throw new ArgumentNullException(nameof(dto));

            return new Product
            {
                Title = dto.Title,
                Price = dto.Price,
                Description = dto.Description,
                ImageUrl = dto.ImageUrl?.Trim(),
                RatingRate = dto.RatingRate,
                Count = dto.Count,
                CategoryId = categoryId
            };
        }

        public static void ApplyUpdate(Product entity, UpdateProductRequest dto, int? categoryId)
        {
            if (entity == null) throw new ArgumentNullException(nameof(entity));
            if (dto == null) throw new ArgumentNullException(nameof(dto));

            if (!string.IsNullOrWhiteSpace(dto.Title)) entity.Title = dto.Title;
            if (dto.Price.HasValue) entity.Price = dto.Price.Value;
            if (!string.IsNullOrWhiteSpace(dto.Description)) entity.Description = dto.Description;
            if (!string.IsNullOrWhiteSpace(dto.ImageUrl)) entity.ImageUrl = dto.ImageUrl.Trim();
            if (dto.RatingRate.HasValue) entity.RatingRate = dto.RatingRate.Value;
            if (dto.Count.HasValue) entity.Count = dto.Count.Value;
            if (categoryId.HasValue) entity.CategoryId = categoryId.Value;

            entity.UpdatedAt = DateTime.UtcNow;
        }

        public static ProductResponse ToResponse(Product entity, string categoryName)
        {
            if (entity is null) throw new ArgumentNullException(nameof(entity));

            return new ProductResponse
            {
                Id = entity.Id,
                Title = entity.Title,
                Price = entity.Price,
                Description = entity.Description ?? string.Empty,
                ImageUrl = entity.ImageUrl ?? string.Empty,
                RatingRate = entity.RatingRate,
                Count = entity.Count,
                Category = categoryName ?? entity?.Category?.Name ?? "Unknown",
                CreatedAt = entity.CreatedAt,
                UpdatedAt = entity.UpdatedAt
            };
        }

        public static List<ProductResponse> ToResponseList(IReadOnlyList<Product> entities, IReadOnlyDictionary<int, string> categoryNamesById)
        {
            if (entities == null) throw new ArgumentNullException(nameof(entities));
            if (categoryNamesById == null) throw new ArgumentNullException(nameof(categoryNamesById));

            var list = new List<ProductResponse>(entities.Count);
            foreach (var e in entities)
            {
                categoryNamesById.TryGetValue(e.CategoryId, out var name);
                list.Add(ToResponse(e, name));
            }

            return list;
        }
    }
}
