using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Nms.Domain.Entities;

namespace Nms.Infrastructure.Data.Configurations;

public class AlertHistoryConfiguration : IEntityTypeConfiguration<AlertHistory>
{
    public void Configure(EntityTypeBuilder<AlertHistory> builder)
    {
        builder.ToTable("AlertHistories");

        builder.HasKey(h => h.Id);

        builder.Property(h => h.ChangedBy)
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(h => h.Note)
            .HasMaxLength(1000);

        builder.HasOne(h => h.Alert)
            .WithMany()
            .HasForeignKey(h => h.AlertId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(h => new { h.TenantId, h.AlertId, h.TimestampUtc });
    }
}