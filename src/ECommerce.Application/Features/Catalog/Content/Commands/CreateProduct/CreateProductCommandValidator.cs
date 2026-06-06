using FluentValidation;

namespace ECommerce.Application.Features.Catalog.Content.Commands.CreateProduct;

public class CreateProductCommandValidator : AbstractValidator<CreateProductCommand>
{
    public CreateProductCommandValidator()
    {
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

        RuleForEach(x => x.Attributes).ChildRules(attr =>
        {
            attr.RuleFor(a => a.Name)
                .NotEmpty().WithMessage("Attribute name is required.")
                .MaximumLength(100).WithMessage("Attribute name must not exceed 100 characters.");

            attr.RuleFor(a => a.Values)
                .NotEmpty().WithMessage("At least one attribute value is required.");

            attr.RuleForEach(a => a.Values)
                .NotEmpty().WithMessage("Attribute value cannot be empty.")
                .MaximumLength(200).WithMessage("Attribute value must not exceed 200 characters.");
        });
    }
}
