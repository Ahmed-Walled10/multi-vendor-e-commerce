namespace ECommerce.Application.Features.Catalog.Variants.DTOs;

public class VariantDto
{
    public Guid Id { get; set; }
    public Guid ProductId { get; set; }
    public string SKU { get; set; } = default!;
    public int Quantity { get; set; }
    public decimal? PriceOverride { get; set; }
    public bool IsActive { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }

    /// <summary>
    /// The attribute combination this variant represents.
    /// Example: [{ AttributeName: "Color", Value: "Red" }, { AttributeName: "Size", Value: "Small" }]
    /// </summary>
    public List<VariantAttributeValueDto> AttributeValues { get; set; } = new();
}

public class VariantAttributeValueDto
{
    public Guid AttributeId { get; set; }
    public string AttributeName { get; set; } = default!;
    public Guid AttributeValueId { get; set; }
    public string Value { get; set; } = default!;
}
