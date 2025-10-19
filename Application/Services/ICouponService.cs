using General.Dto.Coupon;

namespace Application.Services
{
    /// <summary>
    /// Defines the contract for managing <see cref="Coupon"/> entities and related business logic.
    /// This interface is documented by AI.
    /// </summary>
    public interface ICouponService
    {
        /// <summary>
        /// Creates a new coupon based on the provided request data.
        /// </summary>
        /// <param name="dto">The data transfer object containing coupon creation details.</param>
        /// <param name="ct">An optional <see cref="CancellationToken"/> to observe while waiting for the task to complete.</param>
        /// <returns>
        /// A task representing the asynchronous operation.
        /// The task result contains the newly created <see cref="CouponResponse"/>.
        /// </returns>
        Task<CouponResponse> CreateAsync(CreateCouponRequest dto, CancellationToken ct = default);

        /// <summary>
        /// Retrieves a specific coupon by its unique identifier.
        /// </summary>
        /// <param name="id">The unique identifier of the coupon.</param>
        /// <param name="ct">An optional <see cref="CancellationToken"/> to observe while waiting for the task to complete.</param>
        /// <returns>
        /// A task representing the asynchronous operation.
        /// The task result contains the <see cref="CouponResponse"/> if found; otherwise, <c>null</c>.
        /// </returns>
        Task<CouponResponse?> GetByIdAsync(int id, CancellationToken ct = default);

        /// <summary>
        /// Retrieves a paginated list of coupons.
        /// </summary>
        /// <param name="skip">The number of records to skip before starting to collect the result set. Defaults to 0.</param>
        /// <param name="take">The number of records to return. Defaults to 50.</param>
        /// <param name="ct">An optional <see cref="CancellationToken"/> to observe while waiting for the task to complete.</param>
        /// <returns>
        /// A task representing the asynchronous operation.
        /// The task result contains a read-only list of <see cref="CouponResponse"/> objects.
        /// </returns>
        Task<IReadOnlyList<CouponResponse>> ListAsync(int skip = 0, int take = 50, CancellationToken ct = default);

        /// <summary>
        /// Retrieves a paginated list of coupons along with the total number of available records.
        /// </summary>
        /// <param name="skip">The number of records to skip before starting to collect the result set. Defaults to 0.</param>
        /// <param name="take">The number of records to return. Defaults to 50.</param>
        /// <param name="ct">An optional <see cref="CancellationToken"/> to observe while waiting for the task to complete.</param>
        /// <returns>
        /// A task representing the asynchronous operation.
        /// The task result contains a tuple with the list of <see cref="CouponResponse"/> objects and the total count.
        /// </returns>
        Task<(IReadOnlyList<CouponResponse> Items, int Total)> ListWithCountAsync(int skip = 0, int take = 50, CancellationToken ct = default);

        /// <summary>
        /// Updates an existing coupon with the specified identifier using the provided data.
        /// </summary>
        /// <param name="id">The unique identifier of the coupon to update.</param>
        /// <param name="dto">The data transfer object containing updated coupon information.</param>
        /// <param name="ct">An optional <see cref="CancellationToken"/> to observe while waiting for the task to complete.</param>
        /// <returns>
        /// A task representing the asynchronous operation.
        /// The task result contains the updated <see cref="CouponResponse"/>.
        /// </returns>
        Task<CouponResponse> UpdateAsync(int id, UpdateCouponRequest dto, CancellationToken ct = default);

        /// <summary>
        /// Deletes a coupon by its unique identifier.
        /// </summary>
        /// <param name="id">The unique identifier of the coupon to delete.</param>
        /// <param name="ct">An optional <see cref="CancellationToken"/> to observe while waiting for the task to complete.</param>
        /// <returns>
        /// A task representing the asynchronous operation.
        /// The task result contains <c>true</c> if the coupon was successfully deleted; otherwise, <c>false</c>.
        /// </returns>
        Task<bool> DeleteAsync(int id, CancellationToken ct = default);

        /// <summary>
        /// Determines whether a coupon with the specified code already exists.
        /// </summary>
        /// <param name="code">The unique coupon code to check for existence.</param>
        /// <param name="ct">An optional <see cref="CancellationToken"/> to observe while waiting for the task to complete.</param>
        /// <returns>
        /// A task representing the asynchronous operation.
        /// The task result contains <c>true</c> if a coupon with the specified code exists; otherwise, <c>false</c>.
        /// </returns>
        Task<bool> ExistsByCodeAsync(string code, CancellationToken ct = default);

        /// <summary>
        /// Counts the total number of coupons available in the system.
        /// </summary>
        /// <param name="ct">An optional <see cref="CancellationToken"/> to observe while waiting for the task to complete.</param>
        /// <returns>
        /// A task representing the asynchronous operation.
        /// The task result contains the total number of coupons.
        /// </returns>
        Task<int> CountAsync(CancellationToken ct = default);
    }
}
