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

        builder.Property(m => m.CpuUtilization)
               .HasPrecision(5, 2)
               .IsRequired();

        builder.Property(m => m.RamUtilization)
               .HasPrecision(5, 2)
               .IsRequired();

        builder.Property(m => m.LatencyMs)
               .IsRequired();

        builder.Property(m => m.TimestampUtc)
               .IsRequired();

        // Clustered Index on TimestampUtc for time-series range queries
        builder.HasIndex(m => m.TimestampUtc)
               .IsClustered(true);

        // Non-Clustered Index for filtering by Device
        builder.HasIndex(m => m.DeviceId);
    }
}