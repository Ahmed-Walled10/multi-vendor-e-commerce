namespace ECommerce.Domain.Entities.Catalog;

public class ProductVariantAttributeValue
{
    public Guid VariantId { get; set; }
    public ProductVariant Variant { get; set; } = default!;


    public Guid AttributeValueId { get; set; }
    public ProductAttributeValue AttributeValue { get; set; } = default!;
}
