using YourBrand.Auditability;
using YourBrand.Domain;
using YourBrand.Identity;
using YourBrand.Tenancy;
using YourBrand.Ticketing.Domain.ValueObjects;

namespace YourBrand.Ticketing.Domain.Entities;

public class TicketStatus : Entity<int>, IHasTenant, IHasOrganization, IAuditableEntity<int>
{
    protected TicketStatus()
    {

    }

    public TicketStatus(int id, string name, string? handle, string? description)
    : base(id)
    {
        Name = name;
        Handle = handle;
        Description = description;
    }

    public TenantId TenantId { get; set; }

    public OrganizationId OrganizationId { get; set; }

    public ProjectId ProjectId { get; set; }

    public string Name { get; set; } = null!;
    public string? Handle { get; set; } = null!;
    public string? Description { get; set; } = null!;

    public User? CreatedBy { get; set; }
    public UserId? CreatedById { get; set; }
    public DateTimeOffset Created { get; set; }
    public User? LastModifiedBy { get; set; }
    public UserId? LastModifiedById { get; set; }
    public DateTimeOffset? LastModified { get; set; }
}