using YourBrand.Identity;
using YourBrand.Tenancy;
using YourBrand.Domain;

namespace YourBrand.Ticketing.Domain.Entities;

public class TicketComment : Entity<int>, IAuditable, IHasTenant, IHasOrganization
{
    public TenantId TenantId { get; set; }

    public OrganizationId OrganizationId { get; set; }

    public int TicketId { get; set; }

    public string Text { get; set; } = null!;

    public HashSet<Attachment> Attachments { get; } = new HashSet<Attachment>();

    // ...

    public User? CreatedBy { get; set; } = null!;

    public UserId? CreatedById { get; set; } = null!;

    public DateTimeOffset Created { get; set; }

    public User? LastModifiedBy { get; set; }

    public UserId? LastModifiedById { get; set; }

    public DateTimeOffset? LastModified { get; set; }
}