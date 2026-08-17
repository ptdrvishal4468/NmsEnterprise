using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Nms.Domain.Entities;

namespace Nms.Infrastructure.Data.Configurations;

public class TicketConfiguration : IEntityTypeConfiguration<Ticket>
{
    public void Configure(EntityTypeBuilder<Ticket> builder)
    {
        builder.ToTable("Tickets");

        builder.HasKey(t => t.Id);

        builder.Property(t => t.Title)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(t => t.Description)
            .IsRequired()
            .HasMaxLength(4000);

        builder.Property(t => t.ExternalTicketId)
            .HasMaxLength(100);

        builder.Property(t => t.ExternalTicketKey)
            .HasMaxLength(100);

        builder.Property(t => t.ExternalStatus)
            .HasMaxLength(100);

        builder.Property(t => t.ExternalUrl)
            .HasMaxLength(1000);

        builder.Property(t => t.MetadataJson)
            .HasMaxLength(4000);

        // Indexes
        builder.HasIndex(t => new { t.TenantId, t.Status });
        builder.HasIndex(t => new { t.TenantId, t.Priority });
        builder.HasIndex(t => new { t.TenantId, t.ProviderType });
        builder.HasIndex(t => new { t.TenantId, t.ExternalTicketId });
        builder.HasIndex(t => new { t.TenantId, t.CreatedAtUtc });

        // Relationships - using DeleteBehavior.Restrict to prevent SQL Server Error 1785
        builder.HasOne(t => t.Device)
            .WithMany()
            .HasForeignKey(t => t.DeviceId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(t => t.Alert)
            .WithMany()
            .HasForeignKey(t => t.AlertId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(t => t.Customer)
            .WithMany()
            .HasForeignKey(t => t.CustomerId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(t => t.SyncLogs)
            .WithOne(l => l.Ticket)
            .HasForeignKey(l => l.TicketId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}