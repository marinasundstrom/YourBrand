
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

using Newtonsoft.Json;

using YourBrand.UserManagement.Application.Common.Interfaces;
using YourBrand.UserManagement.Domain.Common;
using YourBrand.UserManagement.Domain.Common.Interfaces;
using YourBrand.UserManagement.Domain.Entities;
using YourBrand.UserManagement.Infrastructure.Persistence.Configurations;
using YourBrand.UserManagement.Infrastructure.Persistence.Interceptors;
using YourBrand.UserManagement.Infrastructure.Persistence.Outbox;
using YourBrand.Identity;

namespace YourBrand.UserManagement.Infrastructure.Persistence;

public class ApplicationDbContext : DbContext, IApplicationDbContext
{
    private readonly ICurrentUserService _currentUserService;
    private readonly IDateTime _dateTime;
    private readonly AuditableEntitySaveChangesInterceptor _auditableEntitySaveChangesInterceptor;

    public ApplicationDbContext(
        DbContextOptions<ApplicationDbContext> options,
        ICurrentUserService currentUserService,
        IDateTime dateTime,
        AuditableEntitySaveChangesInterceptor auditableEntitySaveChangesInterceptor) : base(options)
    {
        _currentUserService = currentUserService;
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

        modelBuilder.ApplyConfigurationsFromAssembly(typeof(UserConfiguration).Assembly);
    }

    public DbSet<Role> Roles { get; set; } = null!;

    public DbSet<User> Users { get; set; } = null!;

    public DbSet<Organization> Organizations { get; set; } = null!;

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