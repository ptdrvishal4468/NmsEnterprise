using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Nms.Domain.Entities;

namespace Nms.Infrastructure.Data.Configurations;

public class CustomerConfiguration : IEntityTypeConfiguration<Customer>
{
    public void Configure(EntityTypeBuilder<Customer> builder)
    {
        builder.ToTable("Customers");

        builder.HasKey(c => c.Id);

        builder.Property(c => c.TenantId)
            .IsRequired();

        builder.Property(c => c.Name)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(c => c.Code)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(c => c.AccountNumber)
            .HasMaxLength(100);

        builder.Property(c => c.Description)
            .HasMaxLength(500);

        builder.Property(c => c.Status)
            .IsRequired();

        builder.Property(c => c.Tier)
            .IsRequired();

        builder.Property(c => c.MetadataJson);

        builder.Property(c => c.CreatedBy)
            .HasMaxLength(200);

        builder.Property(c => c.LastModifiedBy)
            .HasMaxLength(200);

        // Self-referencing Parent/Child Hierarchy Relationship
        builder.HasOne(c => c.ParentCustomer)
            .WithMany(c => c.ChildCustomers)
            .HasForeignKey(c => c.ParentCustomerId)
            .OnDelete(DeleteBehavior.Restrict);

        // Contacts Relationship
        builder.HasMany(c => c.Contacts)
            .WithOne(cc => cc.Customer)
            .HasForeignKey(cc => cc.CustomerId)
            .OnDelete(DeleteBehavior.Cascade);

        // Multi-Tenant Indexes
        builder.HasIndex(c => new { c.TenantId, c.Code })
            .IsUnique();

        builder.HasIndex(c => new { c.TenantId, c.Name });
        builder.HasIndex(c => new { c.TenantId, c.Status });
        builder.HasIndex(c => new { c.TenantId, c.Tier });
        builder.HasIndex(c => new { c.TenantId, c.ParentCustomerId });
    }
}