using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Nms.Domain.Entities;

namespace Nms.Infrastructure.Data.Configurations;

public class NotificationLogConfiguration : IEntityTypeConfiguration<NotificationLog>
{
    public void Configure(EntityTypeBuilder<NotificationLog> builder)
    {
        builder.ToTable("NotificationLogs");

        builder.HasKey(l => l.Id);

        builder.Property(l => l.Channel)
            .IsRequired();

        builder.Property(l => l.RecipientTarget)
            .IsRequired()
            .HasMaxLength(500);

        builder.Property(l => l.Subject)
            .IsRequired()
            .HasMaxLength(500);

        builder.Property(l => l.Message)
            .IsRequired();

        builder.Property(l => l.Status)
            .IsRequired();

        builder.HasOne(l => l.Alert)
            .WithMany()
            .HasForeignKey(l => l.AlertId)
            .OnDelete(DeleteBehavior.SetNull);

        builder.HasIndex(l => new { l.TenantId, l.AlertId, l.SentAtUtc })
            .HasDatabaseName("IX_NotificationLogs_TenantId_AlertId_SentAtUtc");
    }
}