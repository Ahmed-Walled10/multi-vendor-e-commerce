using ECommerce.Application.Features.Catalog.Variants.DTOs;
using MediatR;

namespace ECommerce.Application.Features.Catalog.Variants.Queries.GetVariantById;

public class GetVariantByIdQuery : IRequest<VariantDto>
{
    public Guid ProductId { get; set; }
    public Guid Id { get; set; }
}
