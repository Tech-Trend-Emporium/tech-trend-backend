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
                Description = entity.Description,
                ImageUrl = entity.ImageUrl,
                RatingRate = entity.RatingRate,
                Count = entity.Count,
                Category = categoryName,
                CreatedAt = entity.CreatedAt,
                UpdatedAt = entity.UpdatedAt
            };
        }

        public static List<ProductResponse> ToResponseList(IReadOnlyList<Product> entities, List<string> categoryNames)
        {
            if (entities == null) throw new ArgumentNullException(nameof(entities));
            if (categoryNames == null) throw new ArgumentNullException(nameof(categoryNames));

            return entities.Select((entity, index) => ToResponse(entity, categoryNames[index])).ToList();
        }
    }
}
