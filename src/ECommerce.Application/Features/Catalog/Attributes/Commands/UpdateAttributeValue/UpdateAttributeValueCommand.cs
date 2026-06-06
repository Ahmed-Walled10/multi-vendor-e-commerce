using ECommerce.Application.Features.Catalog.Attributes.DTOs;
using MediatR;

namespace ECommerce.Application.Features.Catalog.Attributes.Commands.UpdateAttributeValue;

public class UpdateAttributeValueCommand : IRequest<AttributeValueDto>
{
    public Guid ProductId { get; set; }
    public Guid AttributeId { get; set; }
    public Guid ValueId { get; set; }
    public string Value { get; set; } = default!;
}
