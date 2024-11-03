using YourBrand.Auditability;
using YourBrand.Domain;
using YourBrand.Identity;
using YourBrand.Tenancy;
using YourBrand.Ticketing.Domain.ValueObjects;

namespace YourBrand.Ticketing.Domain.Entities;

public class TicketTag : AggregateRoot<string>, IAuditableEntity<string>
{
    public TicketTag()
        : base(Guid.NewGuid().ToString())
    {

    }

    public TicketTag(string id)
        : base(id)
    {

    }

    public TenantId TenantId { get; set; }

    public OrganizationId OrganizationId { get; set; }

    public Organization Organization { get; set; }

    public TicketId TicketId { get; set; }

    public Ticket Ticket { get; set; }

    public int TagId { get; set; }

    public Tag Tag { get; set; }

    public User? CreatedBy { get; set; }

    public UserId? CreatedById { get; set; }

    public DateTimeOffset Created { get; set; }

    public User? LastModifiedBy { get; set; }

    public UserId? LastModifiedById { get; set; }

    public DateTimeOffset? LastModified { get; set; }
}