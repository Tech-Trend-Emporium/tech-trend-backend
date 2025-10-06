using Application.Dtos.Inventory;
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
    public class CreateProductRequest : IValidatableObject
    {
        [Required(ErrorMessage = ProductValidator.TitleRequiredMessage)]
        [StringLength(200, ErrorMessage = ProductValidator.TitleMaxLengthMessage)]
        public string Title { get; set; } = null!;

        [Required(ErrorMessage = ProductValidator.PriceRequiredMessage)]
        [Range(0, 999999999999.99, ErrorMessage = ProductValidator.PriceRangeMessage)]
        public decimal Price { get; set; }

        [StringLength(2000, ErrorMessage = ProductValidator.DescriptionMaxLengthMessage)]
        public string? Description { get; set; }

        [Url(ErrorMessage = ProductValidator.ImageUrlInvalidMessage)]
        [StringLength(2048, ErrorMessage = ProductValidator.ImageUrlMaxLengthMessage)]
        public string? ImageUrl { get; set; }

        [Range(0, 5, ErrorMessage = ProductValidator.RatingRangeMessage)]
        public double RatingRate { get; set; } = 0;

        [Range(0, int.MaxValue, ErrorMessage = ProductValidator.CountNonNegativeMessage)]
        public int Count { get; set; } = 0;

        [Required(ErrorMessage = ProductValidator.CategoryRequiredMessage)]
        [StringLength(ProductValidator.CategoryMaxLength, MinimumLength = ProductValidator.CategoryMinLength, ErrorMessage = ProductValidator.CategoryLengthMessage)]
        [RegularExpression(ProductValidator.CategoryRegex, ErrorMessage = ProductValidator.CategoryRegexMessage)]
        public string Category { get; set; }

        public CreateInventoryInlineRequest? Inventory { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext _)
        {
            if (Title != null && Title != Title.Trim()) yield return new ValidationResult(ProductValidator.TitleNoLeadingTrailingSpacesMessage, new[] { nameof(Title) });
            if (Description != null && Description != Description.Trim()) yield return new ValidationResult(ProductValidator.DescriptionNoLeadingTrailingSpacesMessage, new[] { nameof(Description) });
            if (ImageUrl != null && ImageUrl != ImageUrl.Trim()) yield return new ValidationResult(ProductValidator.ImageUrlNoLeadingTrailingSpacesMessage, new[] { nameof(ImageUrl) });
            if (Count < 0) yield return new ValidationResult(ProductValidator.CountNegativeErrorMessage, new[] { nameof(Count) });
        }
    }
}
