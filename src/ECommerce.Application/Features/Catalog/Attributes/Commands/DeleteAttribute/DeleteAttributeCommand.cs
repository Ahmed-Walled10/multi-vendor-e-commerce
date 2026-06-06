using MediatR;

namespace ECommerce.Application.Features.Catalog.Attributes.Commands.DeleteAttribute;

public class DeleteAttributeCommand : IRequest<Unit>
{
    public Guid ProductId { get; set; }
    public Guid AttributeId { get; set; }
}
