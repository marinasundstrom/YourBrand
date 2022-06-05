using YourBrand.Documents.Domain.Entities;

using Microsoft.EntityFrameworkCore;

namespace YourBrand.Documents.Domain;

public interface IDocumentsContext
{
    DbSet<Document> Documents { get; set; }
    DbSet<DocumentTemplate> DocumentTemplates { get; set; }
}