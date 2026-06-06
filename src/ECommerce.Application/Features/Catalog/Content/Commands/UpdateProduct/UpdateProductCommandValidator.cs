using FluentValidation;

namespace ECommerce.Application.Features.Catalog.Content.Commands.UpdateProduct;

public class UpdateProductCommandValidator : AbstractValidator<UpdateProductCommand>
{
    public UpdateProductCommandValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty().WithMessage("Product ID is required.");

        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Product name is required.")
            .MaximumLength(300).WithMessage("Product name must not exceed 300 characters.");

        RuleFor(x => x.BasePrice)
            .GreaterThanOrEqualTo(0).WithMessage("Base price must be zero or positive.");

        RuleFor(x => x.Description)
            .MaximumLength(2000).WithMessage("Description must not exceed 2000 characters.");

        RuleFor(x => x.Status)
            .Must(s => string.IsNullOrEmpty(s) || s.Equals("Draft", StringComparison.OrdinalIgnoreCase)
                                                || s.Equals("Active", StringComparison.OrdinalIgnoreCase)
                                                || s.Equals("Archived", StringComparison.OrdinalIgnoreCase))
            .WithMessage("Status must be Draft, Active, or Archived.");
    }
}
