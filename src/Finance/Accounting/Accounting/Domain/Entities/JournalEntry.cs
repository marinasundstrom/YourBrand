using System.ComponentModel.DataAnnotations;

using YourBrand.Accounting.Domain.Common;
using YourBrand.Tenancy;

namespace YourBrand.Accounting.Domain.Entities;

public class JournalEntry : AuditableEntity, IHasTenant
{
    private readonly HashSet<LedgerEntry> _entries = new();
    private readonly HashSet<Verification> _verifications = new HashSet<Verification>();

    protected JournalEntry()
    {

    }

    public JournalEntry(DateTime date, string description, int? invoiceNo = null, int? receiptId = null)
    {
        Date = date;
        Description = description;
        InvoiceNo = invoiceNo;
        ReceiptId = receiptId;
    }

    [Key]
    public int Id { get; private set; }

    public TenantId TenantId { get; set; }

    public DateTime Date { get; private set; }

    public string Description { get; private set; } = null!;

    public int? InvoiceNo { get; private set; }

    public int? ReceiptId { get; private set; }

    public IReadOnlyCollection<LedgerEntry> Entries => _entries;

    public void AddEntries(IEnumerable<LedgerEntry> entries) => entries.ToList().ForEach(x => _entries.Add(x));

    public LedgerEntry AddDebitEntry(Account account, decimal debit, string? description = null)
    {
        var entry = new LedgerEntry(Date, account, debit, null, description);
        _entries.Add(entry);
        return entry;
    }

    public LedgerEntry AddCreditEntry(Account account, decimal credit, string? description = null)
    {
        var entry = new LedgerEntry(Date, account, null, credit, description);
        _entries.Add(entry);
        return entry;
    }

    public IReadOnlyCollection<Verification> Verifications => _verifications;

    public void AddVerification(Verification verification)
    {
        _verifications.Add(verification);
    }

    public decimal Sum => _entries.Sum(x => x.Debit.GetValueOrDefault() - x.Credit.GetValueOrDefault());

    public bool IsValid => Sum == 0;

}