using ECommerce.Application.Features.Catalog.Attributes.DTOs;
using MediatR;

namespace ECommerce.Application.Features.Catalog.Attributes.Commands.AddAttributeValue;

public class AddAttributeValueCommand : IRequest<AttributeValueDto>
{
    public Guid ProductId { get; set; }
    public Guid AttributeId { get; set; }
    public string Value { get; set; } = default!;
}
