namespace Accounting.Application.Verifications;

public class VerificationDto
{
    public int Id { get; set; }

    public DateTime Date { get; set; }

    public string Description { get; set; } = null!;

    public decimal Debit { get; set; }

    public decimal Credit { get; set; }

    public int? InvoiceId { get; set; }

    public IEnumerable<AttachmentDto> Attachments { get; set; } = null!;
}