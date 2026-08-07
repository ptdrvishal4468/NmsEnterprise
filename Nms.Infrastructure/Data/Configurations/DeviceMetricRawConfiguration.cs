using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Nms.Domain.Entities;

namespace Nms.Infrastructure.Data.Configurations;

public class DeviceMetricRawConfiguration : IEntityTypeConfiguration<DeviceMetricRaw>
{
    public void Configure(EntityTypeBuilder<DeviceMetricRaw> builder)
    {
        builder.ToTable("DeviceMetricsRaw");

        // PK MUST be Non-Clustered so TimestampUtc can hold the Clustered Index
        builder.HasKey(m => m.Id)
               .IsClustered(false);

        builder.Property(m => m.TenantId)
               .IsRequired();

        builder.Property(m => m.DeviceId)
               .IsRequired();

        builder.Property(m => m.CpuUtilization)
               .HasPrecision(5, 2)
               .IsRequired();

        builder.Property(m => m.RamUtilization)
               .HasPrecision(5, 2)
               .IsRequired();

        builder.Property(m => m.DiskUtilization)
               .HasPrecision(5, 2)
               .IsRequired();

        builder.Property(m => m.InterfaceUtilization)
               .HasPrecision(5, 2)
               .IsRequired();

        builder.Property(m => m.Temperature)
               .HasPrecision(5, 2)
               .IsRequired();

        builder.Property(m => m.FanStatus)
               .IsRequired();

        builder.Property(m => m.PowerSupplyStatus)
               .IsRequired();

        builder.Property(m => m.LatencyMs)
               .IsRequired();

        builder.Property(m => m.TimestampUtc)
               .IsRequired();

        // Clustered Index on TimestampUtc for time-series range queries
        builder.HasIndex(m => m.TimestampUtc)
               .IsClustered(true);

        // Composite Non-Clustered Index for tenant-isolated device time-series queries
        builder.HasIndex(m => new { m.TenantId, m.DeviceId, m.TimestampUtc });
    }
}