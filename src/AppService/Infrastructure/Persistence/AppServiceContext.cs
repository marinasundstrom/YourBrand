using System.Linq.Expressions;

using LinqKit;

using Microsoft.EntityFrameworkCore;

using Newtonsoft.Json;

using YourBrand.Application.Common.Interfaces;
using YourBrand.Domain;
using YourBrand.Domain.Common;
using YourBrand.Domain.Entities;
using YourBrand.Identity;
using YourBrand.Infrastructure.Persistence.Interceptors;
using YourBrand.Infrastructure.Persistence.Outbox;
using YourBrand.Tenancy;

namespace YourBrand.Infrastructure.Persistence;

public class AppServiceContext(
    DbContextOptions<AppServiceContext> options, ITenantContext tenantContext) : DbContext(options), IAppServiceContext
{
    public TenantId? TenantId => tenantContext.TenantId;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.ApplyConfigurationsFromAssembly(typeof(Configurations.SearchResultItemConfiguration).Assembly);

        modelBuilder.ConfigureDomainModel(configurator =>
        {
            configurator.AddTenancyFilter(() => TenantId);
            configurator.AddSoftDeleteFilter();
        });
    }

    protected override void ConfigureConventions(ModelConfigurationBuilder configurationBuilder)
    {
        configurationBuilder.AddTenantIdConverter();
        configurationBuilder.AddOrganizationIdConverter();
        configurationBuilder.AddUserIdConverter();
    }

#nullable disable

    public DbSet<BrandProfile> BrandProfiles { get; set; } = null!;

    public DbSet<SearchResultItem> SearchResultItems { get; set; } = null!;

    public DbSet<Module> Modules { get; set; } = null!;

    public DbSet<TenantModule> TenantModules { get; set; } = null!;

    public DbSet<WidgetArea> WidgetAreas { get; set; } = null!;

    public DbSet<Widget> Widgets { get; set; } = null!;

    public DbSet<User> Users { get; set; } = null!;

#nullable restore

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        var entities = ChangeTracker
                        .Entries<IHasDomainEvents>()
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
                Id = Guid.NewGuid(),
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