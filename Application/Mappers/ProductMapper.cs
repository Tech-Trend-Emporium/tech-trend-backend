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

            var product = new Product
            {
                Title = dto.Title,
                Price = dto.Price,
                Description = dto.Description,
                ImageUrl = dto.Image,
                RatingRate = dto.RatingRate,
                Count = dto.Count,
                CategoryId = categoryId
            };

            return product;
        }

        public static Product ApplyUpdate(UpdateProductRequest dto, int categoryId)
        {
            var product = new Product
            {
                Id = dto.Id,
                Title = dto.Title,
                Price = dto.Price,
                Description = dto.Description,
                ImageUrl = dto.Image,
                RatingRate = dto.RatingInfo.Rate,
                Count = (int)dto.RatingInfo.Rate,
                CategoryId = categoryId
            };

            return product;
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
