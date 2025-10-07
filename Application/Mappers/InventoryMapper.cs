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

        public static InventoryResponse ToResponse(Inventory entity, string? productName = null)
        {
            if (entity is null) throw new ArgumentNullException(nameof(entity));

            return new InventoryResponse
            {
                Id = entity.Id,
                Total = entity.Total,
                Available = entity.Available,
                Product = productName ?? entity.Product?.Title ?? "Unknown"
            };
        }

        public static List<InventoryResponse> ToResponseList(IReadOnlyList<Inventory> entities, IReadOnlyDictionary<int, string> productNamesById)
        {
            if (entities is null) throw new ArgumentNullException(nameof(entities));
            if (productNamesById is null) throw new ArgumentNullException(nameof(productNamesById));

            var list = new List<InventoryResponse>(entities.Count);
            foreach (var e in entities)
            {
                productNamesById.TryGetValue(e.ProductId, out var name);
                list.Add(ToResponse(e, name));
            }

            return list;
        }
    }
}
