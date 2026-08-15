using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Nms.Domain.Entities;

namespace Nms.Infrastructure.Data.Configurations;

public class ScheduledReportConfiguration : IEntityTypeConfiguration<ScheduledReport>
{
    public void Configure(EntityTypeBuilder<ScheduledReport> builder)
    {
        builder.ToTable("ScheduledReports");

        builder.HasKey(sr => sr.Id);

        builder.Property(sr => sr.Name)
            .IsRequired()
            .HasMaxLength(150);

        builder.Property(sr => sr.RecipientEmail)
            .IsRequired()
            .HasMaxLength(256);

        builder.Property(sr => sr.ReportType)
            .IsRequired()
            .HasConversion<string>()
            .HasMaxLength(50);

        builder.Property(sr => sr.ScheduleFrequency)
            .IsRequired()
            .HasConversion<string>()
            .HasMaxLength(50);

        builder.Property(sr => sr.OutputFormat)
            .IsRequired()
            .HasConversion<string>()
            .HasMaxLength(20);

        builder.Property(sr => sr.FilterJson)
            .HasMaxLength(4000);

        builder.Property(sr => sr.IsActive)
            .IsRequired();

        builder.Property(sr => sr.TenantId)
            .IsRequired();

        builder.HasIndex(sr => sr.TenantId);
        builder.HasIndex(sr => new { sr.IsActive, sr.NextRunUtc });
    }
}