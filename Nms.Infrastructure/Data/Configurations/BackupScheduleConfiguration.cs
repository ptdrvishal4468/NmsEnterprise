using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Nms.Domain.Entities;

namespace Nms.Infrastructure.Data.Configurations;

public class BackupScheduleConfiguration : IEntityTypeConfiguration<BackupSchedule>
{
    public void Configure(EntityTypeBuilder<BackupSchedule> builder)
    {
        builder.HasKey(s => s.Id);

        builder.Property(s => s.TenantId)
            .IsRequired();

        builder.Property(s => s.Name)
            .HasMaxLength(150)
            .IsRequired();

        builder.Property(s => s.Description)
            .HasMaxLength(500);

        builder.HasOne(s => s.Device)
            .WithMany()
            .HasForeignKey(s => s.DeviceId)
            .OnDelete(DeleteBehavior.SetNull);

        builder.HasIndex(s => s.TenantId);
        builder.HasIndex(s => s.IsEnabled);
        builder.HasIndex(s => s.NextRunUtc);
    }
}