using Application.Abstraction;
using Application.Abstractions;
using Application.Exceptions;
using Domain.Validations;
using General.Dto.Coupon;
using General.Mappers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Services.Implementations
{
    public class CouponService : ICouponService
    {
        private readonly ICouponRepository _couponRepository;
        private readonly IUnitOfWork _unitOfWork;

        public CouponService(ICouponRepository couponRepository, IUnitOfWork unitOfWork)
        {
            this._couponRepository = couponRepository;
            this._unitOfWork = unitOfWork;
        }

        public Task<int> CountAsync(CancellationToken ct = default)
        {
            return _couponRepository.CountAsync(null, ct);
        }

        public async Task<CouponResponse> CreateAsync(CreateCouponRequest dto, CancellationToken ct = default)
        {
            if (dto is null) throw new ArgumentNullException(nameof(dto));

            var code = await GenerateUniqueCodeAsync(ct);

            var entity = CouponMapper.ToEntity(dto, code);

            _couponRepository.Add(entity);
            await _unitOfWork.SaveChangesAsync(ct);

            return CouponMapper.ToResponse(entity);
        }

        public async Task<bool> DeleteAsync(int id, CancellationToken ct = default)
        {
            var deleted = await _couponRepository.DeleteByIdAsync(ct, id);
            if (!deleted) return false;

            await _unitOfWork.SaveChangesAsync(ct);
            return true;
        }

        public Task<bool> ExistsByCodeAsync(string code, CancellationToken ct = default)
        {
            var normalized = (code ?? string.Empty).Trim().ToUpperInvariant();

            return _couponRepository.ExistsAsync(c => c.Code.ToUpper() == normalized, ct);
        }

        public async Task<CouponResponse?> GetByIdAsync(int id, CancellationToken ct = default)
        {
            var entity = await _couponRepository.GetByIdAsync(ct, id);
            if (entity is null) return null;

            return CouponMapper.ToResponse(entity);
        }

        public async Task<IReadOnlyList<CouponResponse>> ListAsync(int skip = 0, int take = 50, CancellationToken ct = default)
        {
            var entities = await _couponRepository.ListAsync(skip, take, ct);
            return CouponMapper.ToResponseList(entities);
        }

        public async Task<(IReadOnlyList<CouponResponse> Items, int Total)> ListWithCountAsync(int skip = 0, int take = 50, CancellationToken ct = default)
        {
            var itemsTask = _couponRepository.ListAsync(skip, take, ct);
            var totalTask = _couponRepository.CountAsync(null, ct);

            await Task.WhenAll(itemsTask, totalTask);

            var items = CouponMapper.ToResponseList(itemsTask.Result);
            return (items, totalTask.Result);
        }

        public async Task<CouponResponse> UpdateAsync(int id, UpdateCouponRequest dto, CancellationToken ct = default)
        {
            if (dto is null) throw new ArgumentNullException(nameof(dto));

            var entity = await _couponRepository.GetByIdAsync(ct, id);
            if (entity is null) throw new NotFoundException(CouponValidator.CouponNotFound(id));

            var newFrom = dto.ValidFrom ?? entity.ValidFrom;
            var newTo = dto.ValidTo ?? entity.ValidTo;

            if (newTo.HasValue && newTo.Value < newFrom) throw new BadRequestException(CouponValidator.ValidToAfterValidFromErrorMessage);

            CouponMapper.ApplyUpdate(entity, dto);

            _couponRepository.Update(entity);
            await _unitOfWork.SaveChangesAsync(ct);

            return CouponMapper.ToResponse(entity);
        }


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
