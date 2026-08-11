using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Nms.Domain.Entities;

namespace Nms.Infrastructure.Data.Configurations;

public class NetworkInterfaceHistoryConfiguration : IEntityTypeConfiguration<NetworkInterfaceHistory>
{
    public void Configure(EntityTypeBuilder<NetworkInterfaceHistory> builder)
    {
        builder.ToTable("NetworkInterfaceHistory");

        builder.HasKey(nih => nih.Id);

        builder.HasOne(nih => nih.NetworkInterface)
            .WithMany()
            .HasForeignKey(nih => nih.NetworkInterfaceId)
            .OnDelete(DeleteBehavior.Cascade);

        // Optimized time-series indexing for tenant queries and historical reports
        builder.HasIndex(nih => new { nih.TenantId, nih.DeviceId, nih.TimestampUtc });
        builder.HasIndex(nih => new { nih.NetworkInterfaceId, nih.TimestampUtc });
    }
}