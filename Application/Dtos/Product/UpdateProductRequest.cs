using Data.Entities;
using Domain.Validations;
using General.Dto.Inventory;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace General.Dto.Product
{
    public class UpdateProductRequest
    {
        [StringLength(200, ErrorMessage = ProductValidator.TitleMaxLengthMessage)]
        public string? Title { get; set; }

        [Range(0, 999999999999.99, ErrorMessage = ProductValidator.PriceRangeMessage)]
        public decimal? Price { get; set; }

        [StringLength(2000, ErrorMessage = ProductValidator.DescriptionMaxLengthMessage)]
        public string? Description { get; set; }

        [Url(ErrorMessage = ProductValidator.ImageUrlInvalidMessage)]
        [StringLength(2048, ErrorMessage = ProductValidator.ImageUrlMaxLengthMessage)]
        public string? ImageUrl { get; set; }

        [Range(0, 5, ErrorMessage = ProductValidator.RatingRangeMessage)]
        public double? RatingRate { get; set; }

        [Range(0, int.MaxValue, ErrorMessage = ProductValidator.CountNonNegativeMessage)]
        public int? Count { get; set; }

        [StringLength(ProductValidator.CategoryMaxLength, MinimumLength = ProductValidator.CategoryMinLength, ErrorMessage = ProductValidator.CategoryLengthMessage)]
        [RegularExpression(ProductValidator.CategoryRegex, ErrorMessage = ProductValidator.CategoryRegexMessage)]
        public string? Category { get; set; }

        public UpdateInventoryRequest? Inventory { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext _)
        {
            if (Title != null && Title != Title.Trim()) yield return new ValidationResult(ProductValidator.TitleNoLeadingTrailingSpacesMessage, new[] { nameof(Title) });
            if (Description != null && Description != Description.Trim()) yield return new ValidationResult(ProductValidator.DescriptionNoLeadingTrailingSpacesMessage, new[] { nameof(Description) });
            if (ImageUrl != null && ImageUrl != ImageUrl.Trim()) yield return new ValidationResult(ProductValidator.ImageUrlNoLeadingTrailingSpacesMessage, new[] { nameof(ImageUrl) });
            if (Category != null && Category != Category.Trim()) yield return new ValidationResult(ProductValidator.CategoryNoLeadingTrailingSpacesMessage, new[] { nameof(Category) });
            
            if (Inventory?.Total is int t && Inventory?.Available is int a && a > t)
            {
                yield return new ValidationResult(
                    InventoryValidator.AvailableGreaterThanTotalErrorMessage,
                    new[]
                    {
                        $"Inventory.{nameof(UpdateInventoryRequest.Available)}",
                        $"Inventory.{nameof(UpdateInventoryRequest.Total)}"
                    });
            }
        }
    }
}
