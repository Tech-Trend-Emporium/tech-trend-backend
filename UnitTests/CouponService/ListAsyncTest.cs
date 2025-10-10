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
    public class ListAsyncTests
    {
        private readonly ICouponRepository _couponRepository = Substitute.For<ICouponRepository>();
        private readonly IUnitOfWork _unitOfWork = Substitute.For<IUnitOfWork>();
        private readonly CouponService _sut;

        public ListAsyncTests()
        {            
            _sut = new CouponService(_couponRepository, _unitOfWork);
        }

        [Fact]
        public async Task ListAsync_ShouldReturnMappedCouponResponses_WhenCouponsExist()
        {
            // Arrange
            var ct = CancellationToken.None;
            int skip = 0, take = 50;
            var coupons = new List<Coupon>
            {
                new Coupon { Id = 1, Code = "DISC10", Discount = 10 },
                new Coupon { Id = 2, Code = "SAVE20", Discount = 20 }
            };

            _couponRepository.ListAsync(skip, take, ct).Returns(coupons);

            // Act
            var result = await _sut.ListAsync(skip, take, ct);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(coupons.Count, result.Count);            
            Assert.Equal(coupons[1].Discount, result[1].Discount);
        }

        [Fact]
        public async Task ListAsync_ShouldReturnEmptyList_WhenNoCouponsExist()
        {
            // Arrange
            var ct = CancellationToken.None;
            int skip = 0, take = 50;
            var emptyList = new List<Coupon>();

            _couponRepository.ListAsync(skip, take, ct).Returns(emptyList);

            // Act
            var result = await _sut.ListAsync(skip, take, ct);

            // Assert
            Assert.NotNull(result);
            Assert.Empty(result);
        }

        [Fact]
        public async Task ListAsync_ShouldCallRepository_WithCorrectParameters()
        {
            // Arrange
            var ct = CancellationToken.None;
            int skip = 10, take = 20;
            var coupons = new List<Coupon> { new Coupon { Id = 1, Discount = 15 } };

            _couponRepository.ListAsync(skip, take, ct).Returns(coupons);

            // Act
            await _sut.ListAsync(skip, take, ct);

            // Assert
            await _couponRepository.Received(1).ListAsync(skip, take, ct);
        }
    }
}
