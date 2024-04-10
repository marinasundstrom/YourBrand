
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

using Newtonsoft.Json;

using YourBrand.Identity;
using YourBrand.Domain;
using YourBrand.Tenancy;
using YourBrand.IdentityManagement.Application.Common.Interfaces;
using YourBrand.IdentityManagement.Domain.Common;
using YourBrand.IdentityManagement.Domain.Entities;
using YourBrand.IdentityManagement.Infrastructure.Persistence.Configurations;
using YourBrand.IdentityManagement.Infrastructure.Persistence.Interceptors;
using YourBrand.IdentityManagement.Infrastructure.Persistence.Outbox;

namespace YourBrand.IdentityManagement.Infrastructure.Persistence;

public class ApplicationDbContext(
    DbContextOptions<ApplicationDbContext> options,
    AuditableEntitySaveChangesInterceptor auditableEntitySaveChangesInterceptor) : IdentityDbContext<User, Role, string, IdentityUserClaim<string>, UserRole, IdentityUserLogin<string>, IdentityRoleClaim<string>, IdentityUserToken<string>>(options), IApplicationDbContext
{
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
#if DEBUG
        optionsBuilder.EnableSensitiveDataLogging(); //.LogTo(Console.WriteLine);
#endif

        base.OnConfiguring(optionsBuilder);

        optionsBuilder.AddInterceptors(auditableEntitySaveChangesInterceptor);
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.ApplyConfigurationsFromAssembly(typeof(UserConfiguration).Assembly);
    }

    public DbSet<Tenant> Tenants { get; set; } = default!;

    public DbSet<Organization> Organizations { get; set; } = default!;

    public DbSet<OrganizationUser> OrganizationUsers { get; set; } = default!;

    protected override void ConfigureConventions(ModelConfigurationBuilder configurationBuilder)
    {
        configurationBuilder.AddTenantIdConverter();
        configurationBuilder.AddOrganizationIdConverter();
        configurationBuilder.AddUserIdConverter();
    }

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

        var r = await base.SaveChangesAsync(cancellationToken);

        entities.ToList().ForEach(x => x.ClearDomainEvents());

        return r;
    }
}