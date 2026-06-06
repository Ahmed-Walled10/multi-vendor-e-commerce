using FluentValidation;

namespace ECommerce.Application.Features.Catalog.Variants.Commands.CreateVariant;

public class CreateVariantCommandValidator : AbstractValidator<CreateVariantCommand>
{
    public CreateVariantCommandValidator()
    {
        RuleFor(x => x.ProductId)
            .NotEmpty().WithMessage("Product ID is required.");

        RuleFor(x => x.SKU)
            .NotEmpty().WithMessage("SKU is required.")
            .MaximumLength(100).WithMessage("SKU must not exceed 100 characters.");

        RuleFor(x => x.Quantity)
            .GreaterThanOrEqualTo(0).WithMessage("Quantity must be zero or positive.");

        RuleFor(x => x.PriceOverride)
            .GreaterThanOrEqualTo(0).When(x => x.PriceOverride.HasValue)
            .WithMessage("Price override must be zero or positive.");

        RuleFor(x => x.AttributeValueIds)
            .NotEmpty().WithMessage("At least one attribute value is required.");
    }
}
