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
        [StringLength(120, ErrorMessage = "The name must be at most 120 characters long.")]
        public string Name { get; set; } = null!;
    }
}
