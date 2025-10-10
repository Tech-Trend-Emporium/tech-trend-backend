using Application.Abstraction;
using Application.Abstractions;
using Application.Services.Implementations;
using Data.Entities;
using General.Dto.Product;
using General.Mappers;
using NSubstitute;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

public class GetByIdAsyncTest
{
    private readonly IProductRepository _productRepository = Substitute.For<IProductRepository>();
    private readonly ICategoryRepository _categoryRepository = Substitute.For<ICategoryRepository>();
    private readonly IUnitOfWork _unitOfWork = Substitute.For<IUnitOfWork>();
    private readonly ProductService _sut;

    public GetByIdAsyncTest()
    {
        _sut = new ProductService(_productRepository, _categoryRepository, _unitOfWork);
    }

    [Fact]
    public async Task GetByIdAsync_ShouldReturnProductResponse_WhenProductExists()
    {
        // Arrange
        var ct = CancellationToken.None;
        var productId = 1;
        var product = new Product { Id = productId, Title = "Laptop", Price = 1200.76M,CategoryId = 10, Description = "  ", ImageUrl = " ", RatingRate = 8.5, Count = 12, CreatedAt = DateTime.UtcNow };
        var category = new Category { Id = 10, Name = "Electronics", CreatedAt = DateTime.UtcNow };

        _productRepository.GetByIdAsync(ct, productId).Returns(product);
        _categoryRepository.GetByIdAsync(ct, product.CategoryId).Returns(category);
        /*
        var expectedResponse = new ProductResponse
        {
            Id = product.Id,
            Title = product.Title,
            Price = product.Price,
            Description = product.Description,
            ImageUrl = product.ImageUrl,
            RatingRate = product.RatingRate,
            Count = product.Count,
            Category = category.Name,
            CreatedAt = product.CreatedAt,
            UpdatedAt = product.UpdatedAt
        };
        */
        //ProductMapper.ToResponse(product, category.Name).Returns(expectedResponse);
        var a_response = ProductMapper.ToResponse(product, category.Name);
        // Act
        var result = await _sut.GetByIdAsync(productId, ct);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(a_response.Id, result.Id);
        Assert.Equal(a_response.Title, result.Title);
        Assert.Equal(a_response.Category, result.Category);
    }

    [Fact]
    public async Task GetByIdAsync_ShouldReturnNull_WhenProductDoesNotExist()
    {
        // Arrange
        var ct = CancellationToken.None;
        var productId = 2;

        _productRepository.GetByIdAsync(ct, productId).Returns((Product)null);

        // Act
        var result = await _sut.GetByIdAsync(productId, ct);

        // Assert
        Assert.Null(result);
    }
}
