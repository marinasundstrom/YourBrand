﻿using System.Linq.Expressions;

using LinqKit;

using Microsoft.EntityFrameworkCore;

using Newtonsoft.Json;

using YourBrand.Accounting.Application.Common.Interfaces;
using YourBrand.Accounting.Domain.Common;
using YourBrand.Accounting.Domain.Entities;
using YourBrand.Accounting.Infrastructure.Persistence.Configurations;
using YourBrand.Accounting.Infrastructure.Persistence.Interceptors;
using YourBrand.Accounting.Infrastructure.Persistence.Outbox;
using YourBrand.Domain;
using YourBrand.Identity;
using YourBrand.Tenancy;

namespace YourBrand.Accounting.Infrastructure.Persistence;

public class AccountingContext(DbContextOptions<AccountingContext> options,
    ITenantContext tenantContext) : DbContext(options), IAccountingContext
{
    public async Task<Account?> GetAccount(int number, CancellationToken cancellationToken = default) => await Accounts.FirstOrDefaultAsync(x => x.AccountNo == number, cancellationToken);

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AccountConfiguration).Assembly);

        ConfigQueryFilterForEntity(modelBuilder);
    }

    private void ConfigQueryFilterForEntity(ModelBuilder modelBuilder)
    {
        foreach (var clrType in modelBuilder.Model
                    .GetEntityTypes()
                    .Select(entityType => entityType.ClrType))
        {
            if (!clrType.IsAssignableTo(typeof(Entity)))
            {
                continue;
            }

            var entityTypeBuilder = modelBuilder.Entity(clrType);

            if (clrType.IsAssignableTo(typeof(IHasTenant)))
            {
                entityTypeBuilder.HasIndex(nameof(IHasTenant.TenantId));
            }

            if (clrType.IsAssignableTo(typeof(IHasOrganization)))
            {
                entityTypeBuilder.HasIndex(nameof(IHasOrganization.OrganizationId));
            }

            try
            {
                var parameter = Expression.Parameter(clrType, "entity");

                List<Expression> queryFilters = new();

                if (TenancyQueryFilter.CanApplyTo(clrType))
                {
                    var tenantFilter = TenancyQueryFilter.GetFilter(() => tenantContext.TenantId);

                    queryFilters.Add(
                        Expression.Invoke(tenantFilter, Expression.Convert(parameter, typeof(IHasTenant))));
                }

                if (SoftDeleteQueryFilter.CanApplyTo(clrType))
                {
                    var softDeleteFilter = SoftDeleteQueryFilter.GetFilter();

                    queryFilters.Add(
                        Expression.Invoke(softDeleteFilter, Expression.Convert(parameter, typeof(ISoftDelete))));
                }

                Expression? queryFilter = null;

                foreach (var qf in queryFilters)
                {
                    if (queryFilter is null)
                    {
                        queryFilter = qf;
                    }
                    else
                    {
                        queryFilter = Expression.AndAlso(
                            queryFilter,
                            qf)
                            .Expand();
                    }
                }

                if (queryFilters.Count == 0)
                {
                    continue;
                }

                var queryFilterLambda = Expression.Lambda(queryFilter.Expand(), parameter);

                entityTypeBuilder.HasQueryFilter(queryFilterLambda);
            }
            catch (InvalidOperationException exc)
                when (exc.Message.Contains("cannot be configured as non-owned because it has already been configured as a owned"))
            {
                Console.WriteLine("Skipping previously configured owned type");
            }
            catch (InvalidOperationException exc)
                when (exc.Message.Contains("cannot be added to the model because its CLR type has been configured as a shared type"))
            {
                Console.WriteLine("Skipping previously configured shared type");
            }
        }
    }

    protected override void ConfigureConventions(ModelConfigurationBuilder configurationBuilder)
    {
        configurationBuilder.AddTenantIdConverter();
        configurationBuilder.AddOrganizationIdConverter();
        configurationBuilder.AddUserIdConverter();
    }

#nullable disable

    public DbSet<Account> Accounts { get; set; }

    public DbSet<LedgerEntry> LedgerEntries { get; set; }

    public DbSet<JournalEntry> JournalEntries { get; set; }

    public DbSet<Verification> Verifications { get; set; }

#nullable restore

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        var entities = ChangeTracker
                        .Entries<Entity>()
                        .Where(e => e.Entity.DomainEvents.Any())
                        .Select(e => e.Entity);

        if (!entities.Any())
        {
            return await base.SaveChangesAsync(cancellationToken);
        }

        var domainEvents = entities
            .SelectMany(entity =>
            {
                var domainEvents = entity.DomainEvents.ToList();

                entity.ClearDomainEvents();

                return domainEvents;
            })
            .OrderBy(e => e.Timestamp)
            .ToList();

        var outboxMessages = domainEvents.Select(domainEvent =>
        {
            return new OutboxMessage()
            {
                Id = domainEvent.Id,
                OccurredOnUtc = DateTime.UtcNow,
                Type = domainEvent.GetType().Name,
                Content = JsonConvert.SerializeObject(
                    domainEvent,
                    new JsonSerializerSettings
                    {
                        TypeNameHandling = TypeNameHandling.All
                    })
            };
        }).ToList();

        this.Set<OutboxMessage>().AddRange(outboxMessages);

        return await base.SaveChangesAsync(cancellationToken);
    }
}