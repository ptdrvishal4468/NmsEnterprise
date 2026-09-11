using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Nms.Domain.Entities;

namespace Nms.Infrastructure.Data.Configurations;

public class CompliancePolicyConfiguration : IEntityTypeConfiguration<CompliancePolicy>
{
    public void Configure(EntityTypeBuilder<CompliancePolicy> builder)
    {
        builder.ToTable("CompliancePolicies");

        builder.HasKey(p => p.Id);

        builder.Property(p => p.TenantId)
            .IsRequired();

        builder.Property(p => p.Name)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(p => p.Description)
            .HasMaxLength(1000);

        builder.Property(p => p.Category)
            .IsRequired();

        builder.Property(p => p.CheckType)
            .IsRequired();

        builder.Property(p => p.Severity)
            .IsRequired();

        builder.Property(p => p.IsActive)
            .IsRequired();

        builder.Property(p => p.TargetVendor)
            .HasMaxLength(100);

        builder.Property(p => p.TargetDeviceType);

        builder.Property(p => p.RuleConfigurationJson)
            .HasMaxLength(4000);

        builder.Property(p => p.CreatedBy)
            .HasMaxLength(100);

        builder.Property(p => p.LastModifiedBy)
            .HasMaxLength(100);

        // Indexes for multi-tenant querying
        builder.HasIndex(p => new { p.TenantId, p.Category });
        builder.HasIndex(p => new { p.TenantId, p.IsActive });
        builder.HasIndex(p => new { p.TenantId, p.CheckType });
    }
}