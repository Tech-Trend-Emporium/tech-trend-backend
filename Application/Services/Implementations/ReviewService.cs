using Application.Abstraction;
using Application.Abstractions;
using Application.Exceptions;
using Domain.Validations;
using General.Dto.Review;
using General.Mappers;

namespace Application.Services.Implementations
{
    /// <summary>
    /// Provides business logic for managing product reviews, including creation,
    /// updates, deletion, and listing with user context.
    /// This class is documented by AI.
    /// </summary>
    public class ReviewService : IReviewService
    {
        private readonly IReviewRepository _reviewRepository;
        private readonly IUserRepository _userRepository;
        private readonly IProductRepository _productRepository;
        private readonly IUnitOfWork _unitOfWork;

        /// <summary>
        /// Initializes a new instance of the <see cref="ReviewService"/> class.
        /// </summary>
        /// <param name="reviewRepository">Repository for review persistence and queries.</param>
        /// <param name="userRepository">Repository for accessing user data.</param>
        /// <param name="productRepository">Repository for accessing product data.</param>
        /// <param name="unitOfWork">Unit of Work to coordinate transaction boundaries.</param>
        public ReviewService(
            IReviewRepository reviewRepository,
            IUserRepository userRepository,
            IProductRepository productRepository,
            IUnitOfWork unitOfWork)
        {
            _reviewRepository = reviewRepository;
            _userRepository = userRepository;
            _productRepository = productRepository;
            _unitOfWork = unitOfWork;
        }

        /// <summary>
        /// Returns the total number of reviews available.
        /// </summary>
        /// <param name="ct">An optional <see cref="CancellationToken"/> for task cancellation.</param>
        /// <returns>The total count of reviews.</returns>
        public Task<int> CountAsync(CancellationToken ct = default)
        {
            return _reviewRepository.CountAsync(null, ct);
        }

        /// <summary>
        /// Retrieves a review by its identifier and maps it to a response including the author's username.
        /// </summary>
        /// <param name="id">The review identifier.</param>
        /// <param name="ct">An optional <see cref="CancellationToken"/> for task cancellation.</param>
        /// <returns>
        /// A <see cref="ReviewResponse"/> if found; otherwise, <c>null</c>.
        /// </returns>
        public async Task<ReviewResponse?> GetByIdAsync(int id, CancellationToken ct = default)
        {
            var entity = await _reviewRepository.GetByIdAsync(ct, id);
            if (entity is null) return null;

            var user = await _userRepository.GetByIdAsync(ct, entity.UserId);

            return ReviewMapper.ToResponse(entity, user.Username);
        }

        /// <summary>
        /// Retrieves a paginated list of reviews with the total count, including usernames.
        /// </summary>
        /// <param name="skip">The number of records to skip. Defaults to 0.</param>
        /// <param name="take">The number of records to take. Defaults to 50.</param>
        /// <param name="ct">An optional <see cref="CancellationToken"/> for task cancellation.</param>
        /// <returns>
        /// A tuple with the list of reviews and the total number of reviews.
        /// </returns>
        public async Task<(IReadOnlyList<ReviewResponse> Items, int Total)> ListWithCountAsync(int skip = 0, int take = 50, CancellationToken ct = default)
        {
            var entities = await _reviewRepository.ListAsync(skip, take, ct);
            var total = await _reviewRepository.CountAsync(null, ct);

            var userIds = entities.Select(e => e.UserId).Distinct().ToList();
            var users = await _userRepository.ListByIdsAsync(ct, userIds);

            var usernameById = users.ToDictionary(u => u.Id, u => u.Username);

            var items = ReviewMapper.ToResponseList(entities, usernameById);
            return (items, total);
        }

        /// <summary>
        /// Retrieves a paginated list of reviews, including usernames.
        /// </summary>
        /// <param name="skip">The number of records to skip. Defaults to 0.</param>
        /// <param name="take">The number of records to take. Defaults to 50.</param>
        /// <param name="ct">An optional <see cref="CancellationToken"/> for task cancellation.</param>
        /// <returns>A read-only list of <see cref="ReviewResponse"/> objects.</returns>
        public async Task<IReadOnlyList<ReviewResponse>> ListAsync(int skip = 0, int take = 50, CancellationToken ct = default)
        {
            var entities = await _reviewRepository.ListAsync(skip, take, ct);

            var userIds = entities.Select(e => e.UserId).Distinct().ToList();
            var users = await _userRepository.ListByIdsAsync(ct, userIds);

            var usernameById = users.ToDictionary(u => u.Id, u => u.Username);

            return ReviewMapper.ToResponseList(entities, usernameById);
        }

        /// <summary>
        /// Creates a new review for a product authored by the specified username.
        /// Validates user existence, product existence, and duplicate constraints.
        /// </summary>
        /// <param name="dto">The creation request containing username, product, and review content.</param>
        /// <param name="ct">An optional <see cref="CancellationToken"/> for task cancellation.</param>
        /// <returns>The created <see cref="ReviewResponse"/> including the author's username.</returns>
        /// <exception cref="NotFoundException">Thrown when the user or product cannot be found.</exception>
        /// <exception cref="ConflictException">Thrown when a review already exists for the same user and product.</exception>
        public async Task<ReviewResponse> CreateAsync(CreateReviewRequest dto, CancellationToken ct = default)
        {
            var username = dto.Username.Trim().ToUpper();
            var user = await _userRepository.GetAsync(u => u.Username.Trim().ToUpper() == username, ct: ct);
            if (user is null) throw new NotFoundException(UserValidator.UserNotFound(username));

            var productExists = await _productRepository.ExistsAsync(p => p.Id == dto.ProductId, ct);
            if (!productExists) throw new NotFoundException(ProductValidator.ProductNotFound(dto.ProductId));

            var duplicate = await _reviewRepository.ExistsAsync(r => r.UserId == user.Id && r.ProductId == dto.ProductId, ct);
            if (duplicate) throw new ConflictException(ReviewValidator.ReviewAlreadyExistsForUserAndProduct(username, dto.ProductId));

            var entity = ReviewMapper.ToEntity(dto, userId: user.Id);
            _reviewRepository.Add(entity);
            await _unitOfWork.SaveChangesAsync(ct);

            return ReviewMapper.ToResponse(entity, user.Username);
        }

