using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Nms.Domain.Entities;

namespace Nms.Infrastructure.Data.Configurations;

public class RoleConfiguration : IEntityTypeConfiguration<Role>
{
    public void Configure(EntityTypeBuilder<Role> builder)
    {
        builder.ToTable("Roles");

        builder.HasKey(r => r.Id);
        builder.Property(r => r.Id)
               .HasDefaultValueSql("NEWSEQUENTIALID()");

        builder.Property(r => r.TenantId)
               .IsRequired();

        builder.Property(r => r.Name)
               .IsRequired()
               .HasMaxLength(50);

        builder.Property(r => r.IsSystemDefault)
               .IsRequired()
               .HasDefaultValue(false);

        builder.HasOne<Tenant>()
               .WithMany()
               .HasForeignKey(r => r.TenantId)
               .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(r => r.RolePermissions)
               .WithOne()
               .HasForeignKey(rp => rp.RoleId)
               .OnDelete(DeleteBehavior.Cascade);
    }
}