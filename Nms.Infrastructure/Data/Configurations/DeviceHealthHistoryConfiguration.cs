using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Nms.Domain.Entities;

namespace Nms.Infrastructure.Data.Configurations;

public class DeviceHealthHistoryConfiguration : IEntityTypeConfiguration<DeviceHealthHistory>
{
    public void Configure(EntityTypeBuilder<DeviceHealthHistory> builder)
    {
        builder.ToTable("DeviceHealthHistory");

        builder.HasKey(h => h.Id);

        builder.Property(h => h.TenantId)
            .IsRequired();

        builder.Property(h => h.DeviceId)
            .IsRequired();

        builder.Property(h => h.HealthScore)
            .IsRequired()
            .HasPrecision(5, 2);

        builder.Property(h => h.Status)
            .IsRequired()
            .HasConversion<int>();

        builder.Property(h => h.Reason)
            .HasMaxLength(1000)
            .IsRequired();

        builder.Property(h => h.TimestampUtc)
            .IsRequired();

        builder.HasOne(h => h.Device)
            .WithMany()
            .HasForeignKey(h => h.DeviceId)
            .OnDelete(DeleteBehavior.Cascade);

        // Composite Index for fast time-series pagination filtered by tenant and device
        builder.HasIndex(h => new { h.TenantId, h.DeviceId, h.TimestampUtc })
            .HasDatabaseName("IX_DeviceHealthHistory_Tenant_Device_Timestamp");
    }
}