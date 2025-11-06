using Application.Abstraction;
using Application.Abstractions;
using Application.Dtos.Cart;
using Application.Exceptions;
using Data.Entities;
using Domain.Enums;
using Domain.Validations;
using General.Dto.Cart;
using General.Mappers;
using Microsoft.Extensions.Configuration;
using System.Security.Cryptography;

namespace Application.Services.Implementations
{
    /// <summary>
    /// Provides shopping cart operations (retrieve, add/update/remove items, clear, coupons, checkout).
    /// Coordinates persistence through repositories and the Unit of Work.
    /// This class is documented by AI.
    /// </summary>
    public class CartService : ICartService
    {
        private readonly IConfiguration _config;
        private readonly ICartRepository _cartRepository;
        private readonly IProductRepository _productRepository;
        private readonly ICouponRepository _couponRepository;
        private readonly IInventoryRepository _inventoryRepository;
        private readonly IUnitOfWork _unitOfWork;

        /// <summary>
        /// Initializes a new instance of <see cref="CartService"/>.
        /// </summary>
        /// <param name="cartRepository">Repository for cart access and persistence.</param>
        /// <param name="productRepository">Repository for product lookups.</param>
        /// <param name="couponRepositor">Repository for coupon lookups. (Note: parameter name appears misspelled.)</param>
        /// <param name="inventoryRepository">Repository for inventory checks and updates.</param>
        /// <param name="unitOfWork">Unit of Work to persist changes and orchestrate transactions.</param>
        public CartService(
            IConfiguration config,
            ICartRepository cartRepository,
            IProductRepository productRepository,
            ICouponRepository couponRepositor,
            IInventoryRepository inventoryRepository,
            IUnitOfWork unitOfWork)
        {
            _config = config;
            _cartRepository = cartRepository;
            _productRepository = productRepository;
            _couponRepository = couponRepositor;
            _inventoryRepository = inventoryRepository;
            _unitOfWork = unitOfWork;
        }

        /// <summary>
        /// Ensures the user has a cart; creates one if it does not exist.
        /// </summary>
        /// <param name="userId">The user identifier.</param>
        /// <param name="ct">The cancellation token.</param>
        /// <returns>The existing or newly created <see cref="Cart"/> with related graph loaded.</returns>
        private async Task<Cart> EnsureCart(int userId, CancellationToken ct)
        {
            var cart = await _cartRepository.GetByUserIdAsync(userId, includeGraph: true, ct);
            if (cart is not null && cart.Status == CartStatus.ACTIVE) return cart;

            return await _cartRepository.CreateForUserAsync(userId, ct);
        }

        /// <summary>
        /// Reloads the cart from the data source including its navigation graph.
        /// </summary>
        /// <param name="cart">The cart to reload.</param>
        /// <param name="ct">The cancellation token.</param>
        /// <returns>The refreshed cart instance.</returns>
        private async Task<Cart> Reload(Cart cart, CancellationToken ct)
        {
            return (await _cartRepository.GetByUserIdAsync(cart.UserId, includeGraph: true, ct))!;
        }

        /// <summary>
        /// Validates requested quantity against inventory constraints.
        /// </summary>
        /// <param name="requestedQty">Quantity requested for the item.</param>
        /// <param name="inv">Inventory information for the product.</param>
        /// <param name="product">The related product.</param>
        /// <exception cref="BadRequestException">Thrown if quantity is less than 1.</exception>
        /// <exception cref="ConflictException">Thrown if inventory is invalid or insufficient.</exception>
        private static void ValidateStock(int requestedQty, Inventory inv, Product product)
        {
            if (requestedQty < 1) throw new BadRequestException(CartValidator.QuantityMinErrorMessage);
            if (inv.Available < 0 || inv.Total < 0) throw new ConflictException(CartValidator.InventoryInvalid(product.Title));
            if (requestedQty > inv.Available) throw new ConflictException(CartValidator.InventoryInsufficient(product.Title, inv.Available));
        }

        /// <summary>
        /// Retrieves a product or throws 404-style exception if not found.
        /// </summary>
        /// <param name="productId">Product identifier.</param>
        /// <param name="ct">Cancellation token.</param>
        /// <returns>The <see cref="Product"/> entity.</returns>
        /// <exception cref="NotFoundException">Thrown when product does not exist.</exception>
        private async Task<Product> GetProductOr404Async(int productId, CancellationToken ct)
        {
            var product = await _productRepository.GetByIdAsync(ct, productId);
            if (product is null) throw new NotFoundException(ProductValidator.ProductNotFound(productId));

            return product;
        }

