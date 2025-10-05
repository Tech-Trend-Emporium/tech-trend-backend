using Application.Dtos.Inventory;
using Data.Entities;
using General.Dto.Inventory;
using General.Dto.Product;
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

        public static InventoryResponse ToResponse(Inventory entity, string productName)
        {
            if (entity is null) throw new ArgumentNullException(nameof(entity));

            return new InventoryResponse
            {
                Id = entity.Id,
                Total = entity.Total,
                Available = entity.Available,
                Product = productName
            };
        }

        public static List<InventoryResponse> ToResponseList(IReadOnlyList<Inventory> entities, List<string> productNames)
        {
            if (entities == null) throw new ArgumentNullException(nameof(entities));
            if (productNames == null) throw new ArgumentNullException(nameof(productNames));

            return entities.Select((entity, index) => ToResponse(entity, productNames[index])).ToList();
        }
    }
}
