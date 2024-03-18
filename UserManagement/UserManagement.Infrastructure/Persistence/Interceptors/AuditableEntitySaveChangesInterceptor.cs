using System;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Diagnostics;

using YourBrand.UserManagement.Application.Common.Interfaces;
using YourBrand.UserManagement.Domain.Common;
using YourBrand.UserManagement.Domain.Common.Interfaces;
using YourBrand.Identity;

namespace YourBrand.UserManagement.Infrastructure.Persistence.Interceptors;

public class AuditableEntitySaveChangesInterceptor : SaveChangesInterceptor
{
    private readonly ICurrentUserService _currentPersonService;
    private readonly IDateTime _dateTime;

    public AuditableEntitySaveChangesInterceptor(
        ICurrentUserService currentPersonService,
        IDateTime dateTime)
    {
        _currentPersonService = currentPersonService;
        _dateTime = dateTime;
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

        foreach (var entry in context.ChangeTracker.Entries<AuditableEntity>())
        {
            if (entry.State == EntityState.Added)
            {
                entry.Entity.CreatedBy = _currentPersonService.UserId;
                entry.Entity.Created = _dateTime.Now;
            }
            else if (entry.State == EntityState.Modified || entry.HasChangedOwnedEntities())
            {
                entry.Entity.LastModifiedBy = _currentPersonService.UserId;
                entry.Entity.LastModified = _dateTime.Now;
            }
            else if (entry.State == EntityState.Deleted)
            {
                if (entry.Entity is ISoftDelete softDelete)
                {
                    softDelete.DeletedBy = _currentPersonService.UserId;
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