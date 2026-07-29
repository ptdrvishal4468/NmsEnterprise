using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Nms.Domain.Entities;

namespace Nms.Infrastructure.Data.Configurations;

public class PermissionConfiguration : IEntityTypeConfiguration<Permission>
{
    public void Configure(EntityTypeBuilder<Permission> builder)
    {
        builder.ToTable("Permissions");

        builder.HasKey(p => p.Id);

        builder.Property(p => p.PermissionKey)
               .IsRequired()
               .HasMaxLength(100)
               .IsUnicode(false);

        builder.HasIndex(p => p.PermissionKey)
               .IsUnique();

        builder.Property(p => p.Description)
               .HasMaxLength(250);
    }
}