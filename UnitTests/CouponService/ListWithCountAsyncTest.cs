using Application.Abstraction;
using Application.Abstractions;
using Application.Services.Implementations;
using Data.Entities;
using NSubstitute;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnitTests.CouponServices
{
    public class ListWithCountAsyncTests
    {
        private readonly ICouponRepository _couponRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly CouponService _service;

        public ListWithCountAsyncTests()
        {
            _couponRepository = Substitute.For<ICouponRepository>();
            _unitOfWork = Substitute.For<IUnitOfWork>();
            _service = new CouponService(_couponRepository, _unitOfWork);
        }

        [Fact]
        public async Task ListWithCountAsync_ShouldReturnCouponsAndTotal_WhenDataExists()
        {
            // Arrange
            var ct = CancellationToken.None;
            int skip = 0, take = 50;
            var coupons = new List<Coupon>
            {
                new Coupon { Id = 1, Discount = 10 },
                new Coupon { Id = 2, Discount = 20 }
            };
            var total = 5;

            _couponRepository.ListAsync(skip, take, ct).Returns(coupons);
            _couponRepository.CountAsync(null, ct).Returns(total);

            // Act
            var (items, count) = await _service.ListWithCountAsync(0, 50, ct);

            // Assert
            Assert.Equal(total, count);
            Assert.Equal(coupons.Count, items.Count);
            Assert.Equal(coupons[1].Discount, items[1].Discount);

            await _couponRepository.Received(1).ListAsync(skip, take, ct);
            await _couponRepository.Received(1).CountAsync(null, ct);
        }

        [Fact]
        public async Task ListWithCountAsync_ShouldReturnEmptyList_WhenNoCouponsExist()
        {
            // Arrange
            var ct = CancellationToken.None;
            int skip = 0, take = 50;
            _couponRepository.ListAsync(skip, take, ct).Returns(new List<Coupon>());
            _couponRepository.CountAsync(null, ct).Returns(0);

            // Act
            var (items, count) = await _service.ListWithCountAsync(0, 50, ct);

            // Assert
            Assert.Empty(items);
            Assert.Equal(0, count);
        }

        [Fact]
        public async Task ListWithCountAsync_ShouldCallRepositoryWithCorrectParameters()
        {
            // Arrange
            var ct = CancellationToken.None;
            int skip = 10, take = 20;
            var coupons = new List<Coupon> { new Coupon { Id = 99, Discount = 30 } };

            _couponRepository.ListAsync(skip, take, ct).Returns(coupons);
            _couponRepository.CountAsync(null, ct).Returns(1);

            // Act
            await _service.ListWithCountAsync(skip, take, ct);

            // Assert
            await _couponRepository.Received(1).ListAsync(skip, take, ct);
            await _couponRepository.Received(1).CountAsync(null, ct);
        }
    }
}
