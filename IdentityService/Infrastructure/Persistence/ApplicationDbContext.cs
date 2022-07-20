
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

using YourBrand.Identity;
using YourBrand.IdentityService.Application.Common.Interfaces;
using YourBrand.IdentityService.Domain.Common;
using YourBrand.IdentityService.Domain.Common.Interfaces;
using YourBrand.IdentityService.Domain.Entities;
using YourBrand.IdentityService.Infrastructure.Persistence.Configurations;
using YourBrand.IdentityService.Infrastructure.Persistence.Interceptors;

namespace YourBrand.IdentityService.Infrastructure.Persistence;

public class ApplicationDbContext : IdentityDbContext<Person, Role, string, IdentityUserClaim<string>, PersonRole, IdentityUserLogin<string>, IdentityRoleClaim<string>, IdentityUserToken<string>>, IApplicationDbContext
{
    private readonly ICurrentUserService _currentUserService;
    private readonly IDomainEventService _domainEventService;
    private readonly IDateTime _dateTime;
    private readonly AuditableEntitySaveChangesInterceptor _auditableEntitySaveChangesInterceptor;

    public ApplicationDbContext(
        DbContextOptions<ApplicationDbContext> options,
        ICurrentUserService currentUserService,
        IDomainEventService domainEventService,
        IDateTime dateTime,
        AuditableEntitySaveChangesInterceptor auditableEntitySaveChangesInterceptor) : base(options)
    {
        _currentUserService = currentUserService;
        _domainEventService = domainEventService;
        _dateTime = dateTime;
        _auditableEntitySaveChangesInterceptor = auditableEntitySaveChangesInterceptor;
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
#if DEBUG
        optionsBuilder.EnableSensitiveDataLogging(); //.LogTo(Console.WriteLine);
#endif

        base.OnConfiguring(optionsBuilder);

        optionsBuilder.AddInterceptors(_auditableEntitySaveChangesInterceptor);
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.ApplyConfigurationsFromAssembly(typeof(PersonConfiguration).Assembly);
    }

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        await DispatchEvents();

        return await base.SaveChangesAsync(cancellationToken);
    }

    private async Task DispatchEvents()
    {
        var entities = ChangeTracker
            .Entries<IHasDomainEvents>()
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