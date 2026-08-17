using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Nms.Domain.Entities;

namespace Nms.Infrastructure.Data.Configurations;

public class RoomConfiguration : IEntityTypeConfiguration<Room>
{
    public void Configure(EntityTypeBuilder<Room> builder)
    {
        builder.ToTable("Rooms");

        builder.HasKey(r => r.Id);

        builder.Property(r => r.TenantId)
            .IsRequired();

        builder.Property(r => r.FloorId)
            .IsRequired();

        builder.Property(r => r.Name)
            .IsRequired()
            .HasMaxLength(150);

        builder.Property(r => r.Code)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(r => r.RoomType)
            .HasMaxLength(100);

        builder.Property(r => r.Description)
            .HasMaxLength(500);

        builder.Property(r => r.CreatedBy)
            .HasMaxLength(200);

        builder.Property(r => r.LastModifiedBy)
            .HasMaxLength(200);

        // Hierarchy Relationship
        builder.HasOne(r => r.Floor)
            .WithMany(f => f.Rooms)
            .HasForeignKey(r => r.FloorId)
            .OnDelete(DeleteBehavior.Cascade);

        // Multi-Tenant Indexes
        builder.HasIndex(r => new { r.TenantId, r.FloorId, r.Code })
            .IsUnique();

        builder.HasIndex(r => new { r.TenantId, r.FloorId });
        builder.HasIndex(r => new { r.TenantId, r.IsActive });
    }
}