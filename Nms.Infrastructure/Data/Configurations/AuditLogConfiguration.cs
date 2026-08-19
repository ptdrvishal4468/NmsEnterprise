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

        builder.Property(a => a.Username)
               .HasMaxLength(100);

        builder.Property(a => a.Action)
               .IsRequired()
               .HasMaxLength(100)
               .IsUnicode(false);

        builder.Property(a => a.EntityName)
               .HasMaxLength(100);

        builder.Property(a => a.EntityId)
               .HasMaxLength(100);

        builder.Property(a => a.Category)
               .IsRequired()
               .HasConversion<string>()
               .HasMaxLength(50)
               .IsUnicode(false);

        builder.Property(a => a.Status)
               .IsRequired()
               .HasConversion<string>()
               .HasMaxLength(20)
               .IsUnicode(false);

        builder.Property(a => a.IpAddress)
               .HasMaxLength(45)
               .IsUnicode(false);

        builder.Property(a => a.Details)
               .HasMaxLength(500);

        builder.Property(a => a.OldValuesJson)
               .HasColumnType("nvarchar(max)");

        builder.Property(a => a.NewValuesJson)
               .HasColumnType("nvarchar(max)");

        builder.Property(a => a.TimestampUtc)
               .IsRequired()
               .HasDefaultValueSql("SYSUTCDATETIME()");

        // Performance & Multi-Tenant Query Indexes
        builder.HasIndex(a => a.TenantId);
        builder.HasIndex(a => a.TimestampUtc);
        builder.HasIndex(a => new { a.TenantId, a.TimestampUtc });
        builder.HasIndex(a => new { a.TenantId, a.Category, a.TimestampUtc });
        builder.HasIndex(a => new { a.TenantId, a.UserId });
        builder.HasIndex(a => new { a.TenantId, a.UserId, a.TimestampUtc });
    }
}