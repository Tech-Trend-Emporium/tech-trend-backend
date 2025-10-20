using Data.Entities;

namespace Application.Abstraction
{
    /// <summary>
    /// Interface for managing <see cref="Review"/> entities within the data layer.
    /// This interface is documented by AI.
    /// </summary>
    public interface IReviewRepository : IEfRepository<Review>
    {
        // This interface currently inherits all base CRUD operations 
        // from <see cref="IEfRepository{Review}"/>.
        // Additional review-specific methods can be defined here in the future.
    }
}
