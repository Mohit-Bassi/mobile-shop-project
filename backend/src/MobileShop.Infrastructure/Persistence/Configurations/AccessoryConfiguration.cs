using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MobileShop.Domain.Entities;

namespace MobileShop.Infrastructure.Persistence.Configurations;

public class AccessoryConfiguration : IEntityTypeConfiguration<Accessory>
{
    public void Configure(EntityTypeBuilder<Accessory> builder)
    {
        builder.HasKey(a => a.AccessoryId);
        builder.Property(a => a.Name).HasMaxLength(150).IsRequired();
        builder.Property(a => a.Price).HasColumnType("decimal(10,2)");
        builder.Property(a => a.CreatedAtUtc).HasDefaultValueSql("SYSUTCDATETIME()");
        builder.Property(a => a.UpdatedAtUtc).HasDefaultValueSql("SYSUTCDATETIME()");

        builder.HasIndex(a => a.CategoryId);
        builder.HasIndex(a => a.Price);
        builder.HasIndex(a => a.Status);

        builder.HasOne(a => a.Category)
            .WithMany(c => c.Accessories)
            .HasForeignKey(a => a.CategoryId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
