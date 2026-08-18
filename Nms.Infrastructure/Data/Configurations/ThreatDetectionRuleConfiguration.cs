using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Nms.Domain.Entities;

namespace Nms.Infrastructure.Data.Configurations;

public class ThreatDetectionRuleConfiguration : IEntityTypeConfiguration<ThreatDetectionRule>
{
    public void Configure(EntityTypeBuilder<ThreatDetectionRule> builder)
    {
        builder.ToTable("ThreatDetectionRules");

        builder.HasKey(r => r.Id);

        builder.Property(r => r.RuleName)
            .IsRequired()
            .HasMaxLength(150);

        builder.Property(r => r.Description)
            .HasMaxLength(500);

        builder.Property(r => r.ThreatType)
            .IsRequired();

        builder.Property(r => r.DefaultSeverity)
            .IsRequired();

        builder.HasIndex(r => new { r.TenantId, r.ThreatType }).IsUnique();
        builder.HasIndex(r => new { r.TenantId, r.IsEnabled });
    }
}