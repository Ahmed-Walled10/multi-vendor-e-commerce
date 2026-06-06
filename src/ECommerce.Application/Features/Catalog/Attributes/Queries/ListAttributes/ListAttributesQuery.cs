using ECommerce.Application.Features.Catalog.Attributes.DTOs;
using MediatR;

namespace ECommerce.Application.Features.Catalog.Attributes.Queries.ListAttributes;

public class ListAttributesQuery : IRequest<List<AttributeDto>>
{
    public Guid ProductId { get; set; }
}
