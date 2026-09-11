using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Nms.Domain.Entities;

namespace Nms.Infrastructure.Data.Configurations;

public class RefreshTokenConfiguration : IEntityTypeConfiguration<RefreshToken>
{
    public void Configure(EntityTypeBuilder<RefreshToken> builder)
    {
        builder.ToTable("RefreshTokens");

        builder.HasKey(rt => rt.Id);
        builder.Property(rt => rt.Id)
               .HasDefaultValueSql("NEWSEQUENTIALID()");

        builder.Property(rt => rt.TokenHash)
               .IsRequired()
               .HasMaxLength(256);

        builder.Property(rt => rt.ExpiresAtUtc)
               .IsRequired();

        builder.Property(rt => rt.IsRevoked)
               .IsRequired()
               .HasDefaultValue(false);

        builder.Property(rt => rt.CreatedAtUtc)
               .IsRequired()
               .HasDefaultValueSql("SYSUTCDATETIME()");
    }
}