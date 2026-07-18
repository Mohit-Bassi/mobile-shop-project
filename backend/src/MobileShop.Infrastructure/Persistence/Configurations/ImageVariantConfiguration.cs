using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MobileShop.Domain.Entities;

namespace MobileShop.Infrastructure.Persistence.Configurations;

public class ImageVariantConfiguration : IEntityTypeConfiguration<ImageVariant>
{
    public void Configure(EntityTypeBuilder<ImageVariant> builder)
    {
        builder.HasKey(iv => iv.ImageVariantId);
        builder.Property(iv => iv.Data).IsRequired();

        builder.HasIndex(iv => new { iv.ImageId, iv.VariantType }).IsUnique();

        builder.HasOne(iv => iv.Image)
            .WithMany(i => i.Variants)
            .HasForeignKey(iv => iv.ImageId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
