using Microsoft.EntityFrameworkCore;

using Newtonsoft.Json;

using YourBrand.Domain;
using YourBrand.HumanResources.Application.Common.Interfaces;
using YourBrand.HumanResources.Domain.Common;
using YourBrand.HumanResources.Domain.Entities;
using YourBrand.HumanResources.Infrastructure.Persistence.Configurations;
using YourBrand.HumanResources.Infrastructure.Persistence.Interceptors;
using YourBrand.HumanResources.Infrastructure.Persistence.Outbox;
using YourBrand.Identity;
using YourBrand.Tenancy;

namespace YourBrand.HumanResources.Infrastructure.Persistence;

public class ApplicationDbContext(
    DbContextOptions<ApplicationDbContext> options) : DbContext(options), IApplicationDbContext
{
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.ApplyConfigurationsFromAssembly(typeof(PersonConfiguration).Assembly);
    }

    protected override void ConfigureConventions(ModelConfigurationBuilder configurationBuilder)
    {
        configurationBuilder.AddTenantIdConverter();
        configurationBuilder.AddOrganizationIdConverter();
        configurationBuilder.AddUserIdConverter();
    }

    public DbSet<Organization> Organizations { get; set; } = null!;

    public DbSet<PersonDependant> PersonDependants { get; set; } = null!;

    public DbSet<Team> Teams { get; set; } = null!;

    public DbSet<TeamMembership> TeamMemberships { get; set; } = null!;

    public DbSet<Department> Departments { get; set; } = null!;

    public DbSet<BankAccount> BankAccounts { get; set; } = null!;

    public DbSet<Role> Roles { get; set; } = null!;

    public DbSet<Person> Persons { get; set; } = null!;

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