using ECommerce.Domain.Common.Enums;
using ECommerce.Domain.Common.Primitives;
using ECommerce.Domain.Entities.Identity;

namespace ECommerce.Domain.Entities.Catalog;

public class Product : BaseEntity
{
    public string Name { get; set; } = default!;
    public string? Description { get; set; }
    public ProductStatus Status { get; set; } = ProductStatus.Draft;
    public decimal BasePrice { get; set; }



    public string UserId { get; set; } = default!;
    public ApplicationUser User { get; set; } = default!;

    public ICollection<ProductAttribute> Attributes { get; set; } = new List<ProductAttribute>();
    public ICollection<ProductVariant> Variants { get; set; } = new List<ProductVariant>();
}
