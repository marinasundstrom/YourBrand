using YourBrand.Invoices.Application.Common.Interfaces;
using YourBrand.Invoices.Domain;
using YourBrand.Invoices.Domain.Common;
using YourBrand.Invoices.Domain.Entities;

using Microsoft.EntityFrameworkCore;

namespace YourBrand.Invoices.Infrastructure.Persistence;

public class InvoicesContext : DbContext, IInvoicesContext
{
    private IDomainEventService _domainEventService;
    private ICurrentUserService _currentUserService;
    private IDateTime _dateTime;

    public InvoicesContext(
        DbContextOptions<InvoicesContext> options,
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

        modelBuilder.ApplyConfigurationsFromAssembly(typeof(InvoicesContext).Assembly);
    }

    public DbSet<Invoice> Invoices { get; set; } = null!;

    public DbSet<InvoiceItem> InvoiceItems { get; set; } = null!;

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