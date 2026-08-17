using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Nms.Domain.Entities;

namespace Nms.Infrastructure.Data.Configurations;

public class AssetConfiguration : IEntityTypeConfiguration<Asset>
{
    public void Configure(EntityTypeBuilder<Asset> builder)
    {
        builder.ToTable("Assets");

        builder.HasKey(a => a.Id);

        builder.Property(a => a.AssetTag)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(a => a.Name)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(a => a.SerialNumber)
            .HasMaxLength(100);

        builder.Property(a => a.Vendor)
            .HasMaxLength(100);

        builder.Property(a => a.Model)
            .HasMaxLength(100);

        builder.Property(a => a.Category)
            .HasMaxLength(100);

        builder.Property(a => a.LifecycleState)
            .IsRequired()
            .HasConversion<string>()
            .HasMaxLength(50);

        builder.Property(a => a.LifecycleNotes)
            .HasMaxLength(1000);

        builder.Property(a => a.WarrantyProvider)
            .HasMaxLength(200);

        builder.Property(a => a.WarrantyStatus)
            .IsRequired()
            .HasConversion<string>()
            .HasMaxLength(50);

        builder.Property(a => a.WarrantyContractNumber)
            .HasMaxLength(100);

        builder.Property(a => a.PurchaseOrderNumber)
            .HasMaxLength(100);

        builder.Property(a => a.PurchasePrice)
            .HasPrecision(18, 2);

        builder.Property(a => a.Currency)
            .HasMaxLength(10);

        builder.Property(a => a.SiteOrLocation)
            .HasMaxLength(200);

        builder.Property(a => a.RackIdentifier)
            .HasMaxLength(100);

        builder.Property(a => a.RackUnitPosition)
            .HasMaxLength(50);

        builder.Property(a => a.Department)
            .HasMaxLength(100);

        builder.Property(a => a.CreatedBy)
            .HasMaxLength(200);

        builder.Property(a => a.LastModifiedBy)
            .HasMaxLength(200);

        // Relationships
        builder.HasOne(a => a.AssignedToUser)
            .WithMany()
            .HasForeignKey(a => a.AssignedToUserId)
            .OnDelete(DeleteBehavior.SetNull);

        builder.HasOne(a => a.Device)
            .WithMany()
            .HasForeignKey(a => a.DeviceId)
            .OnDelete(DeleteBehavior.SetNull);

        // Multi-Tenant Indexes
        builder.HasIndex(a => new { a.TenantId, a.AssetTag })
            .IsUnique();

        builder.HasIndex(a => new { a.TenantId, a.LifecycleState });
        builder.HasIndex(a => new { a.TenantId, a.WarrantyStatus });
        builder.HasIndex(a => new { a.TenantId, a.DeviceId });
        builder.HasIndex(a => new { a.TenantId, a.SerialNumber });
    }
}