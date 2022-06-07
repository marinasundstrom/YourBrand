namespace YourBrand.Documents.Domain.Entities;

public class Event
{
    public string Id { get; set; }

    public DateTime Date { get; set; }

    public Document? Document { get; set; }

    public string? DocumentId { get; set; }

    public Directory? Directory { get; set; }

    public string? DirectoryId { get; set; }

    public Directory? FromDirectory { get; set; }

    public string? FromDirectoryId { get; set; }

    public Directory? ToDirectory { get; set; }

    public string? ToDirectoryId { get; set; }

    public string? OldName { get; set; }

    public string? NewName { get; set; }
}