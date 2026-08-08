using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Nms.Domain.Entities;

namespace Nms.Infrastructure.Data.Configurations;

public class PollProfileConfiguration : IEntityTypeConfiguration<PollProfile>
{
    public void Configure(EntityTypeBuilder<PollProfile> builder)
    {
        builder.ToTable("PollProfiles");

        builder.HasKey(p => p.Id);

        builder.Property(p => p.Name)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(p => p.Description)
            .HasMaxLength(500);

        builder.Property(p => p.IntervalSeconds)
            .IsRequired()
            .HasDefaultValue(60);

        builder.Property(p => p.TimeoutSeconds)
            .IsRequired()
            .HasDefaultValue(5);

        builder.Property(p => p.RetryCount)
            .IsRequired()
            .HasDefaultValue(3);

        builder.Property(p => p.IsDefault)
            .IsRequired()
            .HasDefaultValue(false);

        builder.Property(p => p.IsEnabled)
            .IsRequired()
            .HasDefaultValue(true);

        builder.HasIndex(p => new { p.TenantId, p.IsEnabled });
    }
}