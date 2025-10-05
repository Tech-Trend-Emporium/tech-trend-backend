using Application.Dtos.Inventory;
using Data.Entities;
using General.Dto.Inventory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace General.Mappers
{
    public static class InventoryMapper
    {
        public static Inventory ToEntity(CreateInventoryInlineRequest dto)
        {
            if (dto is null) throw new ArgumentNullException(nameof(dto));

            return new Inventory
            {
                Total = dto.Total,
                Available = dto.Available,
            };
        }
    }
}
