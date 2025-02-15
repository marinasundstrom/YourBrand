using YourBrand.Auditability;
using YourBrand.Domain;
using YourBrand.Identity;
using YourBrand.Meetings.Domain.ValueObjects;
using YourBrand.Tenancy;

namespace YourBrand.Meetings.Domain.Entities;

public enum MotionStatus
{
    Proposal,
    Adopted,// Approved
    Rejected,
    Amended
}

public class Motion : AggregateRoot<MotionId>, IAuditableEntity<MotionId>, IHasTenant, IHasOrganization
{
    readonly HashSet<MotionOperativeClause> _operativeClauses = new HashSet<MotionOperativeClause>();

    protected Motion()
    {
    }

    public Motion(MotionId id, string title) : base(id)
    {
        Title = title;
    }

    public TenantId TenantId { get; set; }
    public OrganizationId OrganizationId { get; set; }

    public string Title { get; set; }
    public string? Text { get; set; }
    public MotionStatus Status { get; set; } = MotionStatus.Proposal;

    public IReadOnlyCollection<MotionOperativeClause> OperativeClauses => _operativeClauses;

    public MotionOperativeClause AddOperativeClause(OperativeAction action, string text)
    {
        int order = 1;

        try
        {
            var last = _operativeClauses.OrderByDescending(x => x.Order).First();
            order = last.Order + 1;
        }
        catch { }

        var clause = new MotionOperativeClause(action, text);
        clause.Order = order;
        _operativeClauses.Add(clause);
        return clause;
    }

    public bool RemoveOperativeClause(MotionOperativeClause item)
    {
        return _operativeClauses.Remove(item);
    }

    public User? CreatedBy { get; set; } = null!;
    public UserId? CreatedById { get; set; } = null!;
    public DateTimeOffset Created { get; set; }
    public User? LastModifiedBy { get; set; }
    public UserId? LastModifiedById { get; set; }
    public DateTimeOffset? LastModified { get; set; }
}