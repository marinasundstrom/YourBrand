using Microsoft.EntityFrameworkCore;

using Newtonsoft.Json;

using YourBrand.Domain;
using YourBrand.Identity;
using YourBrand.Marketing.Domain;
using YourBrand.Marketing.Domain.Common;
using YourBrand.Marketing.Domain.Entities;
using YourBrand.Marketing.Infrastructure.Persistence.Interceptors;
using YourBrand.Marketing.Infrastructure.Persistence.Outbox;
using YourBrand.Tenancy;

namespace YourBrand.Marketing.Infrastructure.Persistence;

public class MarketingContext : DbContext, IMarketingContext
{
    private readonly AuditableEntitySaveChangesInterceptor _auditableEntitySaveChangesInterceptor;

    public MarketingContext(
        DbContextOptions<MarketingContext> options,
        AuditableEntitySaveChangesInterceptor auditableEntitySaveChangesInterceptor) : base(options)
    {
        _auditableEntitySaveChangesInterceptor = auditableEntitySaveChangesInterceptor;
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        base.OnConfiguring(optionsBuilder);

        optionsBuilder.AddInterceptors(_auditableEntitySaveChangesInterceptor);

#if DEBUG
        optionsBuilder.EnableSensitiveDataLogging();
#endif
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.HasSequence<int>("MarketingIds");

        modelBuilder.ApplyConfigurationsFromAssembly(typeof(MarketingContext).Assembly);
    }

    protected override void ConfigureConventions(ModelConfigurationBuilder configurationBuilder)
    {
        configurationBuilder.AddTenantIdConverter();
        configurationBuilder.AddOrganizationIdConverter();
        configurationBuilder.AddUserIdConverter();
    }

    public DbSet<Contact> Contacts { get; set; } = null!;

    public DbSet<Campaign> Campaigns { get; set; } = null!;

    public DbSet<Address> Addresses { get; set; } = null!;

    public DbSet<Discount> Discounts { get; set; } = null!;

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        var entities = ChangeTracker
                        .Entries<Entity>()
                        .Where(e => e.Entity.DomainEvents.Any())
                        .Select(e => e.Entity);

        var domainEvents = entities
            .SelectMany(e => e.DomainEvents)
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