using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Nms.Domain.Entities;
using Nms.Domain.Enums;

namespace Nms.Infrastructure.Data.Configurations;

public class DeviceConfiguration : IEntityTypeConfiguration<Device>
{
    public void Configure(EntityTypeBuilder<Device> builder)
    {
        builder.ToTable("Devices");

        builder.HasKey(d => d.Id);
        builder.Property(d => d.Id)
               .HasDefaultValueSql("NEWSEQUENTIALID()");

        builder.Property(d => d.TenantId)
               .IsRequired();

        builder.Property(d => d.Name)
               .IsRequired()
               .HasMaxLength(150);

        builder.Property(d => d.IpAddress)
               .IsRequired()
               .HasMaxLength(45)
               .IsUnicode(false);

        builder.Property(d => d.DeviceType)
               .IsRequired()
               .HasConversion<int>();

        builder.Property(d => d.SnmpPort)
               .IsRequired()
               .HasDefaultValue(161);

        builder.Property(d => d.SnmpV3User)
               .HasMaxLength(100);

        builder.Property(d => d.Status)
       .IsRequired()
       .HasConversion<int>()
       .HasDefaultValue(DeviceStatus.Unknown);

        builder.HasIndex(d => new { d.TenantId, d.Status });

        builder.HasOne<Tenant>()
               .WithMany()
               .HasForeignKey(d => d.TenantId)
               .OnDelete(DeleteBehavior.Cascade);
    }
}