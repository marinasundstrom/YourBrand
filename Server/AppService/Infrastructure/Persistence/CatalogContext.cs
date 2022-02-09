using System;

using Catalog.Application.Common.Interfaces;
using Catalog.Domain.Common;
using Catalog.Domain.Entities;
using Catalog.Infrastructure;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;

namespace Catalog.Infrastructure.Persistence;

class CatalogContext : DbContext, ICatalogContext
{
    private readonly ICurrentUserService _currentUserService;
    private readonly IDomainEventService _domainEventService;
    private readonly IDateTime _dateTime;
    private Transaction? _transaction;

    public CatalogContext(
        DbContextOptions<CatalogContext> options,
        ICurrentUserService currentUserService,
        IDomainEventService domainEventService,
        IDateTime dateTime) : base(options)
    {
        _currentUserService = currentUserService;
        _domainEventService = domainEventService;
        _dateTime = dateTime;
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.ApplyConfigurationsFromAssembly(typeof(Configurations.ItemConfiguration).Assembly);
    }

#nullable disable

    public DbSet<Item> Items { get; set; } = null!;

    public DbSet<Comment> Comments { get; set; } = null!;

#nullable restore

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        foreach (Microsoft.EntityFrameworkCore.ChangeTracking.EntityEntry<AuditableEntity> entry in ChangeTracker.Entries<AuditableEntity>())
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

        if (_transaction is not null)
        {
            return await base.SaveChangesAsync(cancellationToken);
        }

        DomainEvent[] events = GetDomainEvents();

        var result = await base.SaveChangesAsync(cancellationToken);

        await DispatchEvents(events);

        return result;
    }

    private DomainEvent[] GetDomainEvents()
    {
        return ChangeTracker.Entries<IHasDomainEvent>()
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

    public async Task<ITransaction> BeginTransactionAsync()
    {
        var transaction = await Database.BeginTransactionAsync();

        _transaction = new Transaction(
            this,
            transaction);

        return _transaction;
    }

    class Transaction : ITransaction
    {
        private readonly CatalogContext _context;
        private readonly IDbContextTransaction _transaction;

        public Transaction(CatalogContext context, IDbContextTransaction transaction)
        {
            _context = context;
            _transaction = transaction;
        }

        public async Task CommitAsync()
        {
            DomainEvent[] events = _context.GetDomainEvents();

            await _transaction.CommitAsync();

            await _context.DispatchEvents(events);
            _context._transaction = null;
        }

        public void Dispose()
        {
            _transaction.Dispose();
            _context._transaction = null;
        }

        public async ValueTask DisposeAsync()
        {
            await _transaction.DisposeAsync();
            _context._transaction = null;
        }

        public async Task RollbackAsync()
        {
            await _transaction.RollbackAsync();
            _context._transaction = null;
        }
    }
}