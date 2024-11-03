using System.ComponentModel.DataAnnotations;

using YourBrand.Accounting.Domain.Common;
using YourBrand.Accounting.Domain.Enums;
using YourBrand.Domain;
using YourBrand.Tenancy;

namespace YourBrand.Accounting.Domain.Entities;

public class Verification : AuditableEntity<string>, IHasTenant, IHasOrganization
{
    protected Verification()
    {
    }

    public Verification(string id) : base(id)
    {

    }

    public TenantId TenantId { get; set; }

    public OrganizationId OrganizationId { get; set; }

    public string Name { get; set; } = null!;

    public string ContentType { get; set; } = null!;

    public AttachmentType Type { get; set; }

    public int JournalEntryId { get; set; }

    public JournalEntry JournalEntry { get; set; } = null!;

    public DateTimeOffset Date { get; set; }

    public string? Description { get; set; }

    public int? InvoiceNo { get; set; }

    public int? ReceiptNo { get; set; }
}