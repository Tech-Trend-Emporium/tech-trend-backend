using Data.Entities;
using General.Dto.Review;

namespace Application.Services
{
    /// <summary>
    /// Defines the contract for managing <see cref="Review"/> entities and related business logic.
    /// This interface is documented by AI.
    /// </summary>
    public interface IReviewService
    {
        /// <summary>
        /// Creates a new product review based on the provided request data.
        /// </summary>
        /// <param name="dto">The data transfer object containing the review details.</param>
        /// <param name="ct">An optional <see cref="CancellationToken"/> to observe while waiting for the task to complete.</param>
        /// <returns>
        /// A task representing the asynchronous operation.  
        /// The task result contains the newly created <see cref="ReviewResponse"/>.
        /// </returns>
        Task<ReviewResponse> CreateAsync(CreateReviewRequest dto, CancellationToken ct = default);

        /// <summary>
        /// Retrieves a specific review by its unique identifier.
        /// </summary>
        /// <param name="id">The unique identifier of the review.</param>
        /// <param name="ct">An optional <see cref="CancellationToken"/> to observe while waiting for the task to complete.</param>
        /// <returns>
        /// A task representing the asynchronous operation.  
        /// The task result contains the <see cref="ReviewResponse"/> if found; otherwise, <c>null</c>.
        /// </returns>
        Task<ReviewResponse?> GetByIdAsync(int id, CancellationToken ct = default);

        /// <summary>
        /// Retrieves a paginated list of all product reviews.
        /// </summary>
        /// <param name="skip">The number of records to skip before starting to collect the result set. Defaults to 0.</param>
        /// <param name="take">The number of records to return. Defaults to 50.</param>
        /// <param name="ct">An optional <see cref="CancellationToken"/> to observe while waiting for the task to complete.</param>
        /// <returns>
        /// A task representing the asynchronous operation.  
        /// The task result contains a read-only list of <see cref="ReviewResponse"/> objects.
        /// </returns>
        Task<IReadOnlyList<ReviewResponse>> ListAsync(int skip = 0, int take = 50, CancellationToken ct = default);

        /// <summary>
        /// Retrieves a paginated list of reviews along with the total count of available records.
        /// </summary>
        /// <param name="skip">The number of records to skip before starting to collect the result set. Defaults to 0.</param>
        /// <param name="take">The number of records to return. Defaults to 50.</param>
        /// <param name="ct">An optional <see cref="CancellationToken"/> to observe while waiting for the task to complete.</param>
        /// <returns>
        /// A task representing the asynchronous operation.  
        /// The task result contains a tuple with the list of <see cref="ReviewResponse"/> objects and the total count.
        /// </returns>
        Task<(IReadOnlyList<ReviewResponse> Items, int Total)> ListWithCountAsync(int skip = 0, int take = 50, CancellationToken ct = default);

        /// <summary>
        /// Retrieves a paginated list of reviews associated with a specific product.
        /// </summary>
        /// <param name="productId">The unique identifier of the product whose reviews are being retrieved.</param>
        /// <param name="skip">The number of records to skip before starting to collect the result set. Defaults to 0.</param>
        /// <param name="take">The number of records to return. Defaults to 50.</param>
        /// <param name="ct">An optional <see cref="CancellationToken"/> to observe while waiting for the task to complete.</param>
        /// <returns>
        /// A task representing the asynchronous operation.  
        /// The task result contains a read-only list of <see cref="ReviewResponse"/> objects for the specified product.
        /// </returns>
        Task<IReadOnlyList<ReviewResponse>> ListByProductAsync(int productId, int skip = 0, int take = 50, CancellationToken ct = default);

        /// <summary>
        /// Retrieves a paginated list of reviews for a specific product along with the total count of available records.
        /// </summary>
        /// <param name="productId">The unique identifier of the product whose reviews are being retrieved.</param>
        /// <param name="skip">The number of records to skip before starting to collect the result set. Defaults to 0.</param>
        /// <param name="take">The number of records to return. Defaults to 50.</param>
        /// <param name="ct">An optional <see cref="CancellationToken"/> to observe while waiting for the task to complete.</param>
        /// <returns>
        /// A task representing the asynchronous operation.  
        /// The task result contains a tuple with the list of <see cref="ReviewResponse"/> objects and the total count.
        /// </returns>
        Task<(IReadOnlyList<ReviewResponse> Items, int Total)> ListByProductWithCountAsync(int productId, int skip = 0, int take = 50, CancellationToken ct = default);

        /// <summary>
        /// Updates an existing review with the specified identifier using the provided data.
        /// </summary>
        /// <param name="id">The unique identifier of the review to update.</param>
        /// <param name="dto">The data transfer object containing updated review information.</param>
        /// <param name="ct">An optional <see cref="CancellationToken"/> to observe while waiting for the task to complete.</param>
        /// <returns>
        /// A task representing the asynchronous operation.  
        /// The task result contains the updated <see cref="ReviewResponse"/>.
        /// </returns>
        Task<ReviewResponse> UpdateAsync(int id, UpdateReviewRequest dto, CancellationToken ct = default);

        /// <summary>
        /// Deletes a review by its unique identifier.
        /// </summary>
        /// <param name="id">The unique identifier of the review to delete.</param>
        /// <param name="ct">An optional <see cref="CancellationToken"/> to observe while waiting for the task to complete.</param>
        /// <returns>
        /// A task representing the asynchronous operation.  
        /// The task result contains <c>true</c> if the review was successfully deleted; otherwise, <c>false</c>.
        /// </returns>
        Task<bool> DeleteAsync(int id, CancellationToken ct = default);

        /// <summary>
        /// Counts the total number of reviews available in the system.
        /// </summary>
        /// <param name="ct">An optional <see cref="CancellationToken"/> to observe while waiting for the task to complete.</param>
        /// <returns>
        /// A task representing the asynchronous operation.  
        /// The task result contains the total number of reviews.
        /// </returns>
        Task<int> CountAsync(CancellationToken ct = default);
    }
}
