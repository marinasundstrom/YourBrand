using System.ComponentModel.DataAnnotations;

using YourBrand.Accounting.Domain.Common;
using YourBrand.Accounting.Domain.Enums;
using YourBrand.Tenancy;

namespace YourBrand.Accounting.Domain.Entities;

public class Account : AuditableEntity, IHasTenant
{
    public TenantId TenantId { get; set; }

    [Key]
    public int AccountNo { get; set; }

    public AccountClass Class { get; set; }

    public AccountGroup? Group { get; set; }

    public string Name { get; set; } = null!;

    public string Description { get; set; } = null!;

    public List<LedgerEntry> Entries { get; set; } = new List<LedgerEntry>();
}