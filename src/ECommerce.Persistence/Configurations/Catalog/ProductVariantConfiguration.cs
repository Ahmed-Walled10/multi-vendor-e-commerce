using ECommerce.Domain.Entities.Catalog;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ECommerce.Persistence.Configurations.Catalog;

public class ProductVariantConfiguration : IEntityTypeConfiguration<ProductVariant>
{
    public void Configure(EntityTypeBuilder<ProductVariant> builder)
    {
        builder.HasKey(v => v.Id);

        builder.Property(v => v.SKU)
            .IsRequired()
            .HasMaxLength(100);

        builder.HasIndex(v => v.SKU)
            .IsUnique();

        builder.Property(v => v.PriceOverride)
            .HasColumnType("decimal(18,2)");

        builder.HasIndex(v => v.ProductId);

        builder.HasMany(v => v.VariantAttributeValues)
            .WithOne(vav => vav.Variant)
            .HasForeignKey(vav => vav.VariantId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
