using YourBrand.Customers.Application.Common.Interfaces;
using YourBrand.Customers.Domain;
using YourBrand.Customers.Domain.Common;
using YourBrand.Customers.Domain.Entities;

using Microsoft.EntityFrameworkCore;

namespace YourBrand.Customers.Infrastructure.Persistence;

public class CustomersContext : DbContext, ICustomersContext
{
    private IDomainEventService _domainEventService;
    private ICurrentUserService _currentUserService;
    private IDateTime _dateTime;

    public CustomersContext(
        DbContextOptions<CustomersContext> options,
        IDomainEventService domainEventService,
        ICurrentUserService currentUserService,
        IDateTime dateTime) : base(options)
    {
        _domainEventService = domainEventService;
        _currentUserService = currentUserService;
        _dateTime = dateTime;
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.ApplyConfigurationsFromAssembly(typeof(CustomersContext).Assembly);
    }

    public DbSet<Person> Persons { get; set; } = null!;

    public DbSet<Address> Addresses { get; set; }  = null!;

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        foreach (var entry in ChangeTracker.Entries<AuditableEntity>())
        {
            if (entry.State == EntityState.Added)
            {
                entry.Entity.Created = _dateTime.Now;
                entry.Entity.CreatedById = _currentUserService.UserId!;
            }
            else if (entry.State == EntityState.Modified)
            {
                entry.Entity.LastModified = _dateTime.Now;
                entry.Entity.LastModifiedById = _currentUserService.UserId;
            }
            else if (entry.State == EntityState.Deleted
                && entry.Entity is ISoftDelete e)
            {
                e.Deleted = _dateTime.Now;
                e.DeletedById = _currentUserService.UserId;

                entry.State = EntityState.Modified;
            }
        }

        await DispatchEvents();

        return await base.SaveChangesAsync(cancellationToken);
    }

    private async Task DispatchEvents()
    {
        var entities = ChangeTracker
            .Entries<BaseEntity>()
            .Where(e => e.Entity.DomainEvents.Any())
            .Select(e => e.Entity);

        var domainEvents = entities
            .SelectMany(e => e.DomainEvents)
            .ToList();

        entities.ToList().ForEach(e => e.ClearDomainEvents());

        foreach (var domainEvent in domainEvents)
            await _domainEventService.Publish(domainEvent);
    }
}