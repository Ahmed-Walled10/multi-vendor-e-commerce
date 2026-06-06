using ECommerce.Application.Features.Catalog.Variants.DTOs;
using MediatR;

namespace ECommerce.Application.Features.Catalog.Variants.Commands.CreateVariant;

public class CreateVariantCommand : IRequest<VariantDto>
{
    /// <summary>Set from route parameter.</summary>
    public Guid ProductId { get; set; }

    public string SKU { get; set; } = default!;
    public int Quantity { get; set; }
    public decimal? PriceOverride { get; set; }
    public bool IsActive { get; set; } = true;

    /// <summary>
    /// List of attribute value IDs that define this variant's combination.
    /// Example: for a "Red / Small" T-shirt, pass the IDs of the "Red" and "Small" attribute values.
    /// </summary>
    public List<Guid> AttributeValueIds { get; set; } = new();
}
