using General.Dto.Product;

namespace Application.Services
{
    /// <summary>
    /// Defines the contract for managing <see cref="Product"/> entities and related business operations.
    /// This interface is documented by AI.
    /// </summary>
    public interface IProductService
    {
        /// <summary>
        /// Creates a new product based on the provided request data.
        /// </summary>
        /// <param name="dto">The data transfer object containing the product details to be created.</param>
        /// <param name="ct">An optional <see cref="CancellationToken"/> to observe while waiting for the task to complete.</param>
        /// <returns>
        /// A task representing the asynchronous operation.  
        /// The task result contains the newly created <see cref="ProductResponse"/>.
        /// </returns>
        Task<ProductResponse> CreateAsync(CreateProductRequest dto, CancellationToken ct = default);

        /// <summary>
        /// Retrieves a specific product by its unique identifier.
        /// </summary>
        /// <param name="id">The unique identifier of the product.</param>
        /// <param name="ct">An optional <see cref="CancellationToken"/> to observe while waiting for the task to complete.</param>
        /// <returns>
        /// A task representing the asynchronous operation.  
        /// The task result contains the <see cref="ProductResponse"/> if found; otherwise, <c>null</c>.
        /// </returns>
        Task<ProductResponse?> GetByIdAsync(int id, CancellationToken ct = default);

        /// <summary>
        /// Retrieves a paginated list of products.
        /// </summary>
        /// <param name="skip">The number of records to skip before starting to collect the result set. Defaults to 0.</param>
        /// <param name="take">The number of records to return. Defaults to 50.</param>
        /// <param name="ct">An optional <see cref="CancellationToken"/> to observe while waiting for the task to complete.</param>
        /// <returns>
        /// A task representing the asynchronous operation.  
        /// The task result contains a read-only list of <see cref="ProductResponse"/> objects.
        /// </returns>
        Task<IReadOnlyList<ProductResponse>> ListAsync(int skip = 0, int take = 50, CancellationToken ct = default);

        /// <summary>
        /// Retrieves a paginated list of products along with the total count of available records.  
        /// Optionally filters the results by category name.
        /// </summary>
        /// <param name="skip">The number of records to skip before starting to collect the result set. Defaults to 0.</param>
        /// <param name="take">The number of records to return. Defaults to 50.</param>
        /// <param name="category">An optional category name to filter products. If <c>null</c>, all products are included.</param>
        /// <param name="ct">An optional <see cref="CancellationToken"/> to observe while waiting for the task to complete.</param>
        /// <returns>
        /// A task representing the asynchronous operation.  
        /// The task result contains a tuple with the list of <see cref="ProductResponse"/> objects and the total count.
        /// </returns>
        Task<(IReadOnlyList<ProductResponse> Items, int Total)> ListWithCountAsync(
            int skip = 0,
            int take = 50,
            string? category = null,
            CancellationToken ct = default);

        /// <summary>
        /// Updates an existing product with the specified identifier using the provided data.
        /// </summary>
        /// <param name="id">The unique identifier of the product to update.</param>
        /// <param name="dto">The data transfer object containing updated product details.</param>
        /// <param name="ct">An optional <see cref="CancellationToken"/> to observe while waiting for the task to complete.</param>
        /// <returns>
        /// A task representing the asynchronous operation.  
        /// The task result contains the updated <see cref="ProductResponse"/>.
        /// </returns>
        Task<ProductResponse> UpdateAsync(int id, UpdateProductRequest dto, CancellationToken ct = default);

        /// <summary>
        /// Deletes a product by its unique identifier.
        /// </summary>
        /// <param name="id">The unique identifier of the product to delete.</param>
        /// <param name="ct">An optional <see cref="CancellationToken"/> to observe while waiting for the task to complete.</param>
        /// <returns>
        /// A task representing the asynchronous operation.  
        /// The task result contains <c>true</c> if the product was successfully deleted; otherwise, <c>false</c>.
        /// </returns>
        Task<bool> DeleteAsync(int id, CancellationToken ct = default);

        /// <summary>
        /// Determines whether a product with the specified name already exists.
        /// </summary>
        /// <param name="name">The name of the product to check for existence.</param>
        /// <param name="ct">An optional <see cref="CancellationToken"/> to observe while waiting for the task to complete.</param>
        /// <returns>
        /// A task representing the asynchronous operation.  
        /// The task result contains <c>true</c> if a product with the specified name exists; otherwise, <c>false</c>.
        /// </returns>
        Task<bool> ExistsByNameAsync(string name, CancellationToken ct = default);

        /// <summary>
        /// Counts the total number of products available.
        /// </summary>
        /// <param name="ct">An optional <see cref="CancellationToken"/> to observe while waiting for the task to complete.</param>
        /// <returns>
        /// A task representing the asynchronous operation.  
        /// The task result contains the total number of products.
        /// </returns>
        Task<int> CountAsync(CancellationToken ct = default);
    }
}
