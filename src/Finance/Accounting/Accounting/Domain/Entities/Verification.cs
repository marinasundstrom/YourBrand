using System.ComponentModel.DataAnnotations;

using YourBrand.Accounting.Domain.Common;
using YourBrand.Accounting.Domain.Enums;
using YourBrand.Domain;
using YourBrand.Tenancy;

namespace YourBrand.Accounting.Domain.Entities;

public class Verification : AuditableEntity, IHasTenant, IHasOrganization
{
    [Key]
    public string Id { get; set; } = null!;

    public TenantId TenantId { get; set; }

    public OrganizationId OrganizationId { get; set; }

    public string Name { get; set; } = null!;

    public string ContentType { get; set; } = null!;

    public AttachmentType Type { get; set; }

    public JournalEntry JournalEntry { get; set; } = null!;

    public DateTime Date { get; set; }

    public string? Description { get; set; }

    public int? InvoiceNo { get; set; }

    public int? ReceiptNo { get; set; }
}