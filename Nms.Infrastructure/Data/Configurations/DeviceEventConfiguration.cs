using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Nms.Domain.Entities;

namespace Nms.Infrastructure.Data.Configurations;

public class DeviceEventConfiguration : IEntityTypeConfiguration<DeviceEvent>
{
    public void Configure(EntityTypeBuilder<DeviceEvent> builder)
    {
        builder.ToTable("DeviceEvents");

        builder.HasKey(e => e.Id);

        builder.Property(e => e.Source)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(e => e.Message)
            .IsRequired()
            .HasMaxLength(2000);

        builder.Property(e => e.CorrelationId)
            .HasMaxLength(100);

        builder.Property(e => e.MetadataJson)
            .HasColumnType("nvarchar(max)");

        builder.HasOne(e => e.Device)
            .WithMany()
            .HasForeignKey(e => e.DeviceId)
            .OnDelete(DeleteBehavior.SetNull);

        // Performance & Multi-Tenant Indexes
        builder.HasIndex(e => e.TenantId);
        builder.HasIndex(e => new { e.TenantId, e.DeviceId });
        builder.HasIndex(e => new { e.TenantId, e.CorrelationId });
        builder.HasIndex(e => new { e.TenantId, e.CreatedAtUtc });
    }
}