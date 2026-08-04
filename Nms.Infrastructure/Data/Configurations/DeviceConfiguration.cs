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

        builder.Property(d => d.Hostname)
               .HasMaxLength(255);

        builder.Property(d => d.Vendor)
               .HasMaxLength(100);

        builder.Property(d => d.Model)
               .HasMaxLength(100);

        builder.Property(d => d.SerialNumber)
               .HasMaxLength(100);

        builder.Property(d => d.FirmwareVersion)
               .HasMaxLength(100);

        builder.Property(d => d.MacAddress)
               .HasMaxLength(17)
               .IsUnicode(false);

        builder.Property(d => d.Site)
               .HasMaxLength(100);

        builder.Property(d => d.Location)
               .HasMaxLength(200);

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

        // Indexes
        builder.HasIndex(d => new { d.TenantId, d.IpAddress })
               .IsUnique();

        builder.HasIndex(d => new { d.TenantId, d.Status });

        builder.HasIndex(d => new { d.TenantId, d.SerialNumber });

        builder.HasIndex(d => new { d.TenantId, d.DeviceType });

        builder.HasOne<Tenant>()
               .WithMany()
               .HasForeignKey(d => d.TenantId)
               .OnDelete(DeleteBehavior.Cascade);
    }
}