using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Nms.Domain.Entities;

namespace Nms.Infrastructure.Data.Configurations;

public class ConfigurationDriftRecordConfiguration : IEntityTypeConfiguration<ConfigurationDriftRecord>
{
    public void Configure(EntityTypeBuilder<ConfigurationDriftRecord> builder)
    {
        builder.ToTable("ConfigurationDriftRecords");

        builder.HasKey(d => d.Id);

        builder.Property(d => d.AcknowledgmentNotes)
            .HasMaxLength(1000);

        builder.Property(d => d.Severity)
            .IsRequired();

        builder.HasOne(d => d.Device)
            .WithMany()
            .HasForeignKey(d => d.DeviceId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(d => d.BaselineBackup)
            .WithMany()
            .HasForeignKey(d => d.BaselineBackupId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(d => d.CurrentBackup)
            .WithMany()
            .HasForeignKey(d => d.CurrentBackupId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(d => new { d.TenantId, d.DeviceId, d.DetectedAtUtc });
        builder.HasIndex(d => new { d.TenantId, d.HasDrift });
    }
}