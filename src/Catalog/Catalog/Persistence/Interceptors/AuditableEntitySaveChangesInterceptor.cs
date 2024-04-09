using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Diagnostics;

using YourBrand.Catalog.Domain.Entities;
using YourBrand.Catalog.Services;
using YourBrand.Domain;
using YourBrand.Identity;
using YourBrand.Tenancy;

namespace YourBrand.Catalog.Persistence.Interceptors;

public sealed class AuditableEntitySaveChangesInterceptor(
    IUserContext userContext,
    IDateTime dateTime,
    ITenantContext tenantContext) : SaveChangesInterceptor
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

        foreach (var entry in context.ChangeTracker.Entries<IHasTenant>())
        {
            if (entry.State == EntityState.Added)
            {
                entry.Entity.TenantId = tenantContext.TenantId.GetValueOrDefault();
            }
        }

        foreach (var entry in context.ChangeTracker.Entries<IAuditable>())
        {
            if (entry.State == EntityState.Added)
            {
                entry.Entity.CreatedById = userContext.UserId.GetValueOrDefault();
                entry.Entity.Created = dateTime.Now;
            }
            else if (entry.State == EntityState.Modified || entry.HasChangedOwnedEntities())
            {
                entry.Entity.LastModifiedById = userContext.UserId;
                entry.Entity.LastModified = dateTime.Now;
            }
            else if (entry.State == EntityState.Deleted)
            {
                if (entry.Entity is ISoftDelete softDelete)
                {
                    softDelete.DeletedById = userContext.UserId;
                    softDelete.Deleted = dateTime.Now;

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