using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Nms.Domain.Entities;

namespace Nms.Infrastructure.Data.Configurations;

public class TopologyLinkConfiguration : IEntityTypeConfiguration<TopologyLink>
{
    public void Configure(EntityTypeBuilder<TopologyLink> builder)
    {
        builder.ToTable("TopologyLinks");

        builder.HasKey(t => t.Id);

        builder.Property(t => t.TenantId)
            .IsRequired();

        builder.Property(t => t.SourceDeviceId)
            .IsRequired();

        builder.Property(t => t.TargetDeviceId)
            .IsRequired();

        builder.Property(t => t.LayerType)
            .IsRequired();

        builder.Property(t => t.Protocol)
            .IsRequired();

        builder.Property(t => t.Status)
            .IsRequired();

        builder.Property(t => t.SpeedBps)
            .IsRequired()
            .HasDefaultValue(0L);

        builder.Property(t => t.LastDiscoveredUtc)
            .IsRequired();

        builder.Property(t => t.MetadataJson)
            .HasMaxLength(4000);

        // Foreign Key Relationships
        builder.HasOne(t => t.SourceDevice)
            .WithMany()
            .HasForeignKey(t => t.SourceDeviceId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(t => t.TargetDevice)
            .WithMany()
            .HasForeignKey(t => t.TargetDeviceId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(t => t.SourceInterface)
            .WithMany()
            .HasForeignKey(t => t.SourceInterfaceId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(t => t.TargetInterface)
            .WithMany()
            .HasForeignKey(t => t.TargetInterfaceId)
            .OnDelete(DeleteBehavior.Restrict);

        // Performance & Multi-Tenant Indexes
        builder.HasIndex(t => t.TenantId);
        builder.HasIndex(t => new { t.TenantId, t.SourceDeviceId });
        builder.HasIndex(t => new { t.TenantId, t.TargetDeviceId });
        builder.HasIndex(t => new { t.TenantId, t.Status });
        builder.HasIndex(t => t.SourceInterfaceId);
        builder.HasIndex(t => t.TargetInterfaceId);
        builder.HasIndex(t => new { t.TenantId, t.LayerType, t.Status });
    }
}