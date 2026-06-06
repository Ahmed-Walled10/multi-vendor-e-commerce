using ECommerce.Domain.Common.Primitives;

namespace ECommerce.Domain.Entities.Catalog;

public class ProductAttribute : BaseEntity
{
    public string Name { get; set; } = default!;



    public Guid ProductId { get; set; }
    public Product Product { get; set; } = default!;

    public ICollection<ProductAttributeValue> Values { get; set; } = new List<ProductAttributeValue>();
}
