using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Nms.Domain.Entities;

namespace Nms.Infrastructure.Data.Configurations;

public class ConfigurationBackupConfiguration : IEntityTypeConfiguration<ConfigurationBackup>
{
    public void Configure(EntityTypeBuilder<ConfigurationBackup> builder)
    {
        builder.HasKey(b => b.Id);

        builder.Property(b => b.TenantId)
            .IsRequired();

        builder.Property(b => b.DeviceId)
            .IsRequired();

        builder.Property(b => b.VersionNumber)
            .IsRequired();

        builder.Property(b => b.StoragePath)
            .HasMaxLength(500)
            .IsRequired();

        builder.Property(b => b.ChecksumSha256)
            .HasMaxLength(64)
            .IsRequired();

        builder.Property(b => b.FailureReason)
            .HasMaxLength(1000);

        builder.Property(b => b.RestorePreparationNotes)
            .HasMaxLength(1000);

        builder.HasOne(b => b.Device)
            .WithMany()
            .HasForeignKey(b => b.DeviceId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(b => b.TenantId);
        builder.HasIndex(b => new { b.TenantId, b.DeviceId });
        builder.HasIndex(b => new { b.TenantId, b.DeviceId, b.VersionNumber })
            .IsUnique();
        builder.HasIndex(b => new { b.TenantId, b.TimestampUtc });
    }
}