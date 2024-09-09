using YourBrand.Domain;
using YourBrand.Invoicing.Domain.Common;
using YourBrand.Invoicing.Domain.Enums;

namespace YourBrand.Invoicing.Domain.Events;

public record InvoiceStatusUpdated : DomainEvent
{
    public InvoiceStatusUpdated(OrganizationId organizationId, string invoiceId, int status, int oldStatus)
    {
        OrganizationId = organizationId;
        InvoiceId = invoiceId;
        Status = status;
        OldStatus = oldStatus;
    }

    public OrganizationId OrganizationId { get; }
    
    public string InvoiceId { get; }

    public int Status { get; }

    public int OldStatus { get; }
}