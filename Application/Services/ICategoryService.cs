using General.Dto.Category;

namespace Application.Services
{
    /// <summary>
    /// Defines the contract for managing <see cref="Category"/> entities and related business logic.
    /// This interface is documented by AI.
    /// </summary>
    public interface ICategoryService
    {
        /// <summary>
        /// Creates a new category based on the provided request data.
        /// </summary>
        /// <param name="dto">The data transfer object containing information for creating the category.</param>
        /// <param name="ct">An optional <see cref="CancellationToken"/> to observe while waiting for the task to complete.</param>
        /// <returns>
        /// A task representing the asynchronous operation.
        /// The task result contains the newly created <see cref="CategoryResponse"/>.
        /// </returns>
        Task<CategoryResponse> CreateAsync(CreateCategoryRequest dto, CancellationToken ct = default);

        /// <summary>
        /// Retrieves a specific category by its unique identifier.
        /// </summary>
        /// <param name="id">The unique identifier of the category.</param>
        /// <param name="ct">An optional <see cref="CancellationToken"/> to observe while waiting for the task to complete.</param>
        /// <returns>
        /// A task representing the asynchronous operation.
        /// The task result contains the <see cref="CategoryResponse"/> if found; otherwise, <c>null</c>.
        /// </returns>
        Task<CategoryResponse?> GetByIdAsync(int id, CancellationToken ct = default);

        /// <summary>
        /// Retrieves a paginated list of categories.
        /// </summary>
        /// <param name="skip">The number of records to skip before starting to collect the result set. Defaults to 0.</param>
        /// <param name="take">The number of records to return. Defaults to 50.</param>
        /// <param name="ct">An optional <see cref="CancellationToken"/> to observe while waiting for the task to complete.</param>
        /// <returns>
        /// A task representing the asynchronous operation.
        /// The task result contains a read-only list of <see cref="CategoryResponse"/> objects.
        /// </returns>
        Task<IReadOnlyList<CategoryResponse>> ListAsync(int skip = 0, int take = 50, CancellationToken ct = default);

        /// <summary>
        /// Retrieves a paginated list of categories along with the total count of available records.
        /// </summary>
        /// <param name="skip">The number of records to skip before starting to collect the result set. Defaults to 0.</param>
        /// <param name="take">The number of records to return. Defaults to 50.</param>
        /// <param name="ct">An optional <see cref="CancellationToken"/> to observe while waiting for the task to complete.</param>
        /// <returns>
        /// A task representing the asynchronous operation.
        /// The task result contains a tuple with the list of <see cref="CategoryResponse"/> objects and the total count.
        /// </returns>
        Task<(IReadOnlyList<CategoryResponse> Items, int Total)> ListWithCountAsync(int skip = 0, int take = 50, CancellationToken ct = default);

        /// <summary>
        /// Updates an existing category with the specified identifier using the provided data.
        /// </summary>
        /// <param name="id">The unique identifier of the category to update.</param>
        /// <param name="dto">The data transfer object containing the updated category information.</param>
        /// <param name="ct">An optional <see cref="CancellationToken"/> to observe while waiting for the task to complete.</param>
        /// <returns>
        /// A task representing the asynchronous operation.
        /// The task result contains the updated <see cref="CategoryResponse"/>.
        /// </returns>
        Task<CategoryResponse> UpdateAsync(int id, UpdateCategoryRequest dto, CancellationToken ct = default);

        /// <summary>
        /// Deletes a category by its unique identifier.
        /// </summary>
        /// <param name="id">The unique identifier of the category to delete.</param>
        /// <param name="ct">An optional <see cref="CancellationToken"/> to observe while waiting for the task to complete.</param>
        /// <returns>
        /// A task representing the asynchronous operation.
        /// The task result contains <c>true</c> if the category was successfully deleted; otherwise, <c>false</c>.
        /// </returns>
        Task<bool> DeleteAsync(int id, CancellationToken ct = default);

        /// <summary>
        /// Determines whether a category with the specified name already exists.
        /// </summary>
        /// <param name="name">The name of the category to check for existence.</param>
        /// <param name="ct">An optional <see cref="CancellationToken"/> to observe while waiting for the task to complete.</param>
        /// <returns>
        /// A task representing the asynchronous operation.
        /// The task result contains <c>true</c> if a category with the specified name exists; otherwise, <c>false</c>.
        /// </returns>
        Task<bool> ExistsByNameAsync(string name, CancellationToken ct = default);

        /// <summary>
        /// Counts the total number of categories available.
        /// </summary>
        /// <param name="ct">An optional <see cref="CancellationToken"/> to observe while waiting for the task to complete.</param>
        /// <returns>
        /// A task representing the asynchronous operation.
        /// The task result contains the total number of categories.
        /// </returns>
        Task<int> CountAsync(CancellationToken ct = default);
    }
}
