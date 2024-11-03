using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Logging;

using YourBrand.Domain;
using YourBrand.Identity;

namespace YourBrand.Auditability;

public sealed class AuditableEntitySaveChangesInterceptor(
    IUserContext userContext,
    TimeProvider timeProvider,
    ILogger<AuditableEntitySaveChangesInterceptor> logger) : SaveChangesInterceptor
{
    public override InterceptionResult<int> SavingChanges(DbContextEventData eventData, InterceptionResult<int> result)
    {
        UpdateEntities(eventData.Context);

        return base.SavingChanges(eventData, result);
    }

    public override ValueTask<InterceptionResult<int>> SavingChangesAsync(DbContextEventData eventData, InterceptionResult<int> result, CancellationToken cancellationToken = default)
    {
        UpdateEntities(eventData.Context);

        return base.SavingChangesAsync(eventData, result, cancellationToken);
    }

    public void UpdateEntities(DbContext? context)
    {
        if (context == null) return;

        foreach (var entry in context.ChangeTracker.Entries<IAuditableEntity>())
        {
            if (entry.State == EntityState.Added)
            {
                entry.Entity.CreatedById = userContext.UserId!;
                entry.Entity.Created = timeProvider.GetUtcNow();

                logger.LogInformation("Added audit to added entity of type {Type} with id {Id}", entry.Metadata.ClrType.Name, entry.Member("Id").CurrentValue);
            }
            else if (entry.State == EntityState.Modified || entry.HasChangedOwnedEntities())
            {
                entry.Entity.LastModifiedById = userContext.UserId;
                entry.Entity.LastModified = timeProvider.GetUtcNow();

                logger.LogInformation("Added audit to modified entity of type {Type} with id {Id}", entry.Metadata.ClrType.Name, entry.Member("Id").CurrentValue);
            }
        }
    }
}

public static class Extensions
{
    public static bool HasChangedOwnedEntities(this EntityEntry entry) =>
        entry.References.Any(r =>
            r.TargetEntry != null &&
            r.TargetEntry.Metadata.IsOwned() &&
            (r.TargetEntry.State == EntityState.Added || r.TargetEntry.State == EntityState.Modified));
}