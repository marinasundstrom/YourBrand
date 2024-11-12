using YourBrand.Auditability;
using YourBrand.Domain;
using YourBrand.Identity;
using YourBrand.Meetings.Domain.ValueObjects;
using YourBrand.Tenancy;

namespace YourBrand.Meetings.Domain.Entities;

public enum VoteOption
{
    For,
    Against,
    Abstain
}

public sealed class Vote : Entity<VoteId>, IAuditableEntity<VoteId>, IHasTenant, IHasOrganization
{
    public Vote()
        : base(new VoteId())
    {

    }

    public TenantId TenantId { get; set; }
    public OrganizationId OrganizationId { get; set; }
    public VotingId VotingId { get; set; }

    public MeetingAttendeeId VoterId { get; set; }
    public DateTimeOffset TimeCast { get; set; }

    public VoteOption? Option { get; set; }

    public User? CreatedBy { get; set; } = null!;
    public UserId? CreatedById { get; set; } = null!;
    public DateTimeOffset Created { get; set; }
    public User? LastModifiedBy { get; set; }
    public UserId? LastModifiedById { get; set; }
    public DateTimeOffset? LastModified { get; set; }
}
