using FluentValidation;

namespace ECommerce.Application.Features.Catalog.Attributes.Commands.AddAttribute;

public class AddAttributeCommandValidator : AbstractValidator<AddAttributeCommand>
{
    public AddAttributeCommandValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Attribute name is required.")
            .MaximumLength(100).WithMessage("Attribute name must not exceed 100 characters.");
    }
}
