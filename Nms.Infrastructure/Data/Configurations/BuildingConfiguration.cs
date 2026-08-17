using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Nms.Domain.Entities;

namespace Nms.Infrastructure.Data.Configurations;

public class BuildingConfiguration : IEntityTypeConfiguration<Building>
{
    public void Configure(EntityTypeBuilder<Building> builder)
    {
        builder.ToTable("Buildings");

        builder.HasKey(b => b.Id);

        builder.Property(b => b.TenantId)
            .IsRequired();

        builder.Property(b => b.SiteId)
            .IsRequired();

        builder.Property(b => b.Name)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(b => b.Code)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(b => b.Description)
            .HasMaxLength(500);

        builder.Property(b => b.Address)
            .HasMaxLength(500);

        builder.Property(b => b.CreatedBy)
            .HasMaxLength(200);

        builder.Property(b => b.LastModifiedBy)
            .HasMaxLength(200);

        // Hierarchy Relationship
        builder.HasOne(b => b.Site)
            .WithMany(s => s.Buildings)
            .HasForeignKey(b => b.SiteId)
            .OnDelete(DeleteBehavior.Cascade);

        // Multi-Tenant Indexes
        builder.HasIndex(b => new { b.TenantId, b.SiteId, b.Code })
            .IsUnique();

        builder.HasIndex(b => new { b.TenantId, b.SiteId });
        builder.HasIndex(b => new { b.TenantId, b.IsActive });
    }
}