using YourBrand.Identity;
using YourBrand.Tenancy;
using YourBrand.Domain;
using YourBrand.Meetings.Domain.ValueObjects;

namespace YourBrand.Meetings.Domain.Entities;

public enum VoteOption
{
    For,
    Against,
    Abstain
}

public class Vote : AggregateRoot<VoteId>, IAuditable, IHasTenant, IHasOrganization
{
    public TenantId TenantId { get; set; }
    public OrganizationId OrganizationId { get; set; }


    public MeetingParticipantId VoterId { get; set; }
    public VoteOption Option { get; set; }
    public DateTimeOffset VoteTime { get; set; }
    public MotionId MotionId { get; set; }

    public User? CreatedBy { get; set; } = null!;
    public UserId? CreatedById { get; set; } = null!;
    public DateTimeOffset Created { get; set; }
    public User? LastModifiedBy { get; set; }
    public UserId? LastModifiedById { get; set; }
    public DateTimeOffset? LastModified { get; set; }
}