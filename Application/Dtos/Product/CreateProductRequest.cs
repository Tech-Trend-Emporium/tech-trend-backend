using General.Dto.Inventory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Data.Entities;

namespace General.Dto.Product
{
    public class CreateProductRequest
    {
        public string Title { get; set; } = null!;
        public decimal Price { get; set; }
        public string Description { get; set; } = null!;
        public string Category { get; set; } = null!;
        public string Image { get; set; } = null!;
        public double RatingRate { get; set; }
        public int Count { get; set; }
        public CreateInventoryRequest Inventory { get; set; } = null!;
    }
}
