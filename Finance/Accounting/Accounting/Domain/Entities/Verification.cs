using System.ComponentModel.DataAnnotations;

namespace YourBrand.Accounting.Domain.Entities;

public class Verification
{
    private readonly List<Entry> _entries = new();
    private readonly List<Attachment> _attachments = new List<Attachment>();

    protected Verification()
    {

    }

    public Verification(DateTime date, string description, int? invoiceId = null, int? receiptId = null)
    {
        Date = date;
        Description = description;
        InvoiceId = invoiceId;
        ReceiptId = receiptId;
    }

    [Key]
    public int Id { get; private set; }

    public DateTime Date { get; private set; }

    public string Description { get; private set; } = null!;

    public int? InvoiceId { get; private set; }

    public int? ReceiptId { get; private set; }

    public IReadOnlyCollection<Entry> Entries => _entries.AsReadOnly();

    public Entry AddDebitEntry(Account account, decimal debit, string? description = null)
    {
        var entry = new Entry(Date, account, debit, null, description);
        _entries.Add(entry);
        return entry;
    }

    public Entry AddCreditEntry(Account account, decimal credit, string? description = null)
    {
        var entry = new Entry(Date, account, null, credit, description);
        _entries.Add(entry);
        return entry;
    }

    public IReadOnlyCollection<Attachment> Attachments => _attachments.AsReadOnly();

    public void AddAttachment(Attachment attachment)
    {
        _attachments.Add(attachment);
    }

    public decimal Sum => _entries.Sum(x => x.Debit.GetValueOrDefault() - x.Credit.GetValueOrDefault());

    public bool IsValid => Sum == 0;

}