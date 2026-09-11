using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Nms.Domain.Entities;

namespace Nms.Infrastructure.Data.Configurations;

public class RackConfiguration : IEntityTypeConfiguration<Rack>
{
    public void Configure(EntityTypeBuilder<Rack> builder)
    {
        builder.ToTable("Racks");

        builder.HasKey(rk => rk.Id);

        builder.Property(rk => rk.TenantId)
            .IsRequired();

        builder.Property(rk => rk.RoomId)
            .IsRequired();

        builder.Property(rk => rk.Name)
            .IsRequired()
            .HasMaxLength(150);

        builder.Property(rk => rk.Identifier)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(rk => rk.MaxPowerWatts)
            .HasPrecision(18, 2);

        builder.Property(rk => rk.MaxWeightKg)
            .HasPrecision(18, 2);

        builder.Property(rk => rk.Notes)
            .HasMaxLength(1000);

        builder.Property(rk => rk.CreatedBy)
            .HasMaxLength(200);

        builder.Property(rk => rk.LastModifiedBy)
            .HasMaxLength(200);

        // Hierarchy Relationship
        builder.HasOne(rk => rk.Room)
            .WithMany(r => r.Racks)
            .HasForeignKey(rk => rk.RoomId)
            .OnDelete(DeleteBehavior.Cascade);

        // Multi-Tenant Indexes
        builder.HasIndex(rk => new { rk.TenantId, rk.RoomId, rk.Identifier })
            .IsUnique();

        builder.HasIndex(rk => new { rk.TenantId, rk.RoomId });
        builder.HasIndex(rk => new { rk.TenantId, rk.IsActive });
    }
}