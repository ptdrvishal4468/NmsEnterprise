using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Nms.Domain.Entities;

namespace Nms.Infrastructure.Data.Configurations;

public class FirmwareUpgradeRecommendationConfiguration : IEntityTypeConfiguration<FirmwareUpgradeRecommendation>
{
    public void Configure(EntityTypeBuilder<FirmwareUpgradeRecommendation> builder)
    {
        builder.ToTable("FirmwareUpgradeRecommendations");

        builder.HasKey(r => r.Id);

        builder.Property(r => r.TenantId)
            .IsRequired();

        builder.Property(r => r.CurrentVersion)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(r => r.RecommendedVersion)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(r => r.Priority)
            .IsRequired();

        builder.Property(r => r.Reasoning)
            .HasMaxLength(2000);

        builder.HasOne(r => r.Device)
            .WithMany()
            .HasForeignKey(r => r.DeviceId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(r => new { r.TenantId, r.DeviceId, r.IsApplied });
        builder.HasIndex(r => new { r.TenantId, r.Priority });
    }
}