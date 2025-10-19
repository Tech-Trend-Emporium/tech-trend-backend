using Data.Entities;
using General.Dto.User;

namespace General.Mappers
{
    public static class UserMapper
    {
        public static User ToEntity(CreateUserRequest dto)
        {
            if (dto is null) throw new ArgumentNullException(nameof(dto));

            return new User
            {
                Username = dto.Username.Trim(),
                Email = dto.Email.Trim(),
                Role = dto.Role,
                IsActive = dto.IsActive,
                CreatedAt = DateTime.UtcNow,
                SecurityStamp = Guid.NewGuid().ToString("N")
            };
        }

        public static void ApplyUpdate(User entity, UpdateUserRequest dto)
        {
            if (entity is null) throw new ArgumentNullException(nameof(entity));
            if (dto is null) throw new ArgumentNullException(nameof(dto));

            if (!string.IsNullOrWhiteSpace(dto.Username)) entity.Username = dto.Username.Trim();
            if (!string.IsNullOrWhiteSpace(dto.Email)) entity.Email = dto.Email.Trim();
            if (dto.Role.HasValue) entity.Role = dto.Role.Value;
            if (dto.IsActive.HasValue) entity.IsActive = dto.IsActive.Value;

            entity.UpdatedAt = DateTime.UtcNow;
        }

        public static UserResponse ToResponse(User entity)
        {
            if (entity is null) throw new ArgumentNullException(nameof(entity));

            return new UserResponse
            {
                Id = entity.Id,
                Email = entity.Email,
                Username = entity.Username,
                Role = entity.Role.ToString(),
                IsActive = entity.IsActive,
                CreatedAt = entity.CreatedAt,
                UpdatedAt = entity.UpdatedAt
            };
        }

        public static List<UserResponse> ToResponseList(IEnumerable<User> entities)
        {
            if (entities is null) throw new ArgumentNullException(nameof(entities));

            return entities.Select(ToResponse).ToList();
        }
    }
}
