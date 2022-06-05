using YourBrand.Documents.Domain.Entities;

using Microsoft.EntityFrameworkCore;

namespace YourBrand.Documents.Domain;

public interface IDocumentsContext
{
    DbSet<Document> Documents { get; }

    DbSet<DocumentType> DocumentTypes { get; }

    DbSet<DocumentTemplate> DocumentTemplates { get; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}