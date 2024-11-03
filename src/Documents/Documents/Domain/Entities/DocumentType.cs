using YourBrand.Documents.Domain.Common;

namespace YourBrand.Documents.Domain.Entities;

public class DocumentType : AuditableEntity<string>
{
    public string Name { get; set; } = null!;
}