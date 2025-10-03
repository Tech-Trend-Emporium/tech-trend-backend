using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Dtos.Product
{
    public class CreateProductResponse
    {
        public int productId { get; set; }
        public string Message { get; set; } = null!;
    }
}
