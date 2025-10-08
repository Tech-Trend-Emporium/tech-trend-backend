using Application.Abstraction;
using Application.Abstractions;
using Application.Dtos.Cart;
using Application.Exceptions;
using Data.Entities;
using Domain.Validations;
using General.Dto.Cart;
using General.Mappers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Services.Implementations
{
    public class CartService : ICartService
    {
        private readonly ICartRepository _cartRepository;
        private readonly IProductRepository _productRepository;
        private readonly ICouponRepository _couponRepository;
        private readonly IInventoryRepository _inventoryRepository;
        private readonly IUnitOfWork _unitOfWork;

        public CartService(ICartRepository cartRepository, IProductRepository productRepository, ICouponRepository couponRepositor, IInventoryRepository inventoryRepository, IUnitOfWork unitOfWork)
        {
            _cartRepository = cartRepository;
            _productRepository = productRepository;
            _couponRepository = couponRepositor;
            _inventoryRepository = inventoryRepository;
            _unitOfWork = unitOfWork;
        }

        private async Task<Cart> EnsureCart(int userId, CancellationToken ct)
        {
            return await _cartRepository.GetByUserIdAsync(userId, includeGraph: true, ct) ?? await _cartRepository.CreateForUserAsync(userId, ct);
        }

        private async Task<Cart> Reload(Cart cart, CancellationToken ct)
        {
            return (await _cartRepository.GetByUserIdAsync(cart.UserId, includeGraph: true, ct))!;
        }

        private static void ValidateStock(int requestedQty, Inventory inv, Product product)
        {
            if (requestedQty < 1) throw new BadRequestException(CartValidator.QuantityMinErrorMessage);
            if (inv.Available < 0 || inv.Total < 0) throw new ConflictException(CartValidator.InventoryInvalid(product.Title));
            if (requestedQty > inv.Available) throw new ConflictException(CartValidator.InventoryInsufficient(product.Title, inv.Available));
        }

        private async Task<Product> GetProductOr404Async(int productId, CancellationToken ct)
        {
            var product = await _productRepository.GetByIdAsync(ct, productId);
            if (product is null) throw new NotFoundException(ProductValidator.ProductNotFound(productId));

            return product;
        }

        private async Task<Inventory> GetInventoryOr404ByProductIdAsync(int productId, CancellationToken ct)
        {
            var invList = await _inventoryRepository.ListAsync(i => i.ProductId == productId, 0, 1, ct);
            var inv = invList.FirstOrDefault();
            if (inv is null) throw new NotFoundException(CartValidator.InventoryNotConfigured(productId));

            return inv;
        }

        private async Task<Coupon> GetCouponByCodeOr404Async(string code, CancellationToken ct)
        {
            code = (code ?? string.Empty).Trim();
            if (string.IsNullOrWhiteSpace(code)) throw new BadRequestException(CartValidator.CouponCodeRequiredMessage);

            var list = await _couponRepository.ListAsync(c => c.Code == code, 0, 1, ct);
            var coupon = list.FirstOrDefault();
            if (coupon is null) throw new NotFoundException(CouponValidator.CouponNotFound(code));

            return coupon;
        }

        public async Task<CartResponse> GetAsync(int userId, CancellationToken ct = default)
        {
            var cart = await EnsureCart(userId, ct);

            return CartMapper.ToResponse(cart);
        }

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
                item = new CartItem { CartId = cart.Id, ProductId = dto.ProductId, Quantity = dto.Quantity };
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

        public async Task<CartResponse> RemoveItemAsync(int userId, int productId, CancellationToken ct = default)
        {
            var cart = await EnsureCart(userId, ct);
            var item = cart.Items.FirstOrDefault(i => i.ProductId == productId);
            if (item != null) cart.Items.Remove(item);

            await _unitOfWork.SaveChangesAsync(ct);
            cart = await Reload(cart, ct);

            return CartMapper.ToResponse(cart);
        }

        public async Task<CartResponse> ClearAsync(int userId, CancellationToken ct = default)
        {
            var cart = await EnsureCart(userId, ct);
            cart.Items.Clear();

            await _unitOfWork.SaveChangesAsync(ct);
            cart = await Reload(cart, ct);

            return CartMapper.ToResponse(cart);
        }

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

        public async Task<CartResponse> RemoveCouponAsync(int userId, CancellationToken ct = default)
        {
            var cart = await EnsureCart(userId, ct);
            cart.CouponId = null;

            await _unitOfWork.SaveChangesAsync(ct);
            cart = await Reload(cart, ct);

            return CartMapper.ToResponse(cart);
        }

        public async Task CheckoutAsync(int userId, CancellationToken ct = default)
        {
            var cart = await EnsureCart(userId, ct);

            await _unitOfWork.ExecuteInTransactionAsync(async innerCt =>
            {
                foreach (var item in cart.Items.ToList())
                {
                    var product = await _productRepository.GetByIdAsync(innerCt, item.ProductId) ?? throw new NotFoundException(ProductValidator.ProductNotFound(item.ProductId));

                    var invList = await _inventoryRepository.ListAsync(i => i.ProductId == item.ProductId, 0, 1, innerCt);
                    var inv = invList.FirstOrDefault() ?? throw new NotFoundException(CartValidator.InventoryNotConfiguredForProduct(product.Title));

                    if (item.Quantity > inv.Available) throw new ConflictException(CartValidator.InventoryInsufficient(product.Title, inv.Available));

                    inv.Available -= item.Quantity;
                    _inventoryRepository.Update(inv);
                }

                cart.Items.Clear();
                cart.CouponId = null;
            }, ct);
        }
    }
}
