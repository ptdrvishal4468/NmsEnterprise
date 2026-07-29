using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Nms.Domain.Entities;

namespace Nms.Infrastructure.Data.Configurations;

public class DeviceMetricRawConfiguration : IEntityTypeConfiguration<DeviceMetricRaw>
{
    public void Configure(EntityTypeBuilder<DeviceMetricRaw> builder)
    {
        builder.ToTable("DeviceMetricsRaw");

        builder.HasKey(m => m.Id);

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

        // Clustered Index on TimestampUtc for fast time-series analytical queries
        builder.HasIndex(m => m.TimestampUtc)
               .IsClustered(true);

        builder.HasOne<Device>()
               .WithMany()
               .HasForeignKey(m => m.DeviceId)
               .OnDelete(DeleteBehavior.Cascade);
    }
}