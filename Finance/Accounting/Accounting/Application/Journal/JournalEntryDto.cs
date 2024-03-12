namespace YourBrand.Accounting.Application.Journal;

public class JournalEntryDto
{
    public int Id { get; set; }

    public DateTime Date { get; set; }

    public string Description { get; set; } = null!;

    public decimal Debit { get; set; }

    public decimal Credit { get; set; }

    public int? InvoiceNo { get; set; }

    public IEnumerable<VerificationDto> Verifications { get; set; } = null!;
}