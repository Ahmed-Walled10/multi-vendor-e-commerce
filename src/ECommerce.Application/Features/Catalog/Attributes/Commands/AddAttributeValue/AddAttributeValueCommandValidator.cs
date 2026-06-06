using FluentValidation;

namespace ECommerce.Application.Features.Catalog.Attributes.Commands.AddAttributeValue;

public class AddAttributeValueCommandValidator : AbstractValidator<AddAttributeValueCommand>
{
    public AddAttributeValueCommandValidator()
    {
        RuleFor(x => x.Value)
            .NotEmpty().WithMessage("Attribute value is required.")
            .MaximumLength(200).WithMessage("Attribute value must not exceed 200 characters.");
    }
}
