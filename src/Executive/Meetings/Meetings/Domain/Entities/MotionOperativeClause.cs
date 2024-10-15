using YourBrand.Identity;
using YourBrand.Tenancy;
using YourBrand.Domain;
using YourBrand.Meetings.Domain.ValueObjects;

namespace YourBrand.Meetings.Domain.Entities;

public enum OperativeAction
{
    Decides,
    Requests,
    Recommends,
    Instructs,
    Urges,
    Encourages,
    Authorizes,
    CallsFor,
    Acknowledges,
    Endorses,
    Adopts,
    Amends,
    Rejects,
    Elects
}

public class MotionOperativeClause : Entity<MotionOperativeClauseId>, IAuditable, IHasTenant, IHasOrganization
{
    public MotionOperativeClause(OperativeAction action, string text)
    : base(new MotionOperativeClauseId())
    {
        Action = action;
        Text = text;
    }

    public TenantId TenantId { get; set; }
    public OrganizationId OrganizationId { get; set; }
    public MotionId MotionId { get; set; }

    public OperativeAction Action { get; set; }

    public string Text { get; set; }
    public int Order { get; set; }

    public User? CreatedBy { get; set; } = null!;
    public UserId? CreatedById { get; set; } = null!;
    public DateTimeOffset Created { get; set; }
    public User? LastModifiedBy { get; set; }
    public UserId? LastModifiedById { get; set; }
    public DateTimeOffset? LastModified { get; set; }
}