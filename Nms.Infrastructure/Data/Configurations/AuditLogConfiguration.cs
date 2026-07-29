using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Nms.Domain.Entities;

namespace Nms.Infrastructure.Data.Configurations;

public class AuditLogConfiguration : IEntityTypeConfiguration<AuditLog>
{
    public void Configure(EntityTypeBuilder<AuditLog> builder)
    {
        builder.ToTable("AuditLogs");

        builder.HasKey(a => a.Id);

        builder.Property(a => a.TenantId)
               .IsRequired();

        builder.Property(a => a.UserId)
               .IsRequired();

        builder.Property(a => a.Action)
               .IsRequired()
               .HasMaxLength(100)
               .IsUnicode(false);

        builder.Property(a => a.OldValuesJson)
               .HasColumnType("nvarchar(max)");

        builder.Property(a => a.NewValuesJson)
               .HasColumnType("nvarchar(max)");

        builder.Property(a => a.TimestampUtc)
               .IsRequired()
               .HasDefaultValueSql("SYSUTCDATETIME()");

        builder.HasIndex(a => a.TenantId);
        builder.HasIndex(a => a.TimestampUtc);
    }
}