        /// <summary>
        /// Updates an existing review.
        /// </summary>
        /// <param name="id">The review identifier.</param>
        /// <param name="dto">The update request with new rating/comment values.</param>
        /// <param name="ct">An optional <see cref="CancellationToken"/> for task cancellation.</param>
        /// <returns>The updated <see cref="ReviewResponse"/> including the author's username.</returns>
        /// <exception cref="NotFoundException">Thrown when the review or its user cannot be found.</exception>
        public async Task<ReviewResponse> UpdateAsync(int id, UpdateReviewRequest dto, CancellationToken ct = default)
        {
            var entity = await _reviewRepository.GetByIdAsync(ct, id);
            if (entity is null) throw new NotFoundException(ReviewValidator.ReviewNotFound(id));

            var user = await _userRepository.GetByIdAsync(ct, entity.UserId);
            if (user is null) throw new NotFoundException(UserValidator.UserNotFound(entity.UserId));

            ReviewMapper.ApplyUpdate(entity, dto);

            _reviewRepository.Update(entity);
            await _unitOfWork.SaveChangesAsync(ct);

            return ReviewMapper.ToResponse(entity, user.Username);
        }

        /// <summary>
        /// Deletes a review by its identifier.
        /// </summary>
        /// <param name="id">The review identifier.</param>
        /// <param name="ct">An optional <see cref="CancellationToken"/> for task cancellation.</param>
        /// <returns><c>true</c> if the review was deleted; otherwise, <c>false</c>.</returns>
        public async Task<bool> DeleteAsync(int id, CancellationToken ct = default)
        {
            var deleted = await _reviewRepository.DeleteByIdAsync(ct, id);
            if (!deleted) return false;

            await _unitOfWork.SaveChangesAsync(ct);
            return true;
        }

        /// <summary>
        /// Retrieves a paginated list of reviews for a given product, including usernames.
        /// </summary>
        /// <param name="productId">The product identifier to filter reviews.</param>
        /// <param name="skip">The number of records to skip. Defaults to 0.</param>
        /// <param name="take">The number of records to take. Defaults to 50.</param>
        /// <param name="ct">An optional <see cref="CancellationToken"/> for task cancellation.</param>
        /// <returns>A read-only list of <see cref="ReviewResponse"/> objects for the product.</returns>
        /// <exception cref="NotFoundException">Thrown when the product does not exist.</exception>
        public async Task<IReadOnlyList<ReviewResponse>> ListByProductAsync(int productId, int skip = 0, int take = 50, CancellationToken ct = default)
        {
            var productExists = await _productRepository.ExistsAsync(p => p.Id == productId, ct);
            if (!productExists) throw new NotFoundException(ProductValidator.ProductNotFound(productId));

            var entities = await _reviewRepository.ListAsync(r => r.ProductId == productId, skip, take, ct);

            var userIds = entities.Select(e => e.UserId).Distinct().ToList();
            var users = await _userRepository.ListByIdsAsync(ct, userIds);

            var usernameById = users.ToDictionary(u => u.Id, u => u.Username);

            return ReviewMapper.ToResponseList(entities, usernameById);
        }

        /// <summary>
        /// Retrieves a paginated list of reviews for a given product with the total count, including usernames.
        /// </summary>
        /// <param name="productId">The product identifier to filter reviews.</param>
        /// <param name="skip">The number of records to skip. Defaults to 0.</param>
        /// <param name="take">The number of records to take. Defaults to 50.</param>
        /// <param name="ct">An optional <see cref="CancellationToken"/> for task cancellation.</param>
        /// <returns>
        /// A tuple containing the list of reviews for the product and the total count.
        /// </returns>
        /// <exception cref="NotFoundException">Thrown when the product does not exist.</exception>
        public async Task<(IReadOnlyList<ReviewResponse> Items, int Total)> ListByProductWithCountAsync(int productId, int skip = 0, int take = 50, CancellationToken ct = default)
        {
            var productExists = await _productRepository.ExistsAsync(p => p.Id == productId, ct);
            if (!productExists) throw new NotFoundException(ProductValidator.ProductNotFound(productId));

            var entities = await _reviewRepository.ListAsync(r => r.ProductId == productId, skip, take, ct);
            var total = await _reviewRepository.CountAsync(r => r.ProductId == productId, ct);

            var userIds = entities.Select(e => e.UserId).Distinct().ToList();
            var users = await _userRepository.ListByIdsAsync(ct, userIds);

            var usernameById = users.ToDictionary(u => u.Id, u => u.Username);

            var items = ReviewMapper.ToResponseList(entities, usernameById);
            return (items, total);
        }
    }
}
