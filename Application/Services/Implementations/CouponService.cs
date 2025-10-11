using Application.Abstraction;
using Application.Abstractions;
using Application.Exceptions;
using Domain.Validations;
using General.Dto.Coupon;
using General.Mappers;
using System.Globalization;

namespace Application.Services.Implementations
{
    /// <summary>
    /// Provides business logic for managing coupons, including creation, updates,
    /// validation, deletion, and code generation.
    /// This class is documented by AI.
    /// </summary>
    public class CouponService : ICouponService
    {
        private readonly ICouponRepository _couponRepository;
        private readonly IUnitOfWork _unitOfWork;

        /// <summary>
        /// Initializes a new instance of the <see cref="CouponService"/> class.
        /// </summary>
        /// <param name="couponRepository">Repository for coupon persistence and retrieval.</param>
        /// <param name="unitOfWork">Unit of Work to manage transactions and save changes.</param>
        public CouponService(ICouponRepository couponRepository, IUnitOfWork unitOfWork)
        {
            _couponRepository = couponRepository;
            _unitOfWork = unitOfWork;
        }

        /// <summary>
        /// Returns the total number of coupons stored in the system.
        /// </summary>
        /// <param name="ct">An optional <see cref="CancellationToken"/> for task cancellation.</param>
        /// <returns>The total count of coupons.</returns>
        public Task<int> CountAsync(CancellationToken ct = default)
        {
            return _couponRepository.CountAsync(null, ct);
        }

        /// <summary>
        /// Creates a new coupon with a unique code.
        /// </summary>
        /// <param name="dto">The data transfer object containing coupon details.</param>
        /// <param name="ct">An optional <see cref="CancellationToken"/> for task cancellation.</param>
        /// <returns>The created <see cref="CouponResponse"/> object.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="dto"/> is null.</exception>
        public async Task<CouponResponse> CreateAsync(CreateCouponRequest dto, CancellationToken ct = default)
        {
            if (dto is null)
                throw new ArgumentNullException(nameof(dto));

            var code = await GenerateUniqueCodeAsync(ct);

            var entity = CouponMapper.ToEntity(dto, code);
            _couponRepository.Add(entity);

            await _unitOfWork.SaveChangesAsync(ct);
            return CouponMapper.ToResponse(entity);
        }

        /// <summary>
        /// Deletes a coupon by its unique identifier.
        /// </summary>
        /// <param name="id">The coupon ID.</param>
        /// <param name="ct">An optional <see cref="CancellationToken"/> for task cancellation.</param>
        /// <returns><c>true</c> if deleted successfully; otherwise, <c>false</c>.</returns>
        public async Task<bool> DeleteAsync(int id, CancellationToken ct = default)
        {
            var deleted = await _couponRepository.DeleteByIdAsync(ct, id);
            if (!deleted) return false;

            await _unitOfWork.SaveChangesAsync(ct);
            return true;
        }

        /// <summary>
        /// Checks whether a coupon with the specified code exists.
        /// </summary>
        /// <param name="code">The coupon code to check.</param>
        /// <param name="ct">An optional <see cref="CancellationToken"/> for task cancellation.</param>
        /// <returns><c>true</c> if a coupon with the code exists; otherwise, <c>false</c>.</returns>
        public Task<bool> ExistsByCodeAsync(string code, CancellationToken ct = default)
        {
            var normalized = (code ?? string.Empty).Trim().ToUpperInvariant();
            return _couponRepository.ExistsAsync(c => c.Code.ToUpper() == normalized, ct);
        }

        /// <summary>
        /// Retrieves a coupon by its unique identifier.
        /// </summary>
        /// <param name="id">The coupon ID.</param>
        /// <param name="ct">An optional <see cref="CancellationToken"/> for task cancellation.</param>
        /// <returns>
        /// A <see cref="CouponResponse"/> if found; otherwise, <c>null</c>.
        /// </returns>
        public async Task<CouponResponse?> GetByIdAsync(int id, CancellationToken ct = default)
        {
            var entity = await _couponRepository.GetByIdAsync(ct, id);
            if (entity is null) return null;

            return CouponMapper.ToResponse(entity);
        }

