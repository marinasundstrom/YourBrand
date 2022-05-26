namespace Accounting.Application.Verifications;

public class AttachmentDto
{
    public string Id { get; set; } = null!;

    public string Name { get; set; } = null!;

    public string ContentType { get; set; } = null!;

    public string? Description { get; set; }

    public int? InvoiceId { get; set; }

    public string Url { get; set; } = null!;
}