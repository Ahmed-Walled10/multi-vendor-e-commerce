using ECommerce.Domain.Entities.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ECommerce.Persistence.Configurations.Identity;

public class RefreshTokenConfiguration : IEntityTypeConfiguration<RefreshToken>
{
    public void Configure(EntityTypeBuilder<RefreshToken> builder)
    {
        builder.HasKey(t => t.Id);

        builder.Property(t => t.UserId)
               .IsRequired()
               .HasMaxLength(450);    // IdentityUser.Id is nvarchar(450)

        builder.Property(t => t.TokenHash)
               .IsRequired()
               .HasMaxLength(64);    // SHA-256 hex = 64 chars

        builder.HasIndex(t => t.TokenHash)
               .IsUnique();           // Fast lookup + guaranteed uniqueness

        builder.Property(t => t.DeviceInfo)
               .HasMaxLength(512);

        builder.Property(t => t.IpAddress)
               .HasMaxLength(45);     // IPv6 max length

        builder.Property(t => t.ReplacedByTokenHash)
               .HasMaxLength(64);

        // Navigation
        builder.HasOne(t => t.User)
               .WithMany(u => u.RefreshTokens)
               .HasForeignKey(t => t.UserId)
               .OnDelete(DeleteBehavior.Cascade);

        builder.ToTable("RefreshTokens");
    }
}
