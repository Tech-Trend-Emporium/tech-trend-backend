using Application.Abstraction;
using Application.Abstractions;
using Application.Exceptions;
using Data.Entities;
using Domain.Validations;
using General.Dto.WishList;
using General.Mappers;

namespace Application.Services.Implementations
{
    /// <summary>
    /// Provides business logic for managing a user's wish list, including retrieval,
    /// add/remove/clear operations, and moving items to the shopping cart.
    /// This class is documented by AI.
    /// </summary>
    public class WishListService : IWishListService
    {
        private readonly IWishListRepository _wishListRepository;
        private readonly IProductRepository _productRepository;
        private readonly ICartRepository _cartRepository;
        private readonly IUnitOfWork _unitOfWork;

        /// <summary>
        /// Initializes a new instance of the <see cref="WishListService"/> class.
        /// </summary>
        /// <param name="wishListRepository">Repository for wish list persistence and queries.</param>
        /// <param name="productRepository">Repository for product lookups.</param>
        /// <param name="cartRepository">Repository for cart access and persistence.</param>
        /// <param name="unitOfWork">Unit of Work to coordinate transactions and save changes.</param>
        public WishListService(
            IWishListRepository wishListRepository,
            IProductRepository productRepository,
            ICartRepository cartRepository,
            IUnitOfWork unitOfWork)
        {
            _wishListRepository = wishListRepository;
            _productRepository = productRepository;
            _cartRepository = cartRepository;
            _unitOfWork = unitOfWork;
        }

        /// <summary>
        /// Ensures the user has a wish list; creates one if it does not exist.
        /// </summary>
        /// <param name="userId">The user identifier.</param>
        /// <param name="ct">An optional cancellation token.</param>
        /// <returns>The existing or newly created <see cref="WishList"/> with its graph loaded.</returns>
        private async Task<WishList> EnsureWishList(int userId, CancellationToken ct)
        {
            return await _wishListRepository.GetByUserIdAsync(userId, includeGraph: true, ct)
                ?? await _wishListRepository.CreateForUserAsync(userId, ct);
        }

        /// <summary>
        /// Reloads the user's wish list (including navigation properties).
        /// </summary>
        /// <param name="wl">The wish list to reload.</param>
        /// <param name="ct">An optional cancellation token.</param>
        /// <returns>The refreshed <see cref="WishList"/> instance.</returns>
        private async Task<WishList> Reload(WishList wl, CancellationToken ct)
        {
            return (await _wishListRepository.GetByUserIdAsync(wl.UserId, includeGraph: true, ct))!;
        }

        /// <summary>
        /// Retrieves a product or throws a 404-style exception if not found.
        /// </summary>
        /// <param name="productId">The product identifier.</param>
        /// <param name="ct">An optional cancellation token.</param>
        /// <returns>The <see cref="Product"/> entity.</returns>
        /// <exception cref="NotFoundException">Thrown when the product does not exist.</exception>
        private async Task<Product> GetProductOr404Async(int productId, CancellationToken ct)
        {
            var p = await _productRepository.GetByIdAsync(ct, productId);
            if (p is null) throw new NotFoundException(ProductValidator.ProductNotFound(productId));
            return p;
        }

        /// <summary>
        /// Retrieves the user's wish list (creating it if necessary) and maps it to a response DTO.
        /// </summary>
        /// <param name="userId">The user identifier.</param>
        /// <param name="ct">An optional cancellation token.</param>
        /// <returns>The wish list represented as a <see cref="WishListResponse"/>.</returns>
        public async Task<WishListResponse> GetAsync(int userId, CancellationToken ct = default)
        {
            var wl = await EnsureWishList(userId, ct);
            return WishListMapper.ToResponse(wl);
        }

        /// <summary>
        /// Adds a product to the user's wish list.
        /// </summary>
        /// <param name="userId">The user identifier.</param>
        /// <param name="dto">The request containing the product to add.</param>
        /// <param name="ct">An optional cancellation token.</param>
        /// <returns>The updated wish list as <see cref="WishListResponse"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="dto"/> is null.</exception>
        /// <exception cref="NotFoundException">Thrown when the product does not exist.</exception>
        public async Task<WishListResponse> AddItemAsync(int userId, AddWishListItemRequest dto, CancellationToken ct = default)
        {
            if (dto is null) throw new ArgumentNullException(nameof(dto));

            var wl = await EnsureWishList(userId, ct);
            var product = await GetProductOr404Async(dto.ProductId, ct);

            wl.AddItem(product.Id);

            await _unitOfWork.SaveChangesAsync(ct);
            wl = await Reload(wl, ct);

            return WishListMapper.ToResponse(wl);
        }

        /// <summary>
        /// Removes a product from the user's wish list.
        /// </summary>
        /// <param name="userId">The user identifier.</param>
        /// <param name="productId">The product identifier to remove.</param>
        /// <param name="ct">An optional cancellation token.</param>
        /// <returns>The updated wish list as <see cref="WishListResponse"/>.</returns>
        public async Task<WishListResponse> RemoveItemAsync(int userId, int productId, CancellationToken ct = default)
        {
            var wl = await EnsureWishList(userId, ct);
            wl.RemoveItem(productId);

            await _unitOfWork.SaveChangesAsync(ct);
            wl = await Reload(wl, ct);

            return WishListMapper.ToResponse(wl);
        }

        /// <summary>
        /// Clears all items from the user's wish list.
        /// </summary>
        /// <param name="userId">The user identifier.</param>
        /// <param name="ct">An optional cancellation token.</param>
        /// <returns>The emptied wish list as <see cref="WishListResponse"/>.</returns>
        public async Task<WishListResponse> ClearAsync(int userId, CancellationToken ct = default)
        {
            var wl = await EnsureWishList(userId, ct);
            wl.Clear();

            await _unitOfWork.SaveChangesAsync(ct);
            wl = await Reload(wl, ct);

            return WishListMapper.ToResponse(wl);
        }

        /// <summary>
        /// Moves a product from the user's wish list to their shopping cart.
        /// If the product is not on the wish list, the method returns without changes.
        /// </summary>
        /// <param name="userId">The user identifier.</param>
        /// <param name="productId">The product identifier to move.</param>
        /// <param name="ct">An optional cancellation token.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        public async Task MoveItemToCartAsync(int userId, int productId, CancellationToken ct = default)
        {
            var wl = await EnsureWishList(userId, ct);
            var item = wl.Items.FirstOrDefault(i => i.ProductId == productId);
            if (item is null) return;

            var cart = await _cartRepository.GetByUserIdAsync(userId, includeGraph: true, ct)
                       ?? await _cartRepository.CreateForUserAsync(userId, ct);

            var existing = cart.Items.FirstOrDefault(i => i.ProductId == productId);
            if (existing is null) cart.Items.Add(CartMapper.ToEntity(cart.Id, productId, 1));

            wl.RemoveItem(productId);

            await _unitOfWork.SaveChangesAsync(ct);
        }
    }
}
