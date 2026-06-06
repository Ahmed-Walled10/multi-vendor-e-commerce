using ECommerce.Domain.Entities.Catalog;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ECommerce.Persistence.Configurations.Catalog;

public class ProductVariantAttributeValueConfiguration : IEntityTypeConfiguration<ProductVariantAttributeValue>
{
    public void Configure(EntityTypeBuilder<ProductVariantAttributeValue> builder)
    {
        // Composite primary key
        builder.HasKey(vav => new { vav.VariantId, vav.AttributeValueId });

        builder.HasIndex(vav => vav.VariantId);
        builder.HasIndex(vav => vav.AttributeValueId);
    }
}
