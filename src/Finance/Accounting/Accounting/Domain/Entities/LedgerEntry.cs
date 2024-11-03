using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

using YourBrand.Accounting.Domain.Common;
using YourBrand.Domain;
using YourBrand.Tenancy;

namespace YourBrand.Accounting.Domain.Entities;

public class LedgerEntry : AuditableEntity<int>, IHasTenant, IHasOrganization
{
    public LedgerEntry()
    {

    }

    public LedgerEntry(int id, DateTimeOffset date, Account account, decimal? debit, decimal? credit, string? description)
        : base(id)
    {
        Date = date;
        Account = account;
        Debit = debit;
        Credit = credit;
        Description = description;
    }

    public TenantId TenantId { get; set; }

    public OrganizationId OrganizationId { get; set; }

    public DateTimeOffset Date { get; set; } = DateTimeOffset.UtcNow;

    public int? JournalEntryId { get; set; }

    public JournalEntry? JournalEntry { get; set; }

    public int AccountNo { get; set; }

    public Account Account { get; set; } = null!;

    public string? Description { get; set; } = null!;

    public decimal? Debit { get; set; }

    public decimal? Credit { get; set; }
}