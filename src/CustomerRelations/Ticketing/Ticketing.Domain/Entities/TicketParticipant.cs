using YourBrand.Auditability;
using YourBrand.Domain;
using YourBrand.Identity;
using YourBrand.Tenancy;
using YourBrand.Ticketing.Domain.ValueObjects;

namespace YourBrand.Ticketing.Domain.Entities;

public class TicketParticipant : Entity<TicketParticipantId>, IAuditableEntity<TicketParticipantId>, IHasTenant, IHasOrganization
{
    public TicketParticipant()
        : base(new TicketParticipantId())
    {

    }

    public TenantId TenantId { get; set; }

    public OrganizationId OrganizationId { get; set; }

    public TicketId TicketId { get; set; }

    public string? Name { get; set; }

    public UserId? UserId { get; set; }

    public string? Email { get; set; }

    public User? CreatedBy { get; set; } = null!;

    public UserId? CreatedById { get; set; } = null!;

    public DateTimeOffset Created { get; set; }

    public User? LastModifiedBy { get; set; }

    public UserId? LastModifiedById { get; set; }

    public DateTimeOffset? LastModified { get; set; }
}