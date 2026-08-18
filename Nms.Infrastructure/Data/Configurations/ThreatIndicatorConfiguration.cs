using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Nms.Domain.Entities;

namespace Nms.Infrastructure.Data.Configurations;

public class ThreatIndicatorConfiguration : IEntityTypeConfiguration<ThreatIndicator>
{
    public void Configure(EntityTypeBuilder<ThreatIndicator> builder)
    {
        builder.ToTable("ThreatIndicators");

        builder.HasKey(t => t.Id);

        builder.Property(t => t.Title)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(t => t.Description)
            .IsRequired()
            .HasMaxLength(2000);

        builder.Property(t => t.SourceIp)
            .HasMaxLength(100);

        builder.Property(t => t.TargetUser)
            .HasMaxLength(150);

        builder.Property(t => t.ResolutionNotes)
            .HasMaxLength(2000);

        builder.Property(t => t.ThreatType)
            .IsRequired();

        builder.Property(t => t.Severity)
            .IsRequired();

        builder.Property(t => t.Status)
            .IsRequired();

        builder.HasOne(t => t.TargetDevice)
            .WithMany()
            .HasForeignKey(t => t.TargetDeviceId)
            .OnDelete(DeleteBehavior.SetNull);

        builder.HasIndex(t => new { t.TenantId, t.ThreatType, t.Status });
        builder.HasIndex(t => new { t.TenantId, t.Severity });
        builder.HasIndex(t => new { t.TenantId, t.SourceIp });
        builder.HasIndex(t => new { t.TenantId, t.TargetDeviceId });
        builder.HasIndex(t => new { t.TenantId, t.LastDetectedAtUtc });
    }
}