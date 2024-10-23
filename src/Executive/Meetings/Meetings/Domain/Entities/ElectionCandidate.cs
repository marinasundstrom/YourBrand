using YourBrand.Domain;
using YourBrand.Identity;
using YourBrand.Meetings.Domain.ValueObjects;
using YourBrand.Tenancy;

namespace YourBrand.Meetings.Domain.Entities;

public sealed class ElectionCandidate : Entity<ElectionCandidateId>, IAuditable, IHasTenant, IHasOrganization
{
    public ElectionCandidate()
        : base(new ElectionCandidateId())
    {

    }

    public ElectionCandidate(MeetingAttendeeId id, string statement)
        : base(new ElectionCandidateId())
    {
    }

    public TenantId TenantId { get; set; }
    public OrganizationId OrganizationId { get; set; }
    public AgendaItemId AgendaItemId { get; set; }

    public MeetingAttendeeId NomineeId { get; set; }
    public string Statement { get; set; }

    public User? CreatedBy { get; set; } = null!;
    public UserId? CreatedById { get; set; } = null!;
    public DateTimeOffset Created { get; set; }
    public User? LastModifiedBy { get; set; }
    public UserId? LastModifiedById { get; set; }
    public DateTimeOffset? LastModified { get; set; }
}