        /// <summary>
        /// Retrieves inventory by product ID or throws if not configured.
        /// </summary>
        /// <param name="productId">Product identifier.</param>
        /// <param name="ct">Cancellation token.</param>
        /// <returns>The <see cref="Inventory"/> entry.</returns>
        /// <exception cref="NotFoundException">Thrown when inventory is not configured.</exception>
        private async Task<Inventory> GetInventoryOr404ByProductIdAsync(int productId, CancellationToken ct)
        {
            var invList = await _inventoryRepository.ListAsync(i => i.ProductId == productId, 0, 1, ct);
            var inv = invList.FirstOrDefault();
            if (inv is null) throw new NotFoundException(CartValidator.InventoryNotConfigured(productId));

            return inv;
        }

        /// <summary>
        /// Retrieves a coupon by code or throws if not valid/found.
        /// </summary>
        /// <param name="code">Coupon code.</param>
        /// <param name="ct">Cancellation token.</param>
        /// <returns>The <see cref="Coupon"/> instance.</returns>
        /// <exception cref="BadRequestException">Thrown when code is empty.</exception>
        /// <exception cref="NotFoundException">Thrown when coupon is not found.</exception>
        private async Task<Coupon> GetCouponByCodeOr404Async(string code, CancellationToken ct)
        {
            code = (code ?? string.Empty).Trim();
            if (string.IsNullOrWhiteSpace(code)) throw new BadRequestException(CartValidator.CouponCodeRequiredMessage);

            var list = await _couponRepository.ListAsync(c => c.Code == code, 0, 1, ct);
            var coupon = list.FirstOrDefault();
            if (coupon is null) throw new NotFoundException(CouponValidator.CouponNotFound(code));

            return coupon;
        }

        /// <summary>
        /// Simulates a payment authorization/charge for academic/demo purposes.
        /// Uses a configurable success rate and generates a pseudo transaction identifier.
        /// </summary>
        /// <param name="amount">The total amount to charge.</param>
        /// <param name="method">The selected payment method.</param>
        /// <param name="clientProvidedTxnId">
        /// An optional client-provided transaction/intent id (e.g., from a mocked UI). If empty, one is generated.
        /// </param>
        /// <param name="ct">A cancellation token.</param>
        /// <returns>
        /// A tuple with:
        /// <list type="bullet">
        /// <item><description><see cref="PaymentStatus"/> result (APROBBED or REJECTED)</description></item>
        /// <item><description>Transaction id string</description></item>
        /// <item><description>PaidAtUtc when approved; otherwise null</description></item>
        /// </list>
        /// </returns>
        private async Task<(PaymentStatus status, DateTime? paidAtUtc)> SimulatePaymentAsync(decimal amount, PaymentMethod method, CancellationToken ct)
        {
            await Task.Yield();

            var sim = _config.GetSection("Payments").GetSection("Simulation");
            var successRate = sim.GetValue<int?>("SuccessRatePercent") ?? 90;

            successRate = Math.Clamp(successRate, 0, 100);

            var roll = RandomNumberGenerator.GetInt32(1, 101);
            var approved = (amount > 0m) && (roll <= successRate);

            return approved
                ? (PaymentStatus.APPROVED, DateTime.UtcNow)
                : (PaymentStatus.REJECTED, null);
        }

        /// <summary>
        /// Gets the user's cart (creating one on first access) and returns it as DTO.
        /// </summary>
        /// <param name="userId">The user identifier.</param>
        /// <param name="ct">The cancellation token.</param>
        /// <returns>The cart projection as <see cref="CartResponse"/>.</returns>
        public async Task<CartResponse> GetAsync(int userId, CancellationToken ct = default)
        {
            var cart = await EnsureCart(userId, ct);
            return CartMapper.ToResponse(cart);
        }

        /// <summary>
        /// Adds an item to the cart (or increases its quantity), validating inventory first.
        /// </summary>
        /// <param name="userId">The user identifier.</param>
        /// <param name="dto">Item and quantity to add.</param>
        /// <param name="ct">The cancellation token.</param>
        /// <returns>The updated cart as <see cref="CartResponse"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="dto"/> is null.</exception>
        /// <exception cref="BadRequestException">Thrown when quantity is not positive.</exception>
        /// <exception cref="NotFoundException">Thrown when product or inventory is missing.</exception>
        /// <exception cref="ConflictException">Thrown when inventory is insufficient or invalid.</exception>
        public async Task<CartResponse> AddItemAsync(int userId, AddCartItemRequest dto, CancellationToken ct = default)
        {
            if (dto is null) throw new ArgumentNullException(nameof(dto));
            if (dto.Quantity <= 0) throw new BadRequestException(CartValidator.QuantityPositiveMessage);

            var cart = await EnsureCart(userId, ct);
            var product = await GetProductOr404Async(dto.ProductId, ct);
            var inv = await GetInventoryOr404ByProductIdAsync(dto.ProductId, ct);

            var item = cart.Items.FirstOrDefault(i => i.ProductId == dto.ProductId);
            var newQty = (item?.Quantity ?? 0) + dto.Quantity;

            ValidateStock(newQty, inv, product);

            if (item is null)
            {
                item = CartMapper.ToEntity(dto, cart.Id);
                cart.Items.Add(item);
            }
            else
            {
                checked { item.Quantity = newQty; }
            }

            await _unitOfWork.SaveChangesAsync(ct);
            cart = await Reload(cart, ct);
            return CartMapper.ToResponse(cart);
        }

