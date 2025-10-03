using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace General.Dto.Product
{
    public class CreateProductRequest
    {
        public string title { get; set; } = null!;
        public decimal price { get; set; }
        public string description { get; set; } = null!;
        public string category { get; set; } = null!;
        public string image { get; set; } = null!;
        public Rating rating { get; set; } = new Rating();
        public Inventory inventory { get; set; } = new Inventory();

        public class Rating
        {
            public float rate { get; set; } = 0;
            public double count { get; set; } = 0;
        }
        public class Inventory
        {
            public int total { get; set; }
            public int available { get; set; }
        }
    }
}
