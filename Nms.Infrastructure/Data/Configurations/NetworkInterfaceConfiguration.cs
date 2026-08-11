using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Nms.Domain.Entities;

namespace Nms.Infrastructure.Data.Configurations;

public class NetworkInterfaceConfiguration : IEntityTypeConfiguration<NetworkInterface>
{
    public void Configure(EntityTypeBuilder<NetworkInterface> builder)
    {
        builder.ToTable("NetworkInterfaces");

        builder.HasKey(ni => ni.Id);

        builder.Property(ni => ni.Name)
            .IsRequired()
            .HasMaxLength(128);

        builder.Property(ni => ni.Description)
            .HasMaxLength(256);

        builder.Property(ni => ni.InterfaceType)
            .HasMaxLength(64);

        builder.Property(ni => ni.MacAddress)
            .HasMaxLength(64);

        builder.HasOne(ni => ni.Device)
            .WithMany()
            .HasForeignKey(ni => ni.DeviceId)
            .OnDelete(DeleteBehavior.Cascade);

        // Ensures unique interface index per device within the same tenant
        builder.HasIndex(ni => new { ni.TenantId, ni.DeviceId, ni.IfIndex })
            .IsUnique();

        builder.HasIndex(ni => new { ni.TenantId, ni.DeviceId });
    }
}