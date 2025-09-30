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
        [Required(ErrorMessage = "The name is required.")]
        [StringLength(120, ErrorMessage = "The name must be a maximum length of 120 characters.")]
        public string Name { get; set; } = null!;
    }
}
