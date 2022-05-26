using Documents.Domain.Enums;

namespace Documents.Domain.Entities;

public class DocumentTemplate
{
    public string Id { get; set; } = null!;

    public string Name { get; set; } = null!;

    public DocumentTemplateLanguage? TemplateLanguage { get; set; }

    public string Content { get; set; } = null!;
}