using ECommerce.Domain.Common.Primitives;

namespace ECommerce.Domain.Entities.Catalog;

public class ProductAttributeValue : BaseEntity
{
    public string Value { get; set; } = default!;



    public Guid AttributeId { get; set; }
    public ProductAttribute Attribute { get; set; } = default!;
    public ICollection<ProductVariantAttributeValue> VariantAttributeValues { get; set; } = new List<ProductVariantAttributeValue>();
}
