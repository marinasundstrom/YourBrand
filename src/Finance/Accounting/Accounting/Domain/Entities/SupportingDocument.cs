using System.ComponentModel.DataAnnotations;

using YourBrand.Accounting.Domain.Common;
using YourBrand.Accounting.Domain.Enums;
using YourBrand.Domain;
using YourBrand.Tenancy;

namespace YourBrand.Accounting.Domain.Entities;

public class SupportingDocument : AuditableEntity<string>, IHasTenant, IHasOrganization
{
    protected SupportingDocument()
    {
    }

    public SupportingDocument(string id) : base(id)
    {

    }

    public TenantId TenantId { get; set; }

    public OrganizationId OrganizationId { get; set; }

    public string Name { get; set; } = null!;

    public string ContentType { get; set; } = null!;

    public AttachmentType Type { get; set; }

    public int VerificationId { get; set; }

    public Verification Verification { get; set; } = null!;

    public DateTimeOffset Date { get; set; }

    public string? Description { get; set; }

    public int? InvoiceNo { get; set; }

    public int? ReceiptNo { get; set; }
}