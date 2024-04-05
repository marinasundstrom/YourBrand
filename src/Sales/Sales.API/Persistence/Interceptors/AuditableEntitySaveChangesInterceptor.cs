using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Diagnostics;

using YourBrand.Sales.Domain.Entities;

using YourBrand.Domain;

namespace YourBrand.Sales.Persistence.Interceptors;

public sealed class AuditableEntitySaveChangesInterceptor : SaveChangesInterceptor
{
    private readonly IUserContext _userContext;
    private readonly IDateTime _dateTime;
    private readonly ITenantContext _tenantContext;

    public AuditableEntitySaveChangesInterceptor(
        IUserContext userContext,
        IDateTime dateTime,
        ITenantContext tenantContext)
    {
        _userContext = userContext;
        _dateTime = dateTime;
        _tenantContext = tenantContext;
    }

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

                entry.Entity.TenantId = _tenantContext.TenantId.GetValueOrDefault();
            }
        }

        foreach (var entry in context.ChangeTracker.Entries<IAuditable>())
        {
            if (entry.State == EntityState.Added)
            {
                entry.Entity.CreatedById = _userContext.UserId!;
                entry.Entity.Created = _dateTime.Now;
            }
            else if (entry.State == EntityState.Modified || entry.HasChangedOwnedEntities())
            {
                entry.Entity.LastModifiedById = _userContext.UserId;
                entry.Entity.LastModified = _dateTime.Now;
            }
            else if (entry.State == EntityState.Deleted)
            {
                if (entry.Entity is ISoftDelete softDelete)
                {
                    softDelete.DeletedById = _userContext.UserId;
                    softDelete.Deleted = _dateTime.Now;

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