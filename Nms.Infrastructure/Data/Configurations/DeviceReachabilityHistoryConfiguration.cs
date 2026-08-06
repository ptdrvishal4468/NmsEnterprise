using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Nms.Domain.Entities;

namespace Nms.Infrastructure.Data.Configurations;

public sealed class DeviceReachabilityHistoryConfiguration : IEntityTypeConfiguration<DeviceReachabilityHistory>
{
    public void Configure(EntityTypeBuilder<DeviceReachabilityHistory> builder)
    {
        builder.ToTable("DeviceReachabilityHistories");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.DeviceId)
            .IsRequired();

        builder.Property(x => x.TenantId)
            .IsRequired();

        builder.Property(x => x.Status)
            .IsRequired();

        builder.Property(x => x.TimestampUtc)
            .IsRequired();

        builder.HasOne<Device>()
            .WithMany()
            .HasForeignKey(x => x.DeviceId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(x => new { x.DeviceId, x.TimestampUtc })
            .HasDatabaseName("IX_DeviceReachabilityHistories_DeviceId_TimestampUtc");

        builder.HasIndex(x => x.TenantId)
            .HasDatabaseName("IX_DeviceReachabilityHistories_TenantId");
    }
}