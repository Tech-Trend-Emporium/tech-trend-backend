using Domain.Validations;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
