using System;

using YourBrand.Documents.Application.Common.Interfaces;
using YourBrand.Documents.Domain;
using YourBrand.Documents.Domain.Common;
using YourBrand.Documents.Domain.Entities;

using Microsoft.EntityFrameworkCore;

namespace YourBrand.Documents.Infrastructure.Persistence;

public class DocumentsContext : DbContext, IDocumentsContext
{
    private IDomainEventService _domainEventService;
    private ICurrentUserService _currentUserService;
    private IDateTime _dateTime;

    public DocumentsContext(
        DbContextOptions<DocumentsContext> options,
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

        modelBuilder.ApplyConfigurationsFromAssembly(typeof(DocumentsContext).Assembly);
    }

    public DbSet<Domain.Entities.Directory> Directories { get; set; } = null!;

    public DbSet<Document> Documents { get; set; } = null!;

    public DbSet<DocumentType> DocumentTypes { get; set; } = null!;

    public DbSet<DocumentTemplate> DocumentTemplates { get; set; } = null!;

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
            else if (entry.State == EntityState.Deleted)
            {
                if (entry.Entity is ISoftDelete e)
                {
                    e.Deleted = _dateTime.Now;
                    e.DeletedById = _currentUserService.UserId;

                    entry.State = EntityState.Modified;
                }

                if(entry.Entity is IDeletable e2)
                {
                    entry.Entity.AddDomainEvent(e2.GetDeleteEvent());
                }
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