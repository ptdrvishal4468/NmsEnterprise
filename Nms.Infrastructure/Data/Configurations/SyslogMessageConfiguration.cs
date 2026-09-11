using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Nms.Domain.Entities;

namespace Nms.Infrastructure.Data.Configurations;

public class SyslogMessageConfiguration : IEntityTypeConfiguration<SyslogMessage>
{
    public void Configure(EntityTypeBuilder<SyslogMessage> builder)
    {
        builder.ToTable("SyslogMessages");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.TenantId)
            .IsRequired();

        builder.Property(x => x.FacilityName)
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(x => x.SeverityName)
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(x => x.Hostname)
            .HasMaxLength(255);

        builder.Property(x => x.AppTag)
            .HasMaxLength(100);

        builder.Property(x => x.ProcessId)
            .HasMaxLength(50);

        builder.Property(x => x.MessageId)
            .HasMaxLength(100);

        builder.Property(x => x.SourceIpAddress)
            .HasMaxLength(45)
            .IsRequired();

        builder.Property(x => x.Message)
            .IsRequired();

        builder.HasOne(x => x.Device)
            .WithMany()
            .HasForeignKey(x => x.DeviceId)
            .OnDelete(DeleteBehavior.SetNull);

        // Performance & Multi-Tenant Composite Indexes
        builder.HasIndex(x => x.TenantId);
        builder.HasIndex(x => new { x.TenantId, x.DeviceId });
        builder.HasIndex(x => new { x.TenantId, x.TimestampUtc });
        builder.HasIndex(x => new { x.TenantId, x.Severity });
        builder.HasIndex(x => new { x.TenantId, x.Facility });
        builder.HasIndex(x => new { x.TenantId, x.SourceIpAddress });
        builder.HasIndex(x => new { x.TenantId, x.Severity, x.TimestampUtc });
        builder.HasIndex(x => new { x.TenantId, x.DeviceId, x.TimestampUtc });
    }
}