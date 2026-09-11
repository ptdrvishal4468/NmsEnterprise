using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Nms.Domain.Entities;

namespace Nms.Infrastructure.Data.Configurations;

public class DiscoveredDeviceCandidateConfiguration : IEntityTypeConfiguration<DiscoveredDeviceCandidate>
{
    public void Configure(EntityTypeBuilder<DiscoveredDeviceCandidate> builder)
    {
        builder.ToTable("DiscoveredDeviceCandidates");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.IpAddress)
            .IsRequired()
            .HasMaxLength(45);

        builder.Property(x => x.SysDescr)
            .HasMaxLength(500);

        builder.Property(x => x.SysObjectId)
            .HasMaxLength(100);

        builder.Property(x => x.SysName)
            .HasMaxLength(200);

        builder.Property(x => x.MacAddress)
            .HasMaxLength(50);

        builder.Property(x => x.IdentifiedVendor)
            .IsRequired()
            .HasMaxLength(100);

        builder.HasIndex(x => new { x.TenantId, x.IpAddress });
    }
}