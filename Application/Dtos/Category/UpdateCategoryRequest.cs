using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace General.Dto.Category
{
    public class UpdateCategoryRequest
    {
        [Required(ErrorMessage = "The name is required.")]
        [StringLength(120, MinimumLength = 3, ErrorMessage = "The name must be between 3 and 120 characters.")]
        [RegularExpression(@"^[a-zA-Z0-9\s\-\.\,&]+$", ErrorMessage = "The name can only contain letters, numbers, spaces, and the following characters: - . , &")]
        public string Name { get; set; } = null!;
    }
}
