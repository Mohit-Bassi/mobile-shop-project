using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MobileShop.Domain.Entities;

namespace MobileShop.Infrastructure.Persistence.Configurations;

public class InquiryConfiguration : IEntityTypeConfiguration<Inquiry>
{
    public void Configure(EntityTypeBuilder<Inquiry> builder)
    {
        builder.HasKey(i => i.InquiryId);
        builder.Property(i => i.CustomerName).HasMaxLength(150).IsRequired();
        builder.Property(i => i.CustomerPhone).HasMaxLength(30).IsRequired();
        builder.Property(i => i.CustomerEmail).HasMaxLength(256);
        builder.Property(i => i.Message).HasMaxLength(1000);
        builder.Property(i => i.CreatedAtUtc).HasDefaultValueSql("SYSUTCDATETIME()");

        builder.HasIndex(i => new { i.Status, i.CreatedAtUtc });
        builder.HasIndex(i => new { i.ListingType, i.ListingId });
    }
}
