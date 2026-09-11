using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Nms.Domain.Entities;

namespace Nms.Infrastructure.Data.Configurations;

public class TicketSyncLogConfiguration : IEntityTypeConfiguration<TicketSyncLog>
{
    public void Configure(EntityTypeBuilder<TicketSyncLog> builder)
    {
        builder.ToTable("TicketSyncLogs");

        builder.HasKey(l => l.Id);

        builder.Property(l => l.RequestPayload)
            .HasMaxLength(4000);

        builder.Property(l => l.ResponsePayload)
            .HasMaxLength(4000);

        builder.Property(l => l.ErrorMessage)
            .HasMaxLength(4000);

        builder.HasIndex(l => new { l.TenantId, l.TicketId });
        builder.HasIndex(l => new { l.TenantId, l.TimestampUtc });
    }
}