using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Nms.Domain.Entities;

namespace Nms.Infrastructure.Data.Configurations;

public sealed class SnmpTrapMessageConfiguration : IEntityTypeConfiguration<SnmpTrapMessage>
{
    public void Configure(EntityTypeBuilder<SnmpTrapMessage> builder)
    {
        builder.ToTable("SnmpTrapMessages");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.TenantId)
            .IsRequired();

        builder.Property(x => x.EnterpriseOid)
            .IsRequired()
            .HasMaxLength(255);

        builder.Property(x => x.Community)
            .HasMaxLength(100);

        builder.Property(x => x.TrapOid)
            .HasMaxLength(255);

        builder.Property(x => x.AgentAddress)
            .HasMaxLength(45);

        builder.Property(x => x.SourceIpAddress)
            .IsRequired()
            .HasMaxLength(45);

        builder.Property(x => x.VarbindsJson)
            .IsRequired();

        builder.HasOne(x => x.Device)
            .WithMany()
            .HasForeignKey(x => x.DeviceId)
            .OnDelete(DeleteBehavior.SetNull);

        builder.HasIndex(x => x.TenantId);
        builder.HasIndex(x => new { x.TenantId, x.DeviceId });
        builder.HasIndex(x => new { x.TenantId, x.TimestampUtc });
        builder.HasIndex(x => new { x.TenantId, x.Severity });
        builder.HasIndex(x => new { x.TenantId, x.EnterpriseOid });
        builder.HasIndex(x => new { x.TenantId, x.SourceIpAddress });
    }
}