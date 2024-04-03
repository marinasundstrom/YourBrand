using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

using YourBrand.Accounting.Domain.Common;
using YourBrand.Tenancy;

namespace YourBrand.Accounting.Domain.Entities;

public class LedgerEntry : AuditableEntity, IHasTenant
{
    public LedgerEntry()
    {

    }

    public LedgerEntry(DateTime date, Account account, decimal? debit, decimal? credit, string? description)
    {
        Date = date;
        Account = account;
        Debit = debit;
        Credit = credit;
        Description = description;
    }

    [Key]
    public int Id { get; set; }

    public TenantId TenantId { get; set; }

    public DateTime Date { get; set; } = DateTime.Now;

    public int JournalEntryId { get; set; }

    [ForeignKey(nameof(JournalEntryId))]
    public JournalEntry JournalEntry { get; set; } = null!;

    public int AccountNo { get; set; }

    [ForeignKey(nameof(AccountNo))]
    public Account Account { get; set; } = null!;

    public string? Description { get; set; } = null!;

    public decimal? Debit { get; set; }

    public decimal? Credit { get; set; }
}