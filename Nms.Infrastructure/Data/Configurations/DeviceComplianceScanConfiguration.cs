using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Nms.Domain.Entities;

namespace Nms.Infrastructure.Data.Configurations;

public class DeviceComplianceScanConfiguration : IEntityTypeConfiguration<DeviceComplianceScan>
{
    public void Configure(EntityTypeBuilder<DeviceComplianceScan> builder)
    {
        builder.ToTable("DeviceComplianceScans");

        builder.HasKey(s => s.Id);

        builder.Property(s => s.TenantId)
            .IsRequired();

        builder.Property(s => s.DeviceId)
            .IsRequired();

        builder.Property(s => s.ScannedAtUtc)
            .IsRequired();

        builder.Property(s => s.OverallStatus)
            .IsRequired();

        builder.Property(s => s.EvaluationNotes)
            .HasMaxLength(1000);

        builder.Property(s => s.CreatedBy)
            .HasMaxLength(100);

        builder.Property(s => s.LastModifiedBy)
            .HasMaxLength(100);

        // Restrict delete on Device to prevent SQL Server Error 1785
        builder.HasOne(s => s.Device)
            .WithMany()
            .HasForeignKey(s => s.DeviceId)
            .OnDelete(DeleteBehavior.Restrict);

        // Cascade delete results when scan aggregate is removed
        builder.HasMany(s => s.Results)
            .WithOne(r => r.Scan)
            .HasForeignKey(r => r.ScanId)
            .OnDelete(DeleteBehavior.Cascade);

        // Indexes for multi-tenant queries
        builder.HasIndex(s => new { s.TenantId, s.DeviceId });
        builder.HasIndex(s => new { s.TenantId, s.ScannedAtUtc });
        builder.HasIndex(s => new { s.TenantId, s.OverallStatus });
    }
}