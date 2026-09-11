using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Nms.Domain.Common;

namespace Nms.Infrastructure.Data.Interceptors;

public class AuditableEntityInterceptor : SaveChangesInterceptor
{
    public override ValueTask<InterceptionResult<int>> SavingChangesAsync(
        DbContextEventData eventData,
        InterceptionResult<int> result,
        CancellationToken cancellationToken = default)
    {
        UpdateEntities(eventData.Context);
        return base.SavingChangesAsync(eventData, result, cancellationToken);
    }

    public override InterceptionResult<int> SavingChanges(
        DbContextEventData eventData,
        InterceptionResult<int> result)
    {
        UpdateEntities(eventData.Context);
        return base.SavingChanges(eventData, result);
    }

    private static void UpdateEntities(DbContext? context)
    {
        if (context == null) return;

        var entries = context.ChangeTracker.Entries();

        foreach (var entry in entries)
        {
            if (entry.Entity.GetType().BaseType is { IsGenericType: true } baseType &&
                baseType.GetGenericTypeDefinition() == typeof(AuditableEntity<>))
            {
                var utcNow = DateTime.UtcNow;

                if (entry.State == EntityState.Added)
                {
                    entry.Property("CreatedAtUtc").CurrentValue = utcNow;
                }

                if (entry.State == EntityState.Modified)
                {
                    entry.Property("LastModifiedAtUtc").CurrentValue = utcNow;
                }
            }
        }
    }
}