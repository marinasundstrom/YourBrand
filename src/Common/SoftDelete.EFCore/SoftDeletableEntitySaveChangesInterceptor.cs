using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Logging;

using YourBrand.Domain;
using YourBrand.Identity;

namespace YourBrand.Auditability;

public sealed class SoftDeletableEntitySaveChangesInterceptor(
    IUserContext userContext,
    TimeProvider timeProvider,
    ILogger<SoftDeletableEntitySaveChangesInterceptor> logger) : SaveChangesInterceptor
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

        foreach (var entry in context.ChangeTracker.Entries<ISoftDeletable>())
        {
            if (entry.State == EntityState.Deleted)
            {
                if (entry.Entity is ISoftDeletable softDeletable)
                {
                    softDeletable.IsDeleted = true;

                    logger.LogInformation("Applied soft-delete to entity of type {Type} with id {Id}", entry.Metadata.ClrType.Name, entry.Member("Id").CurrentValue);

                    if (entry.Entity is ISoftDeletableWithAudit softDeletable2)
                    {
                        softDeletable2.Deleted = timeProvider.GetUtcNow();
                        softDeletable2.DeletedById = userContext.UserId;

                        logger.LogInformation("Applied audit data to soft-deleted entity of type {Type} with id {Id}", entry.Metadata.ClrType.Name, entry.Member("Id").CurrentValue);
                    }

                    entry.State = EntityState.Modified;
                }
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
