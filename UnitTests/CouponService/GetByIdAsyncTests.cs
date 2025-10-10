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
    public class GetByIdAsyncTests
    {
        private readonly ICouponRepository _couponRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly CouponService _service;

        public GetByIdAsyncTests()
        {
            _couponRepository = Substitute.For<ICouponRepository>();
            _unitOfWork = Substitute.For<IUnitOfWork>();
            _service = new CouponService(_couponRepository, _unitOfWork);
        }

        [Fact]
        public async Task GetByIdAsync_ShouldReturnCouponResponse_WhenCouponExists()
        {
            // Arrange
            var ct = CancellationToken.None;
            int couponId = 1;
            var entity = new Coupon { Id = couponId, Code = "SAVE10", Discount = 10 };

            _couponRepository.GetByIdAsync(ct, couponId).Returns(entity);

            // Act
            var result = await _service.GetByIdAsync(couponId, ct);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(entity.Id, result!.Id);
            Assert.Equal(entity.Code, result.Code);
            Assert.Equal(entity.Discount, result.Discount);
        }

        [Fact]
        public async Task GetByIdAsync_ShouldReturnNull_WhenCouponDoesNotExist()
        {
            // Arrange
            var ct = CancellationToken.None;
            int couponId = 999;

            _couponRepository.GetByIdAsync(ct, couponId).Returns((Coupon?)null);

            // Act
            var result = await _service.GetByIdAsync(couponId, ct);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task GetByIdAsync_ShouldCallRepositoryOnce()
        {
            // Arrange
            var ct = CancellationToken.None;
            int couponId = 1;

            _couponRepository.GetByIdAsync(ct, couponId).Returns(new Coupon { Id = couponId });

            // Act
            await _service.GetByIdAsync(couponId, ct);

            // Assert
            await _couponRepository.Received(1).GetByIdAsync(ct, couponId);
        }
    }
}
