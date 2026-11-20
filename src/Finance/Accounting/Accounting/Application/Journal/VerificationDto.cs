using YourBrand.Accounting.Application.Ledger;

namespace YourBrand.Accounting.Application.Journal;

public class VerificationDto
{
    public int Id { get; set; }

    public DateTimeOffset Date { get; set; }

    public string Description { get; set; } = null!;

    public decimal Debit { get; set; }

    public decimal Credit { get; set; }

    public int? InvoiceNo { get; set; }

    public IEnumerable<LedgerEntryDto> Entries { get; set; } = null!;

    public IEnumerable<SupportingDocumentDto> SupportingDocuments { get; set; } = null!;
}