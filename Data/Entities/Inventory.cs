using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Entities
{
    [Index(nameof(ProductId), IsUnique = true)]
    public class Inventory : IValidatableObject
    {
        public int Id { get; set; }

        [Range(0, int.MaxValue)]
        public int Total { get; set; }

        [Range(0, int.MaxValue)]
        public int Available { get; set; }

        [Required, ForeignKey(nameof(Product))]
        public int ProductId { get; set; }
        public Product Product { get; set; } = null!;

        public IEnumerable<ValidationResult> Validate(ValidationContext _)
        {
            if (Available > Total)
                yield return new ValidationResult("Available no puede ser mayor que Total", new[] { nameof(Available), nameof(Total) });
        }
    }
}
