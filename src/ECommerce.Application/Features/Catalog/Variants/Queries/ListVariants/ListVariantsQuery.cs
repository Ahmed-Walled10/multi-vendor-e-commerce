using ECommerce.Application.Features.Catalog.Variants.DTOs;
using ECommerce.Application.ResourceParameters;
using ECommerce.Application.Responses;
using MediatR;

namespace ECommerce.Application.Features.Catalog.Variants.Queries.ListVariants;

public class ListVariantsQuery : IRequest<PagedResult<VariantDto>>
{
    public Guid ProductId { get; set; }
    public VariantResourceParameters Parameters { get; set; } = new();
}
