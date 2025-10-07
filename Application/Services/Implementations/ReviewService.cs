using Application.Abstraction;
using Application.Abstractions;
using Application.Exceptions;
using Data.Entities;
using Domain.Validations;
using General.Dto.Review;
using General.Mappers;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Services.Implementations
{
    public class ReviewService : IReviewService
    {
        private readonly IReviewRepository _reviewRepository;
        private readonly IUserRepository _userRepository;
        private readonly IProductRepository _productRepository;
        private readonly IUnitOfWork _unitOfWork;

        public ReviewService(IReviewRepository reviewRepository, IUserRepository userRepository, IProductRepository productRepository, IUnitOfWork unitOfWork)
        {
            _reviewRepository = reviewRepository;
            _userRepository = userRepository;
            _productRepository = productRepository;
            _unitOfWork = unitOfWork;
        }

        public Task<int> CountAsync(CancellationToken ct = default)
        {
            return _reviewRepository.CountAsync(null, ct);
        }

        public async Task<ReviewResponse?> GetByIdAsync(int id, CancellationToken ct = default)
        {
            var entity = await _reviewRepository.GetByIdAsync(ct, id);
            if (entity is null) return null;

            var user = await _userRepository.GetByIdAsync(ct, entity.UserId);

            return ReviewMapper.ToResponse(entity, user.Username);
        }

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

        public async Task<IReadOnlyList<ReviewResponse>> ListAsync(int skip = 0, int take = 50, CancellationToken ct = default)
        {
            var entities = await _reviewRepository.ListAsync(skip, take, ct);

            var userIds = entities.Select(e => e.UserId).Distinct().ToList();
            var users = await _userRepository.ListByIdsAsync(ct, userIds);

            var usernameById = users.ToDictionary(u => u.Id, u => u.Username);

            return ReviewMapper.ToResponseList(entities, usernameById);
        }

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

        public async Task<bool> DeleteAsync(int id, CancellationToken ct = default)
        {
            var deleted = await _reviewRepository.DeleteByIdAsync(ct, id);
            if (!deleted) return false;

            await _unitOfWork.SaveChangesAsync(ct);
            return true;
        }

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
