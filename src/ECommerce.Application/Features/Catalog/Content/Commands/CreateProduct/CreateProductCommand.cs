using ECommerce.Application.Features.Catalog.Content.DTOs;
using MediatR;

namespace ECommerce.Application.Features.Catalog.Content.Commands.CreateProduct;

public class CreateProductCommand : IRequest<ProductDto>
{
    public string Name { get; set; } = default!;
    public string? Description { get; set; }
    public decimal BasePrice { get; set; }
    public string? Status { get; set; }

    public List<CreateProductAttributeDto>? Attributes { get; set; }
}

public class CreateProductAttributeDto
{
    public string Name { get; set; } = default!;
    public List<string> Values { get; set; } = new();
}
