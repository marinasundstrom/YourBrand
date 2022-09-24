using YourBrand.Customers.Application.Common.Interfaces;
using YourBrand.Customers.Domain;
using YourBrand.Customers.Domain.Common;
using YourBrand.Customers.Domain.Entities;

using Microsoft.EntityFrameworkCore;
using YourBrand.Customers.Infrastructure.Persistence.Interceptors;
using Newtonsoft.Json;
using YourBrand.Customers.Infrastructure.Persistence.Outbox;

namespace YourBrand.Customers.Infrastructure.Persistence;

public class CustomersContext : DbContext, ICustomersContext
{
    private readonly AuditableEntitySaveChangesInterceptor _auditableEntitySaveChangesInterceptor;

    public CustomersContext(
        DbContextOptions<CustomersContext> options,
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

        modelBuilder.HasSequence<int>("CustomerIds");

        modelBuilder.ApplyConfigurationsFromAssembly(typeof(CustomersContext).Assembly);
    }

    public DbSet<Customer> Customers { get; set; } = null!;

    public DbSet<Person> Persons { get; set; } = null!;

    public DbSet<Organization> Organizations { get; set; } = null!;

    public DbSet<Address> Addresses { get; set; }  = null!;

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        var entities = ChangeTracker
                        .Entries<Entity>()
                        .Where(e => e.Entity.DomainEvents.Any())
                        .Select(e => e.Entity);

        var domainEvents = entities
            .SelectMany(e => e.DomainEvents)
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