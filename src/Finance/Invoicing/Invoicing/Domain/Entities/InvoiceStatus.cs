using YourBrand.Domain;
using YourBrand.Invoicing.Domain.Common;
using YourBrand.Tenancy;

namespace YourBrand.Invoicing.Domain.Entities;

public sealed class InvoiceStatus : AuditableEntity, IHasTenant, IHasOrganization
{
    protected InvoiceStatus()
    {
    }

    public InvoiceStatus(string name, string handle, string? description)
    {
        Name = name;
        Handle = handle;
        Description = description;
    }

    public InvoiceStatus(int id, string name, string handle, string? description)
    {
        Id = id;
        Name = name;
        Handle = handle;
        Description = description;
    }

    public int Id { get; set; }

    public TenantId TenantId { get; set; }

    public OrganizationId OrganizationId { get; set; }

    public string Name { get; set; } = null!;

    public string Handle { get; set; } = null!;

    public string? Description { get; set; }

    public int? Status { get; set; } = null!;
}