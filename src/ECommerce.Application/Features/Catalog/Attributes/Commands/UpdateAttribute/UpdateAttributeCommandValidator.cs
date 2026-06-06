using FluentValidation;

namespace ECommerce.Application.Features.Catalog.Attributes.Commands.UpdateAttribute;

public class UpdateAttributeCommandValidator : AbstractValidator<UpdateAttributeCommand>
{
    public UpdateAttributeCommandValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Attribute name is required.")
            .MaximumLength(100).WithMessage("Attribute name must not exceed 100 characters.");
    }
}
