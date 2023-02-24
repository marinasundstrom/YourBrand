using System;

using YourBrand.Documents.Application.Common.Interfaces;
using YourBrand.Documents.Domain;
using YourBrand.Documents.Domain.Common;
using YourBrand.Documents.Domain.Entities;

using Microsoft.EntityFrameworkCore;
using YourBrand.Documents.Infrastructure.Persistence.Interceptors;
using YourBrand.Documents.Infrastructure.Persistence.Outbox;
using Newtonsoft.Json;

namespace YourBrand.Documents.Infrastructure.Persistence;

public class DocumentsContext : DbContext, IDocumentsContext
{
    private readonly AuditableEntitySaveChangesInterceptor _auditableEntitySaveChangesInterceptor;

    public DocumentsContext(
        DbContextOptions<DocumentsContext> options,
        AuditableEntitySaveChangesInterceptor auditableEntitySaveChangesInterceptor) : base(options)
    {
        _auditableEntitySaveChangesInterceptor = auditableEntitySaveChangesInterceptor;
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        base.OnConfiguring(optionsBuilder);

        optionsBuilder.AddInterceptors(_auditableEntitySaveChangesInterceptor);

#if DEBUG
        optionsBuilder.EnableSensitiveDataLogging(); 
#endif
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