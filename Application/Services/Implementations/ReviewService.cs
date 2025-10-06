using Application.Abstraction;
using Application.Abstractions;
using Data.Entities;
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

        public async Task<ReviewResponse> CreateAsync(CreateReviewRequest dto, CancellationToken ct = default)
        {
            //1. Check if productId exists in the database, else return null
            var exixtingProduct = await _productRepository.GetByIdAsync(ct, dto.ProductId);
            if (exixtingProduct == null) return null;
            //Check if the username exists in the database, else return null
            var username = dto.Username.Trim().ToUpper();
            var existingUser = await _userRepository.GetAsync(u => u.Username.Trim().ToUpper() == username, asTracking: false, ct: ct);
            if (existingUser == null) return null;
            //2. Check if rating is greater than or equal to 0, else defined to 0 
            if (dto.Rating < 0) { dto.Rating = 0; }
            //3. If exists, Create the entity to be added
            var Entity  = ReviewMapper.ToEntity(dto, existingUser.Id);
            //4. Add the entity using the repository
            _reviewRepository.Add(Entity);
            //5. Call SaveChangesAsync on the UnitOfWork
            await _unitOfWork.SaveChangesAsync(ct);
            //6. Return the created entity mapped to a ReviewResponse DTO
            return ReviewMapper.ToResponse(Entity, existingUser.Username);
        }

        public async Task<IReadOnlyList<Review>> ListAsync(int skip = 0, int take = 50, int productId = 0, CancellationToken ct = default)
        {
            //1. Checks if productId is provided exists in the database, else return null
            var existingProduct = await _productRepository.GetByIdAsync(ct, productId);
            if (existingProduct == null) return null;
            //2. If exists, retrieve the list of reviews for the productId using the repository
            var reviews = await _reviewRepository.ListAsync(skip, take, ct);
            //3. Return the list of reviews
            //a. Create a list of reviews filtered by productId
            var filteredReviews = reviews.Where(r => r.ProductId == productId).ToList();
            //Create an empty list of ReviewResponse
            
            return filteredReviews;
        }
    }
}
