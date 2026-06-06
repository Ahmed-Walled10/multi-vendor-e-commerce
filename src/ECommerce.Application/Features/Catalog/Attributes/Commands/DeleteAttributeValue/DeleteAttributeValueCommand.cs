using MediatR;

namespace ECommerce.Application.Features.Catalog.Attributes.Commands.DeleteAttributeValue;

public class DeleteAttributeValueCommand : IRequest<Unit>
{
    public Guid ProductId { get; set; }
    public Guid AttributeId { get; set; }
    public Guid ValueId { get; set; }
}
