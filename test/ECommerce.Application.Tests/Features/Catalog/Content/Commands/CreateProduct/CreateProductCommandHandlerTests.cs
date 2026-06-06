using ECommerce.Application.Contracts.Infrastructure;
using ECommerce.Application.Contracts.Persistence;
using ECommerce.Application.Features.Catalog.Content.Commands.CreateProduct;
using ECommerce.Application.Common.Caching;
using ECommerce.Domain.Common.Enums;
using ECommerce.Domain.Entities.Catalog;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace ECommerce.Application.Tests.Features.Catalog.Content.Commands.CreateProduct;

public class CreateProductCommandHandlerTests
{
    private readonly Mock<IProductRepository> _productRepositoryMock;
    private readonly Mock<ICurrentUserService> _currentUserServiceMock;
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly Mock<ILogger<CreateProductCommandHandler>> _loggerMock;
    private readonly Mock<IProductCacheSignal> _cacheSignalMock;
    private readonly CreateProductCommandHandler _handler;

    public CreateProductCommandHandlerTests()
    {
        _productRepositoryMock = new Mock<IProductRepository>();
        _currentUserServiceMock = new Mock<ICurrentUserService>();
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _loggerMock = new Mock<ILogger<CreateProductCommandHandler>>();
        _cacheSignalMock = new Mock<IProductCacheSignal>();

        _handler = new CreateProductCommandHandler(
            _productRepositoryMock.Object,
            _currentUserServiceMock.Object,
            _unitOfWorkMock.Object,
            _loggerMock.Object,
            _cacheSignalMock.Object
        );
    }

    [Fact]
    public async Task Handle_WithAuthenticatedUser_ShouldCreateProductAndReturnDto()
    {
        // Arrange
        var userId = "test-user-id";
        _currentUserServiceMock.Setup(x => x.UserId).Returns(userId);

        var command = new CreateProductCommand
        {
            Name = "New Laptop",
            Description = "Powerful gaming laptop",
            BasePrice = 1499.99m,
            Status = "Active",
            Attributes = new List<CreateProductAttributeDto>
            {
                new()
                {
                    Name = "Color",
                    Values = new List<string> { "Silver", "Black" }
                }
            }
        };

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(command.Name, result.Name);
        Assert.Equal(command.Description, result.Description);
        Assert.Equal(command.BasePrice, result.BasePrice);
        Assert.Equal(command.Status, result.Status);
        Assert.Equal(userId, result.UserId);
        Assert.Single(result.Attributes);
        Assert.Equal("Color", result.Attributes[0].Name);
        Assert.Equal(2, result.Attributes[0].Values.Count);

        _productRepositoryMock.Verify(x => x.AddAsync(It.IsAny<Product>(), It.IsAny<CancellationToken>()), Times.Once);
        _unitOfWorkMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_WithUnauthenticatedUser_ShouldThrowUnauthorizedAccessException()
    {
        // Arrange
        _currentUserServiceMock.Setup(x => x.UserId).Returns((string?)null);

        var command = new CreateProductCommand
        {
            Name = "New Laptop",
            BasePrice = 1499.99m
        };

        // Act & Assert
        await Assert.ThrowsAsync<UnauthorizedAccessException>(() => _handler.Handle(command, CancellationToken.None));
        _productRepositoryMock.Verify(x => x.AddAsync(It.IsAny<Product>(), It.IsAny<CancellationToken>()), Times.Never);
        _unitOfWorkMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
    }
}
