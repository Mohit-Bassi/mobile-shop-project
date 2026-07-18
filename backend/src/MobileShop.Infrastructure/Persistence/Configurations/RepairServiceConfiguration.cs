using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MobileShop.Domain.Entities;

namespace MobileShop.Infrastructure.Persistence.Configurations;

public class RepairServiceConfiguration : IEntityTypeConfiguration<RepairService>
{
    public void Configure(EntityTypeBuilder<RepairService> builder)
    {
        builder.HasKey(rs => rs.RepairServiceId);
        builder.Property(rs => rs.Title).HasMaxLength(150).IsRequired();
        builder.Property(rs => rs.PriceFrom).HasColumnType("decimal(10,2)");
        builder.Property(rs => rs.EstimatedTurnaround).HasMaxLength(100);
    }
}
