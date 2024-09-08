using System.ComponentModel.DataAnnotations;

using YourBrand.Accounting.Domain.Common;
using YourBrand.Domain;
using YourBrand.Tenancy;

namespace YourBrand.Accounting.Domain.Entities;

public class JournalEntry : AuditableEntity, IHasTenant, IHasOrganization
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

    public OrganizationId OrganizationId { get; set; }

    public DateTime Date { get; private set; }

    public string Description { get; private set; } = null!;

    public int? InvoiceNo { get; private set; }

    public int? ReceiptId { get; private set; }

    public IReadOnlyCollection<LedgerEntry> Entries => _entries;

    public void AddEntries(IEnumerable<LedgerEntry> entries) => entries.ToList().ForEach(entry =>
    {
        entry.OrganizationId = OrganizationId;
        entry.Account.AddEntry(entry);
        if(entry.Debit > 0)
        {
            Debit += entry.Debit.GetValueOrDefault();
        }
        else
        {
            Credit += entry.Credit.GetValueOrDefault();
        }
        _entries.Add(entry);
    });

    public LedgerEntry AddDebitEntry(Account account, decimal debit, string? description = null)
    {
        var entry = new LedgerEntry(Date, account, debit, null, description);
        entry.OrganizationId = OrganizationId;
        account.AddEntry(entry);
        _entries.Add(entry);
        Debit += debit;
        return entry;
    }

    public LedgerEntry AddCreditEntry(Account account, decimal credit, string? description = null)
    {
        var entry = new LedgerEntry(Date, account, null, credit, description);
        entry.OrganizationId = OrganizationId;
        account.AddEntry(entry);
        _entries.Add(entry);
        Credit += credit;
        return entry;
    }

    public IReadOnlyCollection<Verification> Verifications => _verifications;

    public void AddVerification(Verification verification)
    {
        verification.OrganizationId = OrganizationId;
        _verifications.Add(verification);
    }

    public decimal Debit { get; set; }

    public decimal Credit { get; set; }

    public decimal Sum => _entries.Sum(x => x.Debit.GetValueOrDefault() - x.Credit.GetValueOrDefault());

    public bool IsValid => Sum == 0;

}