        /// <summary>
        /// Updates quantity for a product in the cart; removes the item if quantity &lt;= 0.
        /// </summary>
        /// <param name="userId">The user identifier.</param>
        /// <param name="dto">Product identifier and new quantity.</param>
        /// <param name="ct">The cancellation token.</param>
        /// <returns>The updated cart as <see cref="CartResponse"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="dto"/> is null.</exception>
        /// <exception cref="NotFoundException">Thrown when the item is not in the cart or product/inventory is missing.</exception>
        /// <exception cref="ConflictException">Thrown when inventory is insufficient.</exception>
        public async Task<CartResponse> UpdateQuantityAsync(int userId, UpdateCartItemRequest dto, CancellationToken ct = default)
        {
            if (dto is null) throw new ArgumentNullException(nameof(dto));

            var cart = await EnsureCart(userId, ct);
            var item = cart.Items.FirstOrDefault(i => i.ProductId == dto.ProductId);
            if (item is null) throw new NotFoundException(CartValidator.ProductNotInCartMessage);

            if (dto.Quantity <= 0)
            {
                cart.Items.Remove(item);
            }
            else
            {
                var product = await GetProductOr404Async(dto.ProductId, ct);
                var inv = await GetInventoryOr404ByProductIdAsync(dto.ProductId, ct);

                ValidateStock(dto.Quantity, inv, product);
                item.Quantity = dto.Quantity;
            }

            await _unitOfWork.SaveChangesAsync(ct);
            cart = await Reload(cart, ct);

            return CartMapper.ToResponse(cart);
        }

        /// <summary>
        /// Removes a specific product from the cart (no-op if not present).
        /// </summary>
        /// <param name="userId">The user identifier.</param>
        /// <param name="productId">The product identifier to remove.</param>
        /// <param name="ct">The cancellation token.</param>
        /// <returns>The updated cart as <see cref="CartResponse"/>.</returns>
        public async Task<CartResponse> RemoveItemAsync(int userId, int productId, CancellationToken ct = default)
        {
            var cart = await EnsureCart(userId, ct);
            var item = cart.Items.FirstOrDefault(i => i.ProductId == productId);
            if (item != null) cart.Items.Remove(item);

            await _unitOfWork.SaveChangesAsync(ct);
            cart = await Reload(cart, ct);

            return CartMapper.ToResponse(cart);
        }

        /// <summary>
        /// Clears all items from the user's cart.
        /// </summary>
        /// <param name="userId">The user identifier.</param>
        /// <param name="ct">The cancellation token.</param>
        /// <returns>The emptied cart as <see cref="CartResponse"/>.</returns>
        public async Task<CartResponse> ClearAsync(int userId, CancellationToken ct = default)
        {
            var cart = await EnsureCart(userId, ct);
            cart.Items.Clear();

            await _unitOfWork.SaveChangesAsync(ct);
            cart = await Reload(cart, ct);

            return CartMapper.ToResponse(cart);
        }

        /// <summary>
        /// Applies a coupon to the cart after validating status and dates.
        /// </summary>
        /// <param name="userId">The user identifier.</param>
        /// <param name="dto">Coupon code to apply.</param>
        /// <param name="ct">The cancellation token.</param>
        /// <returns>The updated cart as <see cref="CartResponse"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="dto"/> is null.</exception>
        /// <exception cref="BadRequestException">Thrown when the coupon is not active or dates are invalid.</exception>
        /// <exception cref="NotFoundException">Thrown when the coupon does not exist.</exception>
        public async Task<CartResponse> ApplyCouponAsync(int userId, ApplyCouponRequest dto, CancellationToken ct = default)
        {
            if (dto is null) throw new ArgumentNullException(nameof(dto));

            var cart = await EnsureCart(userId, ct);
            var coupon = await GetCouponByCodeOr404Async(dto.CouponCode, ct);

            if (!coupon.Active) throw new BadRequestException(CartValidator.CouponNotActiveMessage);
            var now = DateTime.UtcNow;
            if (coupon.ValidFrom > now) throw new BadRequestException(CartValidator.CouponNotActiveMessage);
            if (coupon.ValidTo.HasValue && coupon.ValidTo.Value < now) throw new BadRequestException(CartValidator.CouponExpiredMessage);

            cart.CouponId = coupon.Id;

            await _unitOfWork.SaveChangesAsync(ct);
            cart = await Reload(cart, ct);

            return CartMapper.ToResponse(cart);
        }

