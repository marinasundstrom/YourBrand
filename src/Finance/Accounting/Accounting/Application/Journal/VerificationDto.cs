namespace YourBrand.Accounting.Application.Journal;

public class VerificationDto
{
    public string Id { get; set; } = null!;

    public string Name { get; set; } = null!;

    public string ContentType { get; set; } = null!;

    public string? Description { get; set; }

    public int? InvoiceNo { get; set; }

    public string Url { get; set; } = null!;
}