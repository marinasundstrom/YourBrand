
using Microsoft.EntityFrameworkCore;

using Newtonsoft.Json;

using YourBrand.ApiKeys.Application.Common.Interfaces;
using YourBrand.ApiKeys.Domain.Common;
using YourBrand.ApiKeys.Domain.Entities;
using YourBrand.ApiKeys.Infrastructure.Persistence.Interceptors;
using YourBrand.ApiKeys.Infrastructure.Persistence.Outbox;
using YourBrand.Domain;
using YourBrand.Identity;
using YourBrand.Tenancy;

namespace YourBrand.ApiKeys.Infrastructure.Persistence;

public class ApiKeysContext : DbContext, IApiKeysContext
{
    private readonly AuditableEntitySaveChangesInterceptor _auditableEntitySaveChangesInterceptor;

    public ApiKeysContext(
        DbContextOptions<ApiKeysContext> options,
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

        modelBuilder.ApplyConfigurationsFromAssembly(typeof(Configurations.ItemConfiguration).Assembly);
    }

    protected override void ConfigureConventions(ModelConfigurationBuilder configurationBuilder)
    {
        configurationBuilder.AddTenantIdConverter();
        configurationBuilder.AddOrganizationIdConverter();
        configurationBuilder.AddUserIdConverter();
    }

#nullable disable

    public DbSet<ApiKey> ApiKeys { get; set; } = null!;

    public DbSet<ApiKeyService> ApiKeyServices { get; set; } = null!;

    public DbSet<Domain.Entities.Application> Applications { get; set; } = null!;

    public DbSet<Resource> Resources { get; set; } = null!;

    public DbSet<Service> Services { get; set; } = null!;

    public DbSet<User> Users { get; set; } = null!;

#nullable restore

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