        /// <summary>
        /// Retrieves a paginated list of coupons.
        /// </summary>
        /// <param name="skip">The number of records to skip. Defaults to 0.</param>
        /// <param name="take">The number of records to take. Defaults to 50.</param>
        /// <param name="ct">An optional <see cref="CancellationToken"/> for task cancellation.</param>
        /// <returns>A read-only list of <see cref="CouponResponse"/> objects.</returns>
        public async Task<IReadOnlyList<CouponResponse>> ListAsync(int skip = 0, int take = 50, CancellationToken ct = default)
        {
            var entities = await _couponRepository.ListAsync(skip, take, ct);
            return CouponMapper.ToResponseList(entities);
        }

        /// <summary>
        /// Retrieves a paginated list of coupons along with the total count.
        /// </summary>
        /// <param name="skip">The number of records to skip. Defaults to 0.</param>
        /// <param name="take">The number of records to take. Defaults to 50.</param>
        /// <param name="ct">An optional <see cref="CancellationToken"/> for task cancellation.</param>
        /// <returns>
        /// A tuple containing the list of <see cref="CouponResponse"/> objects and the total count of coupons.
        /// </returns>
        public async Task<(IReadOnlyList<CouponResponse> Items, int Total)> ListWithCountAsync(int skip = 0, int take = 50, CancellationToken ct = default)
        {
            var itemsTask = _couponRepository.ListAsync(skip, take, ct);
            var totalTask = _couponRepository.CountAsync(null, ct);

            await Task.WhenAll(itemsTask, totalTask);

            var items = CouponMapper.ToResponseList(itemsTask.Result);
            return (items, totalTask.Result);
        }

        /// <summary>
        /// Updates an existing coupon with new data, validating its date range and status.
        /// </summary>
        /// <param name="id">The coupon ID to update.</param>
        /// <param name="dto">The data transfer object containing new coupon information.</param>
        /// <param name="ct">An optional <see cref="CancellationToken"/> for task cancellation.</param>
        /// <returns>The updated <see cref="CouponResponse"/> object.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="dto"/> is null.</exception>
        /// <exception cref="NotFoundException">Thrown when the coupon does not exist.</exception>
        /// <exception cref="BadRequestException">Thrown when the validity dates are inconsistent.</exception>
        public async Task<CouponResponse> UpdateAsync(int id, UpdateCouponRequest dto, CancellationToken ct = default)
        {
            if (dto is null)
                throw new ArgumentNullException(nameof(dto));

            var entity = await _couponRepository.GetByIdAsync(ct, id);
            if (entity is null)
                throw new NotFoundException(CouponValidator.CouponNotFound(id));

            DateTime? fromUtc = null, toUtc = null;

            // Convert date strings to UTC
            if (!string.IsNullOrWhiteSpace(dto.ValidFrom))
                fromUtc = DateTime.ParseExact(dto.ValidFrom.Trim(), "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.AssumeUniversal | DateTimeStyles.AdjustToUniversal);
            if (!string.IsNullOrWhiteSpace(dto.ValidTo))
                toUtc = DateTime.ParseExact(dto.ValidTo.Trim(), "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.AssumeUniversal | DateTimeStyles.AdjustToUniversal);

            var newFrom = fromUtc ?? entity.ValidFrom;
            var newTo = toUtc ?? entity.ValidTo;

            if (newTo.HasValue && newTo.Value < newFrom)
                throw new BadRequestException(CouponValidator.ValidToAfterValidFromErrorMessage);

            CouponMapper.ApplyUpdate(entity, dto);

            _couponRepository.Update(entity);
            await _unitOfWork.SaveChangesAsync(ct);

            return CouponMapper.ToResponse(entity);
        }

        /// <summary>
        /// Generates a unique coupon code, ensuring no conflicts in the repository.
        /// Retries up to 5 times before falling back to a guaranteed unique GUID-based code.
        /// </summary>
        /// <param name="ct">An optional <see cref="CancellationToken"/> for task cancellation.</param>
        /// <returns>A unique coupon code string.</returns>
        private async Task<string> GenerateUniqueCodeAsync(CancellationToken ct)
        {
            const int maxAttempts = 5;

            for (int attempt = 0; attempt < maxAttempts; attempt++)
            {
                var candidate = $"CPN-{Guid.NewGuid().ToString("N")[..8].ToUpperInvariant()}";
                var exists = await _couponRepository.ExistsAsync(c => c.Code.ToUpper() == candidate, ct);
                if (!exists) return candidate;
            }

            var fallback = $"CPN-{Guid.NewGuid().ToString("N").ToUpperInvariant()}";
            return fallback;
        }
    }
}
