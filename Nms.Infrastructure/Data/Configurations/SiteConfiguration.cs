using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Nms.Domain.Entities;

namespace Nms.Infrastructure.Data.Configurations;

public class SiteConfiguration : IEntityTypeConfiguration<Site>
{
    public void Configure(EntityTypeBuilder<Site> builder)
    {
        builder.ToTable("Sites");

        builder.HasKey(s => s.Id);

        builder.Property(s => s.TenantId)
            .IsRequired();

        builder.Property(s => s.Name)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(s => s.Code)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(s => s.Description)
            .HasMaxLength(500);

        builder.Property(s => s.Address)
            .HasMaxLength(500);

        builder.Property(s => s.City)
            .HasMaxLength(100);

        builder.Property(s => s.StateOrProvince)
            .HasMaxLength(100);

        builder.Property(s => s.PostalCode)
            .HasMaxLength(30);

        builder.Property(s => s.Country)
            .HasMaxLength(100);

        builder.Property(s => s.TimeZone)
            .HasMaxLength(100);

        builder.Property(s => s.CreatedBy)
            .HasMaxLength(200);

        builder.Property(s => s.LastModifiedBy)
            .HasMaxLength(200);

        // Multi-Tenant Indexes
        builder.HasIndex(s => new { s.TenantId, s.Code })
            .IsUnique();

        builder.HasIndex(s => new { s.TenantId, s.Name });
        builder.HasIndex(s => new { s.TenantId, s.IsActive });
    }
}