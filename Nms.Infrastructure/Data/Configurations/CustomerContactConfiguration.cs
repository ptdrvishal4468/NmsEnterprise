using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Nms.Domain.Entities;

namespace Nms.Infrastructure.Data.Configurations;

public class CustomerContactConfiguration : IEntityTypeConfiguration<CustomerContact>
{
    public void Configure(EntityTypeBuilder<CustomerContact> builder)
    {
        builder.ToTable("CustomerContacts");

        builder.HasKey(cc => cc.Id);

        builder.Property(cc => cc.TenantId)
            .IsRequired();

        builder.Property(cc => cc.CustomerId)
            .IsRequired();

        builder.Property(cc => cc.FirstName)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(cc => cc.LastName)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(cc => cc.Email)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(cc => cc.PhoneNumber)
            .HasMaxLength(50);

        builder.Property(cc => cc.JobTitle)
            .HasMaxLength(100);

        builder.Property(cc => cc.ContactType)
            .IsRequired();

        builder.Property(cc => cc.IsPrimary)
            .IsRequired();

        builder.Property(cc => cc.CreatedBy)
            .HasMaxLength(200);

        builder.Property(cc => cc.LastModifiedBy)
            .HasMaxLength(200);

        // Multi-Tenant Indexes
        builder.HasIndex(cc => new { cc.TenantId, cc.CustomerId, cc.Email })
            .IsUnique();

        builder.HasIndex(cc => new { cc.TenantId, cc.CustomerId });
        builder.HasIndex(cc => new { cc.TenantId, cc.Email });
        builder.HasIndex(cc => new { cc.TenantId, cc.IsPrimary });
    }
}