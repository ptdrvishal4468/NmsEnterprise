using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Nms.Domain.Entities;

namespace Nms.Infrastructure.Data.Configurations;

public class AlertRuleConfiguration : IEntityTypeConfiguration<AlertRule>
{
    public void Configure(EntityTypeBuilder<AlertRule> builder)
    {
        builder.ToTable("AlertRules");

        builder.HasKey(r => r.Id);

        builder.Property(r => r.Name)
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(r => r.Description)
            .HasMaxLength(1000);

        builder.Property(r => r.MetricType)
            .IsRequired();

        builder.Property(r => r.Operator)
            .IsRequired();

        builder.Property(r => r.ThresholdValue)
            .HasPrecision(18, 4)
            .IsRequired();

        builder.Property(r => r.Severity)
            .IsRequired();

        builder.Property(r => r.IsEnabled)
            .IsRequired();

        builder.HasIndex(r => new { r.TenantId, r.IsEnabled });
        builder.HasIndex(r => new { r.TenantId, r.DeviceId });
    }
}