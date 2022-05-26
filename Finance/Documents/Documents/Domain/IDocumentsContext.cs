using Documents.Domain.Entities;

using Microsoft.EntityFrameworkCore;

namespace Documents.Domain;

public interface IDocumentsContext
{
    DbSet<Document> Documents { get; set; }
    DbSet<DocumentTemplate> DocumentTemplates { get; set; }
}