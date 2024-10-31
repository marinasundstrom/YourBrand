using System.Linq.Expressions;

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

            entityTypeBuilder.AddTenantIndex();

            entityTypeBuilder.AddOrganizationIndex();

            try
            {
                entityTypeBuilder.RegisterQueryFilters(builder =>
                {
                    builder.AddTenancyFilter(tenantContext);
                    builder.AddSoftDeleteFilter();
                });
            }
            catch (InvalidOperationException exc)
                when (exc.MatchQueryFilterExceptions(clrType))
            { }
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