using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MobileShop.Domain.Entities;

namespace MobileShop.Infrastructure.Persistence.Configurations;

public class AccessoryCompatibleMobileConfiguration : IEntityTypeConfiguration<AccessoryCompatibleMobile>
{
    public void Configure(EntityTypeBuilder<AccessoryCompatibleMobile> builder)
    {
        builder.HasKey(acm => new { acm.AccessoryId, acm.CompatibleBrand, acm.CompatibleModel });
        builder.Property(acm => acm.CompatibleBrand).HasMaxLength(100).IsRequired();
        builder.Property(acm => acm.CompatibleModel).HasMaxLength(150).IsRequired();

        builder.HasIndex(acm => new { acm.CompatibleBrand, acm.CompatibleModel });

        builder.HasOne(acm => acm.Accessory)
            .WithMany(a => a.CompatibleMobiles)
            .HasForeignKey(acm => acm.AccessoryId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
