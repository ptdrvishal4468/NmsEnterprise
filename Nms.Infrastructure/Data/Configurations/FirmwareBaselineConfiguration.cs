using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Nms.Domain.Entities;

namespace Nms.Infrastructure.Data.Configurations;

public class FirmwareBaselineConfiguration : IEntityTypeConfiguration<FirmwareBaseline>
{
    public void Configure(EntityTypeBuilder<FirmwareBaseline> builder)
    {
        builder.ToTable("FirmwareBaselines");

        builder.HasKey(b => b.Id);

        builder.Property(b => b.TenantId)
            .IsRequired();

        builder.Property(b => b.Vendor)
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(b => b.Model)
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(b => b.TargetVersion)
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(b => b.IsActive)
            .HasDefaultValue(true);

        builder.Property(b => b.Notes)
            .HasMaxLength(1000);

        builder.HasIndex(b => new { b.TenantId, b.Vendor, b.Model })
            .IsUnique()
            .HasDatabaseName("IX_FirmwareBaselines_TenantId_Vendor_Model");
    }
}