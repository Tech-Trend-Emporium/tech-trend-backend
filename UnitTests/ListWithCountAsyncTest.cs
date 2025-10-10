using Application.Abstraction;
using Application.Abstractions;
using Application.Services.Implementations;
using Data.Entities;
using General.Dto.Product;
using General.Mappers;
using Microsoft.EntityFrameworkCore.Storage;
using NSubstitute;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace UnitTests.ProductServices
{
    

    public class ListWithCountAsyncTest
    {
        private readonly IProductRepository _productRepository = Substitute.For<IProductRepository>();
        private readonly ICategoryRepository _categoryRepository = Substitute.For<ICategoryRepository>();
        private readonly IUnitOfWork _unitOfWork = Substitute.For<IUnitOfWork>();
        private readonly ProductService _sut;

        public ListWithCountAsyncTest()
        {
            _sut = new ProductService(_productRepository, _categoryRepository, _unitOfWork);
        }

        [Fact]
        public async Task ListWithCountAsync_ShouldReturnProductsAndTotal_WhenCategoryIsNull()
        {
            // Arrange
            var ct = CancellationToken.None;
            int skip = 0;
            int take = 50;
            string? category = null;

            var products = new List<Product>
            {
                new Product { Id = 1, Title = "Laptop", CategoryId = 10, Price = 123.43m },
                new Product { Id = 2, Title= "Mouse", CategoryId = 20, Price = 123.43m },
                new Product { Id = 3, Title = "Keyboard", CategoryId = 10, Price = 12.43m },
                new Product { Id = 4, Title = "Monitor", CategoryId = 20, Price = 223.43m },
                new Product { Id = 5, Title = "Printer", CategoryId = 10, Price = 300m }
            };

            var categories = new List<Category>
            {
                new Category { Id = 10, Name = "Electronics" },
                new Category { Id = 20, Name = "Accessories" }
            };


            (List<ProductResponse>, int) expectedResponse = (items : new List<ProductResponse>
            {
                new ProductResponse { Id = 1, Title = "Laptop", Category = "Electronics", Price = 123.43m },
                new ProductResponse { Id = 2, Title= "Mouse", Category = "Accessories", Price = 123.43m },
                new ProductResponse { Id = 3, Title = "Keyboard", Category = "Electronics", Price = 12.43m },
                new ProductResponse { Id = 4, Title = "Monitor", Category = "Accessories", Price = 223.43m },
                new ProductResponse { Id = 5, Title = "Printer", Category = "Electronics", Price = 300m }
            }, 
            
            total : products.Count);

            _productRepository.ListAsync(0, 50, null, ct).Returns(products);
            _productRepository.CountAsync(null, ct).Returns(products.Count);
            _categoryRepository.ListByIdsAsync(ct, Arg.Any<List<int>>()).Returns(categories);

            

            // Act
            var result = await _sut.ListWithCountAsync(skip, take, category, ct);

            // Assert
            Assert.NotNull(result.Items);
            Assert.Equal(expectedResponse.Item1.Count, result.Items.Count);
            Assert.Equal(expectedResponse.Item1[0].Title, result.Items[0].Title);
            Assert.Equal(expectedResponse.Item1[1].Category, result.Items[1].Category);

        }

        [Fact]
        public async Task ListWithCountAsync_ShouldFilterByCategory_WhenCategoryExists()
        {
            // Arrange
            var ct = CancellationToken.None;
            var category = new Category { Id = 99, Name = "Electronics" };

            _categoryRepository.GetAsync(
                Arg.Any<System.Linq.Expressions.Expression<Func<Category, bool>>>(),
                true,
                ct
            ).Returns(category);

            var products = new List<Product>
        {
            new Product { Id = 1, Title = "TV", CategoryId = 99 },
            new Product { Id = 2, Title = "Radio", CategoryId = 99 }
        };

            var categories = new List<Category> { category };
            var expectedResponses = new List<ProductResponse>
        {
            new ProductResponse { Id = 1, Title = "TV", Category = "Electronics" },
            new ProductResponse { Id = 2, Title = "Radio", Category = "Electronics" }
        };

            _productRepository.ListAsync(0, 50, category.Id, ct).Returns(products);
            _productRepository.CountAsync(category.Id, ct).Returns(2);
            _categoryRepository.ListByIdsAsync(ct, Arg.Any<List<int>>()).Returns(categories);

            // Act
            var result = await _sut.ListWithCountAsync(0, 50, "Electronics", ct);

            // Assert
            Assert.Equal(2, result.Total);
            Assert.Equal("TV", result.Items.First().Title);

        }

        [Fact]
        public async Task ListWithCountAsync_ShouldReturnEmpty_WhenNoProductsFound()
        {
            // Arrange
            var ct = CancellationToken.None;

            _productRepository.ListAsync(0, 50, null, ct).Returns(new List<Product>());
            _productRepository.CountAsync(null, ct).Returns(0);
            _categoryRepository.ListByIdsAsync(ct, Arg.Any<List<int>>()).Returns(new List<Category>());

            // Act
            var result = await _sut.ListWithCountAsync(ct: ct);

            // Assert
            Assert.Empty(result.Items);
            Assert.Equal(0, result.Total);

            await _productRepository.Received(1).ListAsync(0, 50, null, ct);
            await _productRepository.Received(1).CountAsync(null, ct);
        }
        
    }

}
