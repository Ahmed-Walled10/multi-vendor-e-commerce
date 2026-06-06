using ECommerce.Application.Features.Catalog.Attributes.DTOs;
using MediatR;

namespace ECommerce.Application.Features.Catalog.Attributes.Commands.UpdateAttribute;

public class UpdateAttributeCommand : IRequest<AttributeDto>
{
    public Guid ProductId { get; set; }
    public Guid AttributeId { get; set; }
    public string Name { get; set; } = default!;
}
