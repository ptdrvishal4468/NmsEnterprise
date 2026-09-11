using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Nms.Domain.Entities;

namespace Nms.Infrastructure.Data.Configurations;

public class ScheduledReportExecutionLogConfiguration : IEntityTypeConfiguration<ScheduledReportExecutionLog>
{
    public void Configure(EntityTypeBuilder<ScheduledReportExecutionLog> builder)
    {
        builder.ToTable("ScheduledReportExecutionLogs");

        builder.HasKey(log => log.Id);

        builder.Property(log => log.Status)
            .IsRequired()
            .HasConversion<string>()
            .HasMaxLength(30);

        builder.Property(log => log.ErrorMessage)
            .HasMaxLength(2000);

        builder.Property(log => log.TenantId)
            .IsRequired();

        builder.HasOne(log => log.ScheduledReport)
            .WithMany()
            .HasForeignKey(log => log.ScheduledReportId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(log => log.TenantId);
        builder.HasIndex(log => log.ScheduledReportId);
        builder.HasIndex(log => log.ExecutedAtUtc);
    }
}