using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Nms.Domain.Entities;

namespace Nms.Infrastructure.Data.Configurations;

public class AlertConfiguration : IEntityTypeConfiguration<Alert>
{
    public void Configure(EntityTypeBuilder<Alert> builder)
    {
        builder.ToTable("Alerts");

        builder.HasKey(a => a.Id);

        builder.Property(a => a.MetricType)
            .IsRequired();

        builder.Property(a => a.Severity)
            .IsRequired();

        builder.Property(a => a.State)
            .IsRequired();

        builder.Property(a => a.MetricValue)
            .HasPrecision(18, 4)
            .IsRequired();

        builder.Property(a => a.ThresholdValue)
            .HasPrecision(18, 4)
            .IsRequired();

        builder.Property(a => a.Message)
            .HasMaxLength(1000)
            .IsRequired();

        builder.Property(a => a.AcknowledgedBy)
            .HasMaxLength(200);

        builder.Property(a => a.SuppressedBy)
            .HasMaxLength(200);

        builder.HasOne(a => a.AlertRule)
            .WithMany()
            .HasForeignKey(a => a.AlertRuleId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(a => a.Device)
            .WithMany()
            .HasForeignKey(a => a.DeviceId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(a => new { a.TenantId, a.DeviceId, a.State });
        builder.HasIndex(a => new { a.TenantId, a.AlertRuleId, a.DeviceId, a.State });
        builder.HasIndex(a => new { a.TenantId, a.TriggeredAtUtc });
    }
}