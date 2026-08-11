using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Nms.Domain.Entities;

namespace Nms.Infrastructure.Data.Configurations;

public class DiscoveryJobConfiguration : IEntityTypeConfiguration<DiscoveryJob>
{
    public void Configure(EntityTypeBuilder<DiscoveryJob> builder)
    {
        builder.ToTable("DiscoveryJobs");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Name)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(x => x.IpRange)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(x => x.SnmpCommunity)
            .HasMaxLength(100);

        builder.Property(x => x.Status)
            .IsRequired();

        builder.Property(x => x.FailureReason)
            .HasMaxLength(500);

        builder.HasIndex(x => new { x.TenantId, x.Status });

        builder.HasMany(x => x.Candidates)
            .WithOne(c => c.DiscoveryJob)
            .HasForeignKey(c => c.DiscoveryJobId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}