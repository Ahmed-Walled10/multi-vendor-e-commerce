using ECommerce.Domain.Common.Primitives;

namespace ECommerce.Domain.Entities.Catalog;

public class ProductVariant : BaseEntity
{
    public string SKU { get; set; } = default!;
    public int Quantity { get; set; }
    public decimal? PriceOverride { get; set; }
    public bool IsActive { get; set; } = true;


    public Guid ProductId { get; set; }
    public Product Product { get; set; } = default!;

    public ICollection<ProductVariantAttributeValue> VariantAttributeValues { get; set; } = new List<ProductVariantAttributeValue>();
}
