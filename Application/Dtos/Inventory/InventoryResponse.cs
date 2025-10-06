using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace General.Dto.Inventory
{
    public class InventoryResponse
    {
        public int Id { get; set; }
        public int Total { get; set; }
        public int Available { get; set; }
        public int ProductId { get; set; }
    }
}
