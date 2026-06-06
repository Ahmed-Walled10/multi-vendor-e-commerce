using ECommerce.Application.Features.Catalog.Variants.DTOs;
using MediatR;

namespace ECommerce.Application.Features.Catalog.Variants.Commands.UpdateVariant;

public class UpdateVariantCommand : IRequest<VariantDto>
{
    /// <summary>Set from route parameter.</summary>
    public Guid ProductId { get; set; }

    /// <summary>Set from route parameter.</summary>
    public Guid Id { get; set; }

    public string SKU { get; set; } = default!;
    public int Quantity { get; set; }
    public decimal? PriceOverride { get; set; }
    public bool IsActive { get; set; }

    /// <summary>
    /// Updated list of attribute value IDs. Replaces the existing combination.
    /// </summary>
    public List<Guid> AttributeValueIds { get; set; } = new();
}
