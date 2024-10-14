using YourBrand.Identity;
using YourBrand.Tenancy;
using YourBrand.Domain;
using YourBrand.Meetings.Domain.ValueObjects;

namespace YourBrand.Meetings.Domain.Entities;

public enum MinutesItemType
{
    Default,
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

    public MinutesItem(MinutesItemType type, string title, string description)
    : base(new MinutesItemId())
    {
        Type = type;
        Title = title;
        Description = description;
    }

    public TenantId TenantId { get; set; }
    public OrganizationId OrganizationId { get; set; }
    public MinutesId MinutesId { get; set; }

    public AgendaId? AgendaId { get; set; }
    public AgendaItemId? AgendaItemId { get; set; }

    public MinutesItemType Type { get; set; }

    public string Title { get; set; }
    public string Description { get; set; }
    public MinutesItemState State { get; set; } = MinutesItemState.Created;
    public int Order { get; set; }

    // For motions
    public MotionId? MotionId { get; set; }

    public User? CreatedBy { get; set; } = null!;
    public UserId? CreatedById { get; set; } = null!;
    public DateTimeOffset Created { get; set; }
    public User? LastModifiedBy { get; set; }
    public UserId? LastModifiedById { get; set; }
    public DateTimeOffset? LastModified { get; set; }
}