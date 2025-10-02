using Data.Entities;
using General.Dto.Category;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace General.Mappers
{
    public static class CategoryMapper
    {
        public static Category ToEntity(CreateCategoryRequest dto)
        {
            if (dto is null) throw new ArgumentNullException(nameof(dto));

            return new Category
            {
                Name = dto.Name.Trim()
            };
        }

        public static void ApplyUpdate(Category entity, UpdateCategoryRequest dto)
        {
            if (entity is null) throw new ArgumentNullException(nameof(entity));
            if (dto is null) throw new ArgumentNullException(nameof(dto));

            entity.Name = dto.Name.Trim();
            entity.UpdatedAt = DateTime.UtcNow;
        }

        public static CategoryResponse ToResponse(Category entity)
        {
            if (entity is null) throw new ArgumentNullException(nameof(entity));

            return new CategoryResponse
            {
                Id = entity.Id,
                Name = entity.Name,
                CreatedAt = entity.CreatedAt,
                UpdatedAt = entity.UpdatedAt
            };
        }

        public static List<CategoryResponse> ToResponseList(IEnumerable<Category> entities)
        {
            if (entities is null) throw new ArgumentNullException(nameof(entities));

            return entities.Select(ToResponse).ToList();
        }
    }
}
