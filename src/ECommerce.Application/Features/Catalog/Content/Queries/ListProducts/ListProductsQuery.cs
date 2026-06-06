using ECommerce.Application.Features.Catalog.Content.DTOs;
using ECommerce.Application.ResourceParameters;
using ECommerce.Application.Responses;
using MediatR;

namespace ECommerce.Application.Features.Catalog.Content.Queries.ListProducts;

public class ListProductsQuery : IRequest<PagedResult<ProductListItemDto>>
{
    public ProductResourceParameters Parameters { get; set; } = new();
}
