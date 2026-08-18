using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Nms.Domain.Entities;

namespace Nms.Infrastructure.Data.Configurations;

public class DeviceComplianceResultConfiguration : IEntityTypeConfiguration<DeviceComplianceResult>
{
    public void Configure(EntityTypeBuilder<DeviceComplianceResult> builder)
    {
        builder.ToTable("DeviceComplianceResults");

        builder.HasKey(r => r.Id);

        builder.Property(r => r.TenantId)
            .IsRequired();

        builder.Property(r => r.ScanId)
            .IsRequired();

        builder.Property(r => r.PolicyId)
            .IsRequired();

        builder.Property(r => r.CheckType)
            .IsRequired();

        builder.Property(r => r.Category)
            .IsRequired();

        builder.Property(r => r.Severity)
            .IsRequired();

        builder.Property(r => r.Status)
            .IsRequired();

        builder.Property(r => r.Summary)
            .IsRequired()
            .HasMaxLength(500);

        builder.Property(r => r.Details)
            .HasMaxLength(2000);

        builder.Property(r => r.RemediationGuidance)
            .HasMaxLength(2000);

        builder.Property(r => r.EvaluatedAtUtc)
            .IsRequired();

        // Restrict delete on Policy to prevent cascade cycle
        builder.HasOne(r => r.Policy)
            .WithMany(p => p.Results)
            .HasForeignKey(r => r.PolicyId)
            .OnDelete(DeleteBehavior.Restrict);

        // Indexes
        builder.HasIndex(r => new { r.TenantId, r.ScanId });
        builder.HasIndex(r => new { r.TenantId, r.PolicyId });
        builder.HasIndex(r => new { r.TenantId, r.Status });
    }
}