using Microsoft.EntityFrameworkCore;

using Newtonsoft.Json;

using YourBrand.HumanResources.Application.Common.Interfaces;
using YourBrand.HumanResources.Domain.Common;
using YourBrand.HumanResources.Domain.Entities;
using YourBrand.HumanResources.Infrastructure.Persistence.Configurations;
using YourBrand.HumanResources.Infrastructure.Persistence.Interceptors;
using YourBrand.HumanResources.Infrastructure.Persistence.Outbox;
using YourBrand.Identity;

namespace YourBrand.HumanResources.Infrastructure.Persistence;

public class ApplicationDbContext : DbContext, IApplicationDbContext
{
    private readonly IUserContext _currentPersonService;
    private readonly IDateTime _dateTime;
    private readonly AuditableEntitySaveChangesInterceptor _auditableEntitySaveChangesInterceptor;

    public ApplicationDbContext(
        DbContextOptions<ApplicationDbContext> options,
        IUserContext currentPersonService,
        IDateTime dateTime,
        AuditableEntitySaveChangesInterceptor auditableEntitySaveChangesInterceptor) : base(options)
    {
        _currentPersonService = currentPersonService;
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

        return await base.SaveChangesAsync(cancellationToken);
    }
}