using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Nms.Domain.Entities;

namespace Nms.Infrastructure.Data.Configurations;

public class NotificationTemplateConfiguration : IEntityTypeConfiguration<NotificationTemplate>
{
    public void Configure(EntityTypeBuilder<NotificationTemplate> builder)
    {
        builder.ToTable("NotificationTemplates");

        builder.HasKey(t => t.Id);

        builder.Property(t => t.Name)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(t => t.Channel)
            .IsRequired();

        builder.Property(t => t.SubjectTemplate)
            .IsRequired()
            .HasMaxLength(500);

        builder.Property(t => t.BodyTemplate)
            .IsRequired();

        builder.Property(t => t.RecipientTarget)
            .IsRequired()
            .HasMaxLength(500);

        builder.Property(t => t.IsEnabled)
            .IsRequired();

        builder.HasIndex(t => new { t.TenantId, t.Channel, t.IsEnabled })
            .HasDatabaseName("IX_NotificationTemplates_TenantId_Channel_IsEnabled");
    }
}