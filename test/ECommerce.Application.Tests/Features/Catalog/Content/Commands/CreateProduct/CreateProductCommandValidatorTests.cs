using ECommerce.Application.Features.Catalog.Content.Commands.CreateProduct;
using Xunit;

namespace ECommerce.Application.Tests.Features.Catalog.Content.Commands.CreateProduct;

public class CreateProductCommandValidatorTests
{
    private readonly CreateProductCommandValidator _validator;

    public CreateProductCommandValidatorTests()
    {
        _validator = new CreateProductCommandValidator();
    }

    [Fact]
    public void Validate_WithValidCommand_ShouldSucceed()
    {
        // Arrange
        var command = new CreateProductCommand
        {
            Name = "Valid Product Name",
            BasePrice = 10.99m,
            Description = "A valid product description",
            Status = "Active"
        };

        // Act
        var result = _validator.Validate(command);

        // Assert
        Assert.True(result.IsValid);
        Assert.Empty(result.Errors);
    }

    [Fact]
    public void Validate_WithEmptyName_ShouldFail()
    {
        // Arrange
        var command = new CreateProductCommand
        {
            Name = "",
            BasePrice = 10.99m
        };

        // Act
        var result = _validator.Validate(command);

        // Assert
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == "Name" && e.ErrorMessage.Contains("Product name is required"));
    }

    [Fact]
    public void Validate_WithNegativePrice_ShouldFail()
    {
        // Arrange
        var command = new CreateProductCommand
        {
            Name = "Product",
            BasePrice = -1.00m
        };

        // Act
        var result = _validator.Validate(command);

        // Assert
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == "BasePrice" && e.ErrorMessage.Contains("Base price must be zero or positive"));
    }

    [Fact]
    public void Validate_WithInvalidStatus_ShouldFail()
    {
        // Arrange
        var command = new CreateProductCommand
        {
            Name = "Product",
            BasePrice = 10.99m,
            Status = "InvalidStatusName"
        };

        // Act
        var result = _validator.Validate(command);

        // Assert
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == "Status" && e.ErrorMessage.Contains("Status must be Draft, Active, or Archived"));
    }
}
