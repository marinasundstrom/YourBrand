using YourBrand.Identity;
using YourBrand.Tenancy;
using YourBrand.Domain;
using YourBrand.Meetings.Domain.ValueObjects;

namespace YourBrand.Meetings.Domain.Entities;

public sealed class ElectionCandidate : Entity<ElectionCandidateId>, IAuditable, IHasTenant, IHasOrganization
{
    public ElectionCandidate()
        : base(new ElectionCandidateId())
    {

    }

    public ElectionCandidate(MeetingParticipantId id, string statement)
        : base(new ElectionCandidateId())
    {
    }

    public TenantId TenantId { get; set; }
    public OrganizationId OrganizationId { get; set; }
    public AgendaItemId AgendaItemId { get; set; }

    public MeetingParticipantId NomineeId { get; set; }
    public string Statement { get; set; }

    public User? CreatedBy { get; set; } = null!;
    public UserId? CreatedById { get; set; } = null!;
    public DateTimeOffset Created { get; set; }
    public User? LastModifiedBy { get; set; }
    public UserId? LastModifiedById { get; set; }
    public DateTimeOffset? LastModified { get; set; }
}

