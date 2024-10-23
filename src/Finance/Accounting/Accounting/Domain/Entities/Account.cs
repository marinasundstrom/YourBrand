using System.ComponentModel.DataAnnotations;

using YourBrand.Accounting.Domain.Common;
using YourBrand.Accounting.Domain.Enums;
using YourBrand.Domain;
using YourBrand.Tenancy;

namespace YourBrand.Accounting.Domain.Entities;

public class Account : AuditableEntity, IHasTenant, IHasOrganization
{
    readonly List<LedgerEntry> _entries = new List<LedgerEntry>();

    public TenantId TenantId { get; set; }

    public OrganizationId OrganizationId { get; set; }

    [Key]
    public int AccountNo { get; set; }

    public AccountClass Class { get; set; }

    public AccountGroup? Group { get; set; }

    public string Name { get; set; } = null!;

    public string Description { get; set; } = null!;

    public decimal Debit { get; set; }

    public decimal Credit { get; set; }

    public IReadOnlyCollection<LedgerEntry> Entries => _entries;

    internal void AddEntry(LedgerEntry ledgerEntry)
    {
        _entries.Add(ledgerEntry);

        Debit += ledgerEntry.Debit.GetValueOrDefault();
        Credit += ledgerEntry.Credit.GetValueOrDefault();
    }
}