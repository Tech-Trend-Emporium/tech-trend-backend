using Domain.Validations;
using System.ComponentModel.DataAnnotations;

namespace General.Dto.Category
{
    public class CreateCategoryRequest
    {
        [Required(ErrorMessage = CategoryValidator.NameRequiredMessage)]
        [StringLength(CategoryValidator.NameMaxLength, MinimumLength = CategoryValidator.NameMinLength, ErrorMessage = CategoryValidator.NameLengthMessage)]
        [RegularExpression(CategoryValidator.NameRegex, ErrorMessage = CategoryValidator.NameRegexMessage)]
        public string Name { get; set; } = null!;
    }
}
