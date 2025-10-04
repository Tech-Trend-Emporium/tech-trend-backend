using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Data.Entities;

namespace General.Dto.Product
{
    public class ProductResponse
    {
        public int Id { get; set; }
        public string title { get; set; } = null!;
        public string category { get; set; } = null!;
        public decimal price { get; set; }
    }

    
}
