using ECommerce.Application.Features.Catalog.Content.DTOs;
using MediatR;

namespace ECommerce.Application.Features.Catalog.Content.Queries.GetProductById;

public class GetProductByIdQuery : IRequest<ProductDto>
{
    public Guid Id { get; set; }
}
