using Application.Abstraction;
using Application.Abstractions;
using Application.Exceptions;
using Application.Services.Implementations;
using Data.Entities;
using General.Dto.Inventory;
using General.Dto.Product;
using General.Mappers;
using Microsoft.VisualStudio.TestPlatform.ObjectModel;
using NSubstitute;
using System;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace UnitTests.ProductServices
{

    public class UpdateAsyncTests
    {
        private readonly IProductRepository _productRepository = Substitute.For<IProductRepository>();
        private readonly ICategoryRepository _categoryRepository = Substitute.For<ICategoryRepository>();
        private readonly IUnitOfWork _unitOfWork = Substitute.For<IUnitOfWork>();
        private readonly ProductService _sut;

        public UpdateAsyncTests()
        {
            _sut = new ProductService(_productRepository, _categoryRepository, _unitOfWork);
        }

        [Fact]
        public async Task UpdateAsync_ShouldUpdateProduct_WhenValid()
        {
            // Arrange
            var ct = CancellationToken.None;
            var dto = new UpdateProductRequest
            {
                Title = "Updated Name",
                Category = "Electronics",
                Description = "Updated Description",
                ImageUrl = "http://example.com/updated.jpg",
                Price = 1500.00m,
                RatingRate = 9.0,
                Inventory = new UpdateInventoryRequest
                {
                    Total = 10,
                    Available = 5
                }
            };

            var existingProduct = new Product
            {
                Id = 1,
                Title = "Old Name",
                Price = 1200.00m,
                ImageUrl = "http://example.com/old.jpg",
                Description = "Old Description",
                RatingRate = 8.0,
                CategoryId = 5,
                Inventory = new Inventory { Total = 8, Available = 4 }
            };

            var category = new Category { Id = 5, Name = "Electronics" };
            var expectedResponse = new ProductResponse { Id = 1, Title = "Updated Name", Category = "Electronics", Price = 1500.00m, ImageUrl = "http://example.com/updated.jpg" , Description = "Updated Description"};

            _productRepository.GetByIdAsync(ct, existingProduct.Id).Returns(existingProduct);
            _categoryRepository.GetAsync(
                Arg.Any<System.Linq.Expressions.Expression<Func<Category, bool>>>(),
                true,
                ct
            ).Returns(category);            

            // Act
            var result = await _sut.UpdateAsync(existingProduct.Id, dto, ct);

            // Assert
            _productRepository.Received(1).Update(existingProduct);
            _unitOfWork.Received(1).SaveChangesAsync(ct);
            Assert.Equal(expectedResponse.Id, result.Id);
            Assert.Equal(expectedResponse.Title, result.Title);
            Assert.Equal(expectedResponse.Category, result.Category);
        }

        
        //Case 1: DTO is null
        [Fact]
        public async Task UpdateAsync_ShouldThrowArgumentNullException_WhenDtoIsNull()
        {
            // Arrange
            UpdateProductRequest dto = null;
            CancellationToken ct = CancellationToken.None;

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentNullException>(() => _sut.UpdateAsync(1, dto, ct));
        }
        

        //Case 1 (continued): Product not found
        [Fact]
        public async Task UpdateAsync_ShouldThrowNotFoundException_WhenProductDoesNotExist()
        {
            // Arrange
            var ct = CancellationToken.None;
            var dto = new UpdateProductRequest
            {
                Title = "Updated Name",
                Category = "Electronics",
                Description = "Updated Description",
                ImageUrl = "http://example.com/updated.jpg",
                Price = 1500.00m,
                RatingRate = 9.0,
                Inventory = new UpdateInventoryRequest
                {
                    Total = 10,
                    Available = 5
                }
            };

            var category = new Category { Id = 5, Name = "Electronics" };

            _productRepository.GetByIdAsync(Arg.Any<CancellationToken>(), Arg.Any<int>()).Returns((Product)null);
            _categoryRepository.GetAsync(
                Arg.Any<System.Linq.Expressions.Expression<Func<Category, bool>>>(),
                true,
                ct
            ).Returns(category);

            // Act & Assert
            await Assert.ThrowsAsync<NotFoundException>(() => _sut.UpdateAsync(99, dto, ct));
        }
        
        //Case 2: Category does not exist
        [Fact]
        public async Task UpdateAsync_ShouldThrowNotFoundException_WhenCategoryNotFound()
        {
            // Arrange
            var ct = CancellationToken.None;
            var dto = new UpdateProductRequest
            {
                Title = "Updated Name",
                Category = "Electronics",
                Description = "Updated Description",
                ImageUrl = "http://example.com/updated.jpg",
                Price = 1500.00m,
                RatingRate = 9.0,
                Inventory = new UpdateInventoryRequest
                {
                    Total = 10,
                    Available = 5
                }
            };

            var existingProduct = new Product
            {
                Id = 1,
                Title = "Old Name",
                Price = 1200.00m,
                ImageUrl = "http://example.com/old.jpg",
                Description = "Old Description",
                RatingRate = 8.0,
                CategoryId = 5,
                Inventory = new Inventory { Total = 8, Available = 4 }
            };

            _productRepository.GetByIdAsync(Arg.Any<CancellationToken>(), Arg.Any<int>()).Returns(existingProduct);
            _categoryRepository.GetAsync(
                Arg.Any<System.Linq.Expressions.Expression<Func<Category, bool>>>(),
                true,
                Arg.Any<CancellationToken>()
            ).Returns((Category)null);

            // Act & Assert
            await Assert.ThrowsAsync<NotFoundException>(() => _sut.UpdateAsync(existingProduct.Id, dto, ct));
        }

        //Case 3: Inventory available > total
        [Fact]
        public async Task UpdateAsync_ShouldThrowBadRequestException_WhenAvailableGreaterThanTotal()
        {
            // Arrange
            var ct = CancellationToken.None;
            var dto = new UpdateProductRequest
            {
                Title = "Updated Name",
                Category = "Electronics",
                Description = "Updated Description",
                ImageUrl = "http://example.com/updated.jpg",
                Price = 1500.00m,
                RatingRate = 9.0,
                Inventory = new UpdateInventoryRequest
                {
                    Total = 10,
                    Available = 15
                }
            };

            var existingProduct = new Product
            {
                Id = 1,
                Title = "Old Name",
                Price = 1200.00m,
                ImageUrl = "http://example.com/old.jpg",
                Description = "Old Description",
                RatingRate = 8.0,
                CategoryId = 5,
                Inventory = new Inventory { Total = 8, Available = 4 }
            };

            var category = new Category { Id = 5, Name = "Electronics" };

            _productRepository.GetByIdAsync(ct, existingProduct.Id).Returns(existingProduct);
            _categoryRepository.GetAsync(
                Arg.Any<System.Linq.Expressions.Expression<Func<Category, bool>>>(),
                true,
                ct
            ).Returns(category);

            // Act & Assert
            await Assert.ThrowsAsync<BadRequestException>(() => _sut.UpdateAsync(existingProduct.Id, dto));
        }
        
        // Case 4: Product has inventory but new available > total
        [Fact]
        public async Task UpdateAsync_ShouldThrowBadRequestException_WhenExistingInventoryBecomesInvalid()
        {
            // Arrange
            var ct = CancellationToken.None;
            var dto = new UpdateProductRequest
            {
                Title = "Updated Name",
                Category = "Electronics",
                Description = "Updated Description",
                ImageUrl = "http://example.com/updated.jpg",
                Price = 1500.00m,
                RatingRate = 9.0,
                Inventory = new UpdateInventoryRequest
                {
                    Total = 5,
                    Available = 9
                }
            };

            var existingProduct = new Product
            {
                Id = 1,
                Title = "Old Name",
                Price = 1200.00m,
                ImageUrl = "http://example.com/old.jpg",
                Description = "Old Description",
                RatingRate = 8.0,
                CategoryId = 5,
                Inventory = (Inventory)null
            };

            var category = new Category { Id = 5, Name = "Electronics" };
            var expectedResponse = new ProductResponse { Id = 1, Title = "Updated Name", Category = "Electronics", Price = 1500.00m, ImageUrl = "http://example.com/updated.jpg", Description = "Updated Description" };

            _productRepository.GetByIdAsync(ct, existingProduct.Id).Returns(existingProduct);
            _categoryRepository.GetAsync(
                Arg.Any<System.Linq.Expressions.Expression<Func<Category, bool>>>(),
                true,
                ct
            ).Returns(category);

            // Act & Assert
            await Assert.ThrowsAsync<BadRequestException>(() => _sut.UpdateAsync(existingProduct.Id, dto));
        }
        
    }

}
