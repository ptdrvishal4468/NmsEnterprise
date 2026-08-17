using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Nms.Domain.Entities;

namespace Nms.Infrastructure.Data.Configurations;

public class FloorConfiguration : IEntityTypeConfiguration<Floor>
{
    public void Configure(EntityTypeBuilder<Floor> builder)
    {
        builder.ToTable("Floors");

        builder.HasKey(f => f.Id);

        builder.Property(f => f.TenantId)
            .IsRequired();

        builder.Property(f => f.BuildingId)
            .IsRequired();

        builder.Property(f => f.Name)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(f => f.Description)
            .HasMaxLength(500);

        builder.Property(f => f.CreatedBy)
            .HasMaxLength(200);

        builder.Property(f => f.LastModifiedBy)
            .HasMaxLength(200);

        // Hierarchy Relationship
        builder.HasOne(f => f.Building)
            .WithMany(b => b.Floors)
            .HasForeignKey(f => f.BuildingId)
            .OnDelete(DeleteBehavior.Cascade);

        // Multi-Tenant Indexes
        builder.HasIndex(f => new { f.TenantId, f.BuildingId, f.FloorNumber })
            .IsUnique();

        builder.HasIndex(f => new { f.TenantId, f.BuildingId });
        builder.HasIndex(f => new { f.TenantId, f.IsActive });
    }
}