using Application.Abstraction;
using Application.Abstractions;
using General.Dto.Product;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using General.Mappers;
using Application.Dtos.Product;
using Microsoft.EntityFrameworkCore.Migrations.Operations;

namespace Application.Services.Implementations
{
    public class ProductService : IProductService
    {
        private readonly IProductRepository _productRepository;
        private readonly IUnitOfWork  _unitOfWork;

        public ProductService(
            IProductRepository productRepository,
            IUnitOfWork unitOfWork)
        {
            _productRepository = productRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<ProductResponse> GetByIdAsync(int id, CancellationToken ct = default)
        {
            var product = await _productRepository.GetByIdAsync(ct, id);
            if (product == null) return null;

            return ProductMapper.ToResponse(product);
        }
       
        
        public async Task<IReadOnlyList<ProductResponse>> ListAsync(int skip = 0, int take = 50, CancellationToken ct = default)
        {
            var products = await _productRepository.ListAsync(skip, take, ct);                        

            return ProductMapper.ToResponseList(products);
        }
        
        public async Task<CreateProductResponse> CreateAsync(CreateProductRequest dto, CancellationToken ct = default)
        {
            var normalizedTitle = dto.title.Trim().ToUpperInvariant();
            var exists = await _productRepository.ExistsAsync(p => p.Title.ToUpper() == normalizedTitle, ct);
            if (exists) return ProductMapper.CreateResponse(false,-1);
            var product = ProductMapper.createToEntity(dto, ct);
            _productRepository.Add(product);
            await _unitOfWork.SaveChangesAsync(ct);
            return ProductMapper.CreateResponse(true,product.Id);
            
        }
        public async Task<UpdateProductResponse> UpdateAsync(int id, UpdateProductRequest dto, CancellationToken ct = default)
        {
            bool exists = await _productRepository.ExistsAsync(p => p.Id == id, ct);
            if (!exists) return ProductMapper.UpdateResponse(false);
            var product = ProductMapper.updateToEntity(dto, ct);
            _productRepository.Update(product);
            await _unitOfWork.SaveChangesAsync(ct);
            return ProductMapper.UpdateResponse(true);

        }
        public async Task<DeleteProductResponse> DeleteAsync(int id, CancellationToken ct = default)
        {
            var deleted = await _productRepository.DeleteByIdAsync(ct, id);
            if (!deleted) return ProductMapper.DeleteResponse(false);
            await _unitOfWork.SaveChangesAsync(ct);
            return ProductMapper.DeleteResponse(true);
        }
        public async Task<(IReadOnlyList<ProductResponse> Items, int Total)> ListWithCountAsync(int skip = 0, int take = 50, CancellationToken ct = default)
        {
            var products = await _productRepository.ListAsync(skip, take, ct);
            var count = await _productRepository.CountAsync(null, ct);
            var productResponses = ProductMapper.ToResponseList(products);
            return (productResponses, count);
        }
        public async Task<bool> ExistsByNameAsync(string name, CancellationToken ct = default)
        {
            var normalizedTitle = (name ?? string.Empty).Trim().ToUpperInvariant();
            return await _productRepository.ExistsAsync(p => p.Title.ToUpper() == normalizedTitle, ct);
        }
        public async Task<int> CountAsync(CancellationToken ct = default)
        {
            var count = await _productRepository.CountAsync(null, ct);
            return  count;
        }
    }
}
