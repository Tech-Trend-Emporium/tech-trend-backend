using Application.Abstraction;
using Application.Abstractions;
using Application.Services.Implementations;
using Data.Entities;
using NSubstitute;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace UnitTests.InventoryServices
{
    

    public class ListWithCountTest
    {
        private readonly IInventoryRepository _inventoryRepository = Substitute.For<IInventoryRepository>();
        private readonly IProductRepository _productRepository = Substitute.For<IProductRepository>();
        private readonly IUnitOfWork _unitOfWork = Substitute.For<IUnitOfWork>();

        private readonly InventoryService _sut;

        public ListWithCountTest()
        {
            _sut = new InventoryService(_inventoryRepository, _productRepository, _unitOfWork);
        }

        [Fact]
        public async Task ListWithCountAsync_ShouldReturnInventoriesAndTotal()
        {
            // Arrange
            var ct = CancellationToken.None;
            int skip = 0;
            int take = 50;

            var inventories = new List<Inventory>
            {
                new() { Id = 1, ProductId = 10, Total = 5, Available = 2 },
                new() { Id = 2, ProductId = 20, Total = 15, Available = 2 }
            };

            var products = new List<Product>
            {
                new() { Id = 10, Title = "Monitor" },
                new() { Id = 20, Title = "Mouse" }
            };

            _inventoryRepository.ListAsync(skip, take, ct).Returns(inventories);
            _inventoryRepository.CountAsync(null, ct).Returns(inventories.Count);
            _productRepository.ListByIdsAsync(ct, Arg.Any<List<int>>()).Returns(products);

            // Act
            var (items, total) = await _sut.ListWithCountAsync();
            
            // Assert
            Assert.Equal(inventories.Count, total);
            Assert.Equal(inventories.Count, items.Count);
            Assert.True(products[0].Title == items[0].ProductName);
            Assert.True(inventories[0].Available == items[0].Available);
        }

        [Fact]
        public async Task ListWithCountAsync_ShouldReturnEmptyList_WhenNoInventoriesExist()
        {
            // Arrange
            var ct = CancellationToken.None;
            int skip = 0;
            int take = 50;

            _inventoryRepository.ListAsync(skip, take, ct).Returns(new List<Inventory>());
            _inventoryRepository.CountAsync(null, ct).Returns(0);
            _productRepository.ListByIdsAsync(ct, Arg.Any<List<int>>()).Returns(new List<Product>());

            // Act
            var (items, total) = await _sut.ListWithCountAsync();

            // Assert
            Assert.Equal(0,total);
            Assert.Empty(items);
        }

        [Fact]
        public async Task ListWithCountAsync_ShouldHandleMissingProductNames()
        {
            // Arrange

            var ct = CancellationToken.None;
            int skip = 0;
            int take = 50;

            var inventories = new List<Inventory>
            {
                new() { Id = 1, ProductId = 10, Total = 5, Available = 2 },
                new() { Id = 2, ProductId = 20, Total = 15, Available = 2 }
            };

            var products = new List<Product>
            {
                new() { Id = 10, Title = "Keyboard" } // ProductId 30 missing
            };

            _inventoryRepository.ListAsync(skip, take, ct).Returns(inventories);
            _inventoryRepository.CountAsync(null, ct).Returns(inventories.Count);
            _productRepository.ListByIdsAsync(ct, Arg.Any<List<int>>()).Returns(products);

            // Act
            var (items, total) = await _sut.ListWithCountAsync();

            // Assert
            Assert.Equal(inventories.Count, total);
            Assert.Equal(inventories.Count, items.Count);
            Assert.True(products[0].Title == items[0].ProductName);
            Assert.True(inventories[0].Available == items[0].Available);
            Assert.True(items[1].ProductName == "Unknown");
        }
    }

}
