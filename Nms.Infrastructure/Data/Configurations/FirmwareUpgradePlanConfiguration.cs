using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Nms.Domain.Entities;

namespace Nms.Infrastructure.Data.Configurations;

public class FirmwareUpgradePlanConfiguration : IEntityTypeConfiguration<FirmwareUpgradePlan>
{
    public void Configure(EntityTypeBuilder<FirmwareUpgradePlan> builder)
    {
        builder.ToTable("FirmwareUpgradePlans");

        builder.HasKey(p => p.Id);

        builder.Property(p => p.TenantId)
            .IsRequired();

        builder.Property(p => p.DeviceId)
            .IsRequired();

        builder.Property(p => p.TargetVersion)
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(p => p.PlannedDateUtc)
            .IsRequired();

        builder.Property(p => p.Status)
            .IsRequired();

        builder.Property(p => p.Notes)
            .HasMaxLength(1000);

        builder.HasOne(p => p.Device)
            .WithMany()
            .HasForeignKey(p => p.DeviceId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(p => new { p.TenantId, p.DeviceId })
            .HasDatabaseName("IX_FirmwareUpgradePlans_TenantId_DeviceId");

        builder.HasIndex(p => new { p.TenantId, p.Status })
            .HasDatabaseName("IX_FirmwareUpgradePlans_TenantId_Status");
    }
}