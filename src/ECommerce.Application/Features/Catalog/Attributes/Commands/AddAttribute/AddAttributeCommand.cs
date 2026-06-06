using ECommerce.Application.Features.Catalog.Attributes.DTOs;
using MediatR;

namespace ECommerce.Application.Features.Catalog.Attributes.Commands.AddAttribute;

public class AddAttributeCommand : IRequest<AttributeDto>
{
    public Guid ProductId { get; set; }
    public string Name { get; set; } = default!;
}
