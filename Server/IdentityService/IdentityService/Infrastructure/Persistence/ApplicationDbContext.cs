
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

using YourCompany.IdentityService.Application.Common.Interfaces;
using YourCompany.IdentityService.Domain.Common;
using YourCompany.IdentityService.Domain.Common.Interfaces;
using YourCompany.IdentityService.Domain.Entities;
using YourCompany.IdentityService.Infrastructure.Persistence.Configurations;

namespace YourCompany.IdentityService.Infrastructure.Persistence;

public class ApplicationDbContext : IdentityDbContext<User, Role, string, IdentityUserClaim<string>, UserRole, IdentityUserLogin<string>, IdentityRoleClaim<string>, IdentityUserToken<string>>, IApplicationDbContext
{
    private readonly ICurrentUserService _currentUserService;
    private readonly IDomainEventService _domainEventService;
    private readonly IDateTime _dateTime;

    public ApplicationDbContext(
        DbContextOptions<ApplicationDbContext> options,
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

        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApplicationUserConfiguration).Assembly);

        modelBuilder.Entity<User>(entity =>
        {
            entity.ToTable(name: "Users");
        });

        modelBuilder.Entity<Role>(entity =>
        {
            entity.ToTable(name: "Roles");
        });

        modelBuilder.Entity<UserRole>(entity =>
        {
            entity.ToTable("UserRoles");
            //in case you chagned the TKey type
            //  entity.HasKey(key => new { key.UserId, key.RoleId });
        });

        modelBuilder.Entity<IdentityUserClaim<string>>(entity =>
        {
            entity.ToTable("UserClaims");
        });

        modelBuilder.Entity<IdentityUserLogin<string>>(entity =>
        {
            entity.ToTable("UserLogins");
            //in case you chagned the TKey type
            //  entity.HasKey(key => new { key.ProviderKey, key.LoginProvider });       
        });

        modelBuilder.Entity<IdentityRoleClaim<string>>(entity =>
        {
            entity.ToTable("RoleClaims");

        });

        modelBuilder.Entity<IdentityUserToken<string>>(entity =>
        {
            entity.ToTable("UserTokens");
            //in case you chagned the TKey type
            // entity.HasKey(key => new { key.UserId, key.LoginProvider, key.Name });

        });
    }

    public DbSet<Role> Roles { get; set; } = null!;

    public DbSet<UserDependant> UserDependants { get; set; } = null!;

    public DbSet<Team> Teams { get; set; } = null!;

    public DbSet<Department> Departments { get; set; } = null!;

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        foreach (var entry in ChangeTracker.Entries<IAuditableEntity>())
        {
            UpdateState(entry);
        }

        var events = ChangeTracker.Entries<IHasDomainEvent>()
            .Select(x => x.Entity.DomainEvents)
            .SelectMany(x => x)
            .Where(domainEvent => !domainEvent.IsPublished)
            .ToArray();

        var result = await base.SaveChangesAsync(cancellationToken);

        await DispatchEvents(events);

        return result;
    }

    private void UpdateState(Microsoft.EntityFrameworkCore.ChangeTracking.EntityEntry<IAuditableEntity> entry)
    {
        switch (entry.State)
        {
            case EntityState.Added:
                entry.Entity.CreatedBy = _currentUserService.UserId;
                entry.Entity.Created = _dateTime.Now;
                break;

            case EntityState.Modified:
                entry.Entity.LastModifiedBy = _currentUserService.UserId;
                entry.Entity.LastModified = _dateTime.Now;
                break;

            case EntityState.Deleted:
                if (entry.Entity is ISoftDelete softDelete)
                {
                    softDelete.DeletedBy = _currentUserService.UserId;
                    softDelete.Deleted = _dateTime.Now;

                    entry.State = EntityState.Modified;
                }
                break;
        }
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