        /// <summary>
        /// Removes any applied coupon from the cart.
        /// </summary>
        /// <param name="userId">The user identifier.</param>
        /// <param name="ct">The cancellation token.</param>
        /// <returns>The updated cart as <see cref="CartResponse"/>.</returns>
        public async Task<CartResponse> RemoveCouponAsync(int userId, CancellationToken ct = default)
        {
            var cart = await EnsureCart(userId, ct);
            cart.CouponId = null;

            await _unitOfWork.SaveChangesAsync(ct);
            cart = await Reload(cart, ct);

            return CartMapper.ToResponse(cart);
        }

        /// <summary>
        /// Performs checkout by simulating payment and finalizing the order atomically.
        /// When payment is approved, inventory is decremented; when rejected, inventory remains unchanged.
        /// In both cases, the cart is snapshotted as an order (PLACED) and a new ACTIVE cart is created.
        /// </summary>
        /// <param name="userId">The user identifier.</param>
        /// <param name="dto">The checkout request data transfer object (address, payment method, optional txn id).</param>
        /// <param name="ct">The cancellation token.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        /// <exception cref="ConflictException">Thrown when the cart is not ACTIVE or inventory is insufficient.</exception>
        /// <exception cref="NotFoundException">Thrown when a product or its inventory cannot be found.</exception>
        public async Task CheckoutAsync(int userId, CheckoutRequest dto, CancellationToken ct = default)
        {
            var cart = await _cartRepository.GetByUserIdAsync(userId, includeGraph: true, ct)
                ?? await _cartRepository.CreateForUserAsync(userId, ct);

            if (cart.Status != CartStatus.ACTIVE)
                throw new ConflictException(CartValidator.CartNotActiveErrorMessage);

            await _unitOfWork.ExecuteInTransactionAsync(async innerCt =>
            {
                decimal total = 0m;
                foreach (var item in cart.Items.ToList())
                {
                    var product = await _productRepository.GetByIdAsync(innerCt, item.ProductId)
                        ?? throw new NotFoundException(ProductValidator.ProductNotFound(item.ProductId));

                    checked { total += product.Price * item.Quantity; }
                }

                var (simStatus, paidAtUtc) =
                    await SimulatePaymentAsync(total, dto.PaymentMethod, innerCt);

                if (simStatus == PaymentStatus.APPROVED)
                {
                    foreach (var item in cart.Items.ToList())
                    {
                        var product = await _productRepository.GetByIdAsync(innerCt, item.ProductId)
                            ?? throw new NotFoundException(ProductValidator.ProductNotFound(item.ProductId));

                        var invList = await _inventoryRepository.ListAsync(i => i.ProductId == item.ProductId, 0, 1, innerCt);
                        var inv = invList.FirstOrDefault()
                            ?? throw new NotFoundException(CartValidator.InventoryNotConfiguredForProduct(product.Title));

                        if (item.Quantity > inv.Available)
                            throw new ConflictException(CartValidator.InventoryInsufficient(product.Title, inv.Available));

                        inv.Available -= item.Quantity;
                        _inventoryRepository.Update(inv);
                    }
                }

                cart.Address = dto.Address;
                cart.PaymentMethod = dto.PaymentMethod;
                cart.PaymentStatus = simStatus;
                cart.TotalAmount = total;
                cart.Status = CartStatus.PLACED;
                cart.PlacedAtUtc = DateTime.UtcNow;
                cart.PaidAtUtc = simStatus == PaymentStatus.APPROVED ? paidAtUtc : null;
                cart.CouponId = null;

                await _unitOfWork.SaveChangesAsync(innerCt);

                await _cartRepository.CreateForUserAsync(userId, innerCt);
            }, ct);
        }

        /// <summary>
        /// Retrieves the current user's placed orders with pagination.
        /// </summary>
        /// <param name="userId">The unique identifier of the user.</param>
        /// <param name="skip">The number of records to skip. Defaults to 0.</param>
        /// <param name="take">The number of records to return. Defaults to 50.</param>
        /// <param name="ct">An optional <see cref="CancellationToken"/>.</param>
        /// <returns>
        /// A tuple with the list of <see cref="OrderResponse"/> items and the total count of placed orders.
        /// </returns>
        public async Task<(IReadOnlyList<OrderResponse> Items, int Total)> ListMyOrdersAsync(int userId, int skip = 0, int take = 50, CancellationToken ct = default)
        {
            var carts = await _cartRepository.ListPlacedByUserAsync(userId, skip, take, ct);
            var total = await _cartRepository.CountPlacedByUserAsync(userId, ct);

            var items = CartMapper.ToOrderResponseList(carts);
            
            return (items, total);
        }
    }
}
