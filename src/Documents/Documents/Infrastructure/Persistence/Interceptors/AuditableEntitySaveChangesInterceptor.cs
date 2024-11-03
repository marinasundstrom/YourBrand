using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Diagnostics;

using YourBrand.Auditability;
using YourBrand.Documents.Application.Common.Interfaces;
using YourBrand.Documents.Domain.Common;
using YourBrand.Domain;
using YourBrand.Identity;
using YourBrand.Tenancy;

namespace YourBrand.Documents.Infrastructure.Persistence.Interceptors;

public class AuditableEntitySaveChangesInterceptor(
    ITenantContext tenantContext,
    IUserContext userContext,
    TimeProvider timeProvider) : SaveChangesInterceptor
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

        /*
        foreach (var entry in context.ChangeTracker.Entries<IAuditableEntity>())
        {
            if (entry.State == EntityState.Added)
            {
                entry.Entity.CreatedById = userContext.UserId;
                entry.Entity.Created = timeProvider.GetUtcNow();

                if (entry.Entity is IHasTenant hasTenant)
                {
                    hasTenant.TenantId = tenantContext.TenantId.GetValueOrDefault();
                }
            }
            else if (entry.State == EntityState.Modified || entry.HasChangedOwnedEntities())
            {
                entry.Entity.LastModifiedById = userContext.UserId;
                entry.Entity.LastModified = timeProvider.GetUtcNow();
            }
            else if (entry.State == EntityState.Deleted)
            {
                if (entry.Entity is ISoftDeletable softDelete)
                {
                    softDelete.IsDeleted = true;

                    if (entry.Entity is ISoftDeletableWithAudit softDelete2)
                    {
                        softDelete2.DeletedById = userContext.UserId;
                        softDelete2.Deleted = timeProvider.GetUtcNow();
                    }

                    entry.State = EntityState.Modified;
                }

                if (entry.Entity is IDeletable e2)
                {
                    entry.Entity.AddDomainEvent(e2.GetDeleteEvent());
                }
            }
        }*/

        foreach (var entry in context.ChangeTracker.Entries<IHasTenant>())
        {
            if (entry.State == EntityState.Added)
            {
                entry.Entity.TenantId = tenantContext.TenantId.GetValueOrDefault();
            }
        }

        foreach (var entry in context.ChangeTracker.Entries<IAuditableEntity>())
        {
            if (entry.State == EntityState.Added)
            {
                entry.Entity.CreatedById = userContext.UserId;
                entry.Entity.Created = timeProvider.GetUtcNow();
            }
            else if (entry.State == EntityState.Modified || entry.HasChangedOwnedEntities())
            {
                entry.Entity.LastModifiedById = userContext.UserId;
                entry.Entity.LastModified = timeProvider.GetUtcNow();
            }
        }

        foreach (var entry in context.ChangeTracker.Entries<ISoftDeletable>())
        {
            if (entry.State == EntityState.Deleted)
            {
                if (entry.Entity is ISoftDeletable softDelete)
                {
                    softDelete.IsDeleted = true;

                    if (entry.Entity is ISoftDeletableWithAudit softDelete2)
                    {
                        softDelete2.DeletedById = userContext.UserId;
                        softDelete2.Deleted = timeProvider.GetUtcNow();
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