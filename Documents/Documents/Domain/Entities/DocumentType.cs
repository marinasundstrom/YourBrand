using YourBrand.Documents.Domain.Common;

namespace YourBrand.Documents.Domain.Entities;

public class DocumentType : AuditableEntity
{
    public string Id { get; set; } = null!;

    public string Name { get; set; } = null!;
}