using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Logging;

namespace YourBrand.Tenancy;

public sealed class SetTenantSaveChangesInterceptor(
    ITenantContext tenantContext,
    ILogger<SetTenantSaveChangesInterceptor> logger) : SaveChangesInterceptor
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

                logger.LogInformation("Tenant was assigned to entity of type {Type} with id {Id}", entry.Metadata.ClrType.Name, entry.Member("Id").CurrentValue);
            }
        }
    }
}