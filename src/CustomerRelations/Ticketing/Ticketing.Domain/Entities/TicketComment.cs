using YourBrand.Identity;
using YourBrand.Tenancy;
using YourBrand.Domain;
using YourBrand.Ticketing.Domain.ValueObjects;

namespace YourBrand.Ticketing.Domain.Entities;

public class TicketComment : Entity<int>, IHasTenant, IHasOrganization
{
    public TicketComment()
    {
    }

    public TicketComment(int id) : base(id)
    {
    }

    public TenantId TenantId { get; set; }

    public OrganizationId OrganizationId { get; set; }

    public TicketId TicketId { get; set; }

    public string Text { get; set; } = null!;

    public HashSet<Attachment> Attachments { get; } = new HashSet<Attachment>();

    // ...

    public TicketParticipantId? CreatedById { get; set; } = null!;

    public DateTimeOffset Created { get; set; }

    public TicketParticipantId? LastModifiedById { get; set; }

    public DateTimeOffset? LastModified { get; set; }
}