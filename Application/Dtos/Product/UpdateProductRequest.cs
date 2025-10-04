using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Data.Entities;

namespace General.Dto.Product
{
    public class UpdateProductRequest
    {
        public int Id { get; set; }
        public string Title { get; set; } = null!;
        public decimal Price { get; set; }
        public string Description { get; set; } = null!;
        public string Category { get; set; } = null!;
        public string Image { get; set; } = null!;
        public UpdateRating RatingInfo { get; set; } = new UpdateRating();
        public UpdateInventory InventoryInfo { get; set; } = new UpdateInventory();

        public class UpdateRating
        {
            public double Rate { get; set; }
            public double Count { get; set; }
        }

        public class UpdateInventory
        {
            public int Total { get; set; }
            public int Available { get; set; }
        }
    }
}
