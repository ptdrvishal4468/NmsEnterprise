using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Nms.Domain.Entities;

namespace Nms.Infrastructure.Data.Configurations;

public class ConfigurationRestoreLogConfiguration : IEntityTypeConfiguration<ConfigurationRestoreLog>
{
    public void Configure(EntityTypeBuilder<ConfigurationRestoreLog> builder)
    {
        builder.ToTable("ConfigurationRestoreLogs");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.TenantId)
            .IsRequired();

        builder.Property(x => x.InitiatedBy)
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(x => x.FailureReason)
            .HasMaxLength(2000);

        builder.Property(x => x.RollbackReason)
            .HasMaxLength(2000);

        builder.Property(x => x.AuditNotes)
            .HasMaxLength(1000);

        builder.Property(x => x.Status)
            .IsRequired();

        builder.Property(x => x.StartTimeUtc)
            .IsRequired();

        // Foreign Key to Device (Restrict deletion to prevent cascade conflicts)
        builder.HasOne(x => x.Device)
            .WithMany()
            .HasForeignKey(x => x.DeviceId)
            .OnDelete(DeleteBehavior.Restrict);

        // Foreign Key to Target Backup
        builder.HasOne(x => x.TargetBackup)
            .WithMany()
            .HasForeignKey(x => x.TargetBackupId)
            .OnDelete(DeleteBehavior.Restrict);

        // Foreign Key to Pre-Restore Safety Backup (Optional)
        builder.HasOne(x => x.PreRestoreBackup)
            .WithMany()
            .HasForeignKey(x => x.PreRestoreBackupId)
            .OnDelete(DeleteBehavior.SetNull);

        // Indexes for performance and multi-tenant queries
        builder.HasIndex(x => x.TenantId);
        builder.HasIndex(x => x.DeviceId);
        builder.HasIndex(x => x.TargetBackupId);
        builder.HasIndex(x => new { x.TenantId, x.DeviceId });
        builder.HasIndex(x => new { x.TenantId, x.Status });
    }
}