using ECommerce.Domain.Entities.Catalog;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ECommerce.Persistence.Configurations.Catalog;

public class ProductAttributeValueConfiguration : IEntityTypeConfiguration<ProductAttributeValue>
{
    public void Configure(EntityTypeBuilder<ProductAttributeValue> builder)
    {
        builder.HasKey(v => v.Id);

        builder.Property(v => v.Value)
            .IsRequired()
            .HasMaxLength(200);

        builder.HasIndex(v => v.AttributeId);

        builder.HasMany(v => v.VariantAttributeValues)
            .WithOne(vav => vav.AttributeValue)
            .HasForeignKey(vav => vav.AttributeValueId)
            .OnDelete(DeleteBehavior.NoAction); 
    }
}
