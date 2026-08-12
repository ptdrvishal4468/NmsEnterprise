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

        // Optional relationship with Device
        builder.HasOne(x => x.Device)
            .WithMany()
            .HasForeignKey(x => x.DeviceId)
            .OnDelete(DeleteBehavior.SetNull);

        // Performance composite indexes
        builder.HasIndex(x => x.TenantId);
        builder.HasIndex(x => x.TenantId, nameof(SyslogMessage.DeviceId));
        builder.HasIndex(x => x.TenantId, nameof(SyslogMessage.TimestampUtc));
        builder.HasIndex(x => x.TenantId, nameof(SyslogMessage.Severity));
        builder.HasIndex(x => x.TenantId, nameof(SyslogMessage.Facility));
        builder.HasIndex(x => x.TenantId, nameof(SyslogMessage.SourceIpAddress));
    }
}