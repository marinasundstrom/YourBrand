using Invoices.Application.Common.Interfaces;
using Invoices.Domain;
using Invoices.Domain.Common;
using Invoices.Domain.Entities;

using Microsoft.EntityFrameworkCore;

namespace Invoices.Infrastructure.Persistence;

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

    public DbSet<InvoiceItem> InvoiceItems { get; set; }  = null!;

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