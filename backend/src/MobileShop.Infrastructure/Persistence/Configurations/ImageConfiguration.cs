using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MobileShop.Domain.Entities;

namespace MobileShop.Infrastructure.Persistence.Configurations;

public class ImageConfiguration : IEntityTypeConfiguration<Image>
{
    public void Configure(EntityTypeBuilder<Image> builder)
    {
        builder.HasKey(i => i.ImageId);
        builder.Property(i => i.ContentType).HasMaxLength(50).IsRequired();
        builder.Property(i => i.CreatedAtUtc).HasDefaultValueSql("SYSUTCDATETIME()");

        builder.HasIndex(i => new { i.OwnerType, i.OwnerId });
    }
}
