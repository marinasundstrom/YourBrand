using YourBrand.Auditability;
using YourBrand.Domain;
using YourBrand.Identity;
using YourBrand.Meetings.Domain.ValueObjects;
using YourBrand.Tenancy;

namespace YourBrand.Meetings.Domain.Entities;

public sealed class Ballot : Entity<BallotId>, IAuditableEntity<BallotId>, IHasTenant, IHasOrganization
{
    public Ballot()
        : base(new BallotId())
    {

    }

    public TenantId TenantId { get; set; }
    public OrganizationId OrganizationId { get; set; }
    public ElectionId ElectionId { get; set; }

    public MeetingAttendeeId VoterId { get; set; }
    public DateTimeOffset TimeCast { get; set; }

    // For elections
    public ElectionCandidate? SelectedCandidate { get; set; }
    public ElectionCandidateId? SelectedCandidateId { get; set; }

    public User? CreatedBy { get; set; } = null!;
    public UserId? CreatedById { get; set; } = null!;
    public DateTimeOffset Created { get; set; }
    public User? LastModifiedBy { get; set; }
    public UserId? LastModifiedById { get; set; }
    public DateTimeOffset? LastModified { get; set; }
}