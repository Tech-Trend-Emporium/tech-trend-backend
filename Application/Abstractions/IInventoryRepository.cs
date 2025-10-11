using Data.Entities;

namespace Application.Abstraction
{
    /// <summary>
    /// Interface for managing <see cref="Inventory"/> entities within the data layer.
    /// This interface is documented by AI.
    /// </summary>
    public interface IInventoryRepository : IEfRepository<Inventory>
    {
        // This interface currently inherits all base CRUD operations 
        // from <see cref="IEfRepository{Inventory}"/>.
        // Additional methods specific to inventory management can be defined here in the future.
    }
}
