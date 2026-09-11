using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Nms.Domain.Entities;

namespace Nms.Infrastructure.Data.Configurations;

public class SecurityAdvisoryConfiguration : IEntityTypeConfiguration<SecurityAdvisory>
{
    public void Configure(EntityTypeBuilder<SecurityAdvisory> builder)
    {
        builder.ToTable("SecurityAdvisories");

        builder.HasKey(sa => sa.Id);

        builder.Property(sa => sa.TenantId)
            .IsRequired();

        builder.Property(sa => sa.AdvisoryId)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(sa => sa.Vendor)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(sa => sa.Title)
            .IsRequired()
            .HasMaxLength(250);

        builder.Property(sa => sa.Summary)
            .HasMaxLength(4000);

        builder.Property(sa => sa.Severity)
            .IsRequired();

        builder.Property(sa => sa.RemediationGuidance)
            .HasMaxLength(4000);

        builder.Property(sa => sa.ReferenceUrl)
            .HasMaxLength(500);

        builder.HasIndex(sa => new { sa.TenantId, sa.AdvisoryId })
            .IsUnique();

        builder.HasIndex(sa => new { sa.TenantId, sa.Vendor });
        builder.HasIndex(sa => new { sa.TenantId, sa.Severity });
    }
}