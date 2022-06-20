
using YourBrand.ApiKeys.Application.Common.Interfaces;
using YourBrand.ApiKeys.Domain.Common;
using YourBrand.ApiKeys.Domain.Entities;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;

namespace YourBrand.ApiKeys.Infrastructure.Persistence;

class ApiKeysContext : DbContext, IApiKeysContext
{
    private readonly ICurrentUserService _currentUserService;
    private readonly IDomainEventService _domainEventService;
    private readonly IDateTime _dateTime;

    public ApiKeysContext(
        DbContextOptions<ApiKeysContext> options,
        ICurrentUserService currentUserService,
        IDomainEventService domainEventService,
        IDateTime dateTime) : base(options)
    {
        _currentUserService = currentUserService;
        _domainEventService = domainEventService;
        _dateTime = dateTime;
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
#if DEBUG
        optionsBuilder.EnableSensitiveDataLogging(); //.LogTo(Console.WriteLine);
#endif

        base.OnConfiguring(optionsBuilder);
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.ApplyConfigurationsFromAssembly(typeof(Configurations.ItemConfiguration).Assembly);
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
        foreach (Microsoft.EntityFrameworkCore.ChangeTracking.EntityEntry<AuditableEntity> entry in ChangeTracker.Entries<AuditableEntity>())
        {
            switch (entry.State)
            {
                case EntityState.Added:
                    entry.Entity.CreatedById = _currentUserService.UserId;
                    entry.Entity.Created = _dateTime.Now;
                    break;

                case EntityState.Modified:
                    entry.Entity.LastModifiedById = _currentUserService.UserId;
                    entry.Entity.LastModified = _dateTime.Now;
                    break;

                case EntityState.Deleted:
                    if (entry.Entity is ISoftDelete softDelete)
                    {
                        softDelete.DeletedById = _currentUserService.UserId;
                        softDelete.Deleted = _dateTime.Now;

                        entry.State = EntityState.Modified;
                    }
                    break;
            }
        }

        DomainEvent[] events = GetDomainEvents();

        var result = await base.SaveChangesAsync(cancellationToken);

        await DispatchEvents(events);

        return result;
    }

    private DomainEvent[] GetDomainEvents()
    {
        return ChangeTracker.Entries<IHasDomainEvents>()
            .Select(x => x.Entity.DomainEvents)
            .SelectMany(x => x)
            .Where(domainEvent => !domainEvent.IsPublished)
            .ToArray();
    }

    private async Task DispatchEvents(DomainEvent[] events)
    {
        foreach (var @event in events)
        {
            @event.IsPublished = true;
            await _domainEventService.Publish(@event);
        }
    }
}