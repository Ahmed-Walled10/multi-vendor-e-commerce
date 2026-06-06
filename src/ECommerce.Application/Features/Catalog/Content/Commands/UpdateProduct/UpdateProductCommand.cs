using ECommerce.Application.Features.Catalog.Content.DTOs;
using MediatR;

namespace ECommerce.Application.Features.Catalog.Content.Commands.UpdateProduct;

public class UpdateProductCommand : IRequest<ProductDto>
{
    /// <summary>Set from route parameter in the controller.</summary>
    public Guid Id { get; set; }
    public string Name { get; set; } = default!;
    public string? Description { get; set; }
    public decimal BasePrice { get; set; }
    public string? Status { get; set; }
}
