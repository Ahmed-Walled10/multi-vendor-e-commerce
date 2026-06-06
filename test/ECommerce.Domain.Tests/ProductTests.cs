using ECommerce.Domain.Common.Enums;
using ECommerce.Domain.Entities.Catalog;
using Xunit;

namespace ECommerce.Domain.Tests;

public class ProductTests
{
    [Fact]
    public void Product_ShouldInitializeWithDefaultValues()
    {
        // Act
        var product = new Product();

        // Assert
        Assert.NotEqual(Guid.Empty, product.Id);
        Assert.Equal(ProductStatus.Draft, product.Status);
        Assert.True(product.CreatedAt <= DateTime.UtcNow);
        Assert.True(product.UpdatedAt <= DateTime.UtcNow);
        Assert.Empty(product.Attributes);
        Assert.Empty(product.Variants);
    }

    [Fact]
    public void Product_ShouldAllowSettingProperties()
    {
        // Arrange
        var id = Guid.NewGuid();
        var userId = "user-123";

        // Act
        var product = new Product
        {
            Id = id,
            Name = "Test Product",
            Description = "Test Description",
            BasePrice = 99.99m,
            Status = ProductStatus.Active,
            UserId = userId
        };

        // Assert
        Assert.Equal(id, product.Id);
        Assert.Equal("Test Product", product.Name);
        Assert.Equal("Test Description", product.Description);
        Assert.Equal(99.99m, product.BasePrice);
        Assert.Equal(ProductStatus.Active, product.Status);
        Assert.Equal(userId, product.UserId);
    }
}
