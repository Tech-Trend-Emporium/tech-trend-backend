using Data.Entities;
using General.Dto.Coupon;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace General.Mappers
{
    public static class CouponMapper
    {
        private static DateTime ParseDateUtc(string s)
        {
            return DateTime.ParseExact(
                s.Trim(),   
                "yyyy-MM-dd",
                CultureInfo.InvariantCulture,
                DateTimeStyles.AssumeUniversal | DateTimeStyles.AdjustToUniversal
            );
        }

        private static DateTime? ParseNullableDateUtc(string? s)
        {
            return string.IsNullOrWhiteSpace(s) ? (DateTime?)null : ParseDateUtc(s);
        }

        public static Coupon ToEntity(CreateCouponRequest dto, string generatedCode)
        {
            if (dto is null) throw new ArgumentNullException(nameof(dto));
            if (string.IsNullOrWhiteSpace(generatedCode)) throw new ArgumentNullException(nameof(generatedCode));

            return new Coupon
            {
                Code = generatedCode.Trim().ToUpperInvariant(),
                Discount = dto.Discount,
                Active = dto.Active,
                ValidFrom = ParseDateUtc(dto.ValidFrom),
                ValidTo = ParseNullableDateUtc(dto.ValidTo)
            };
        }

        public static void ApplyUpdate(Coupon entity, UpdateCouponRequest dto)
        {
            if (entity is null) throw new ArgumentNullException(nameof(entity));
            if (dto is null) throw new ArgumentNullException(nameof(dto));

            if (dto.Discount.HasValue) entity.Discount = dto.Discount.Value;
            if (dto.Active.HasValue) entity.Active = dto.Active.Value;
            if (!string.IsNullOrWhiteSpace(dto.ValidFrom)) entity.ValidFrom = ParseDateUtc(dto.ValidFrom);
            if (!string.IsNullOrWhiteSpace(dto.ValidTo)) entity.ValidTo = ParseDateUtc(dto.ValidTo);
        }

        public static CouponResponse ToResponse(Coupon entity)
        {
            if (entity is null) throw new ArgumentNullException(nameof(entity));

            return new CouponResponse
            {
                Id = entity.Id,
                Code = entity.Code,
                Discount = entity.Discount,
                Active = entity.Active,
                ValidFrom = entity.ValidFrom,
                ValidTo = entity.ValidTo
            };
        }

        public static List<CouponResponse> ToResponseList(IEnumerable<Coupon> entities)
        {
            if (entities is null) throw new ArgumentNullException(nameof(entities));

            return entities.Select(ToResponse).ToList();
        }
    }
}
