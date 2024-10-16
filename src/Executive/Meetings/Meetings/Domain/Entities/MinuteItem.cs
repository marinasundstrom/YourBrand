using YourBrand.Identity;
using YourBrand.Tenancy;
using YourBrand.Domain;
using YourBrand.Meetings.Domain.ValueObjects;

namespace YourBrand.Meetings.Domain.Entities;

public enum MinutesItemType
{
    General,
}

public enum MinutesItemState
{
    Created,
    Reviewing,
    Approved
}

public class MinutesItem : Entity<MinutesItemId>, IAuditable, IHasTenant, IHasOrganization
{
    readonly HashSet<ElectionCandidate> _candidates = new HashSet<ElectionCandidate>();

    public MinutesItem(MinutesItemType type, string heading, string details)
    : base(new MinutesItemId())
    {
        Type = type;
        Heading = heading;
        Details = details;
    }

    public TenantId TenantId { get; set; }
    public OrganizationId OrganizationId { get; set; }
    public MinutesId MinutesId { get; set; }

    public AgendaId? AgendaId { get; set; }
    public AgendaItemId? AgendaItemId { get; set; }

    public MinutesItemType Type { get; set; }

    public string Heading { get; set; }
    public string Details { get; set; }
    public MinutesItemState State { get; set; } = MinutesItemState.Created;
    public int Order  { get; set; }

    // For motions
    public MotionId? MotionId { get; set; }

    public User? CreatedBy { get; set; } = null!;
    public UserId? CreatedById { get; set; } = null!;
    public DateTimeOffset Created { get; set; }
    public User? LastModifiedBy { get; set; }
    public UserId? LastModifiedById { get; set; }
    public DateTimeOffset? LastModified { get; set; }
}