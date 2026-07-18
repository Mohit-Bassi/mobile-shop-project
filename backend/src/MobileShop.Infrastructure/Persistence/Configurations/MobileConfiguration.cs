using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MobileShop.Domain.Entities;

namespace MobileShop.Infrastructure.Persistence.Configurations;

public class MobileConfiguration : IEntityTypeConfiguration<Mobile>
{
    public void Configure(EntityTypeBuilder<Mobile> builder)
    {
        builder.HasKey(m => m.MobileId);
        builder.Property(m => m.Brand).HasMaxLength(100).IsRequired();
        builder.Property(m => m.Model).HasMaxLength(150).IsRequired();
        builder.Property(m => m.Storage).HasMaxLength(50);
        builder.Property(m => m.Color).HasMaxLength(50);
        builder.Property(m => m.Price).HasColumnType("decimal(10,2)");
        builder.Property(m => m.CreatedAtUtc).HasDefaultValueSql("SYSUTCDATETIME()");
        builder.Property(m => m.UpdatedAtUtc).HasDefaultValueSql("SYSUTCDATETIME()");

        builder.HasIndex(m => m.Brand);
        builder.HasIndex(m => m.Price);
        builder.HasIndex(m => m.Status);
        builder.HasIndex(m => new { m.Status, m.Brand, m.Price });
    }
}
