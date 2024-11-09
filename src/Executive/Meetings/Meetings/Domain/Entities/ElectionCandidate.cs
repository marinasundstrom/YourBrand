using YourBrand.Auditability;
using YourBrand.Domain;
using YourBrand.Identity;
using YourBrand.Meetings.Domain.ValueObjects;
using YourBrand.Tenancy;

namespace YourBrand.Meetings.Domain.Entities;

public sealed class ElectionCandidate : Entity<ElectionCandidateId>, IAuditableEntity<ElectionCandidateId>, IHasTenant, IHasOrganization
{
    public ElectionCandidate()
        : base(new ElectionCandidateId())
    {
    }

    public ElectionCandidate(MeetingAttendeeId id, string name, string? statement, string? position = null)
        : base(new ElectionCandidateId())
    {
        AttendeeId = id;
        Name = name;
        Statement = statement;
        Position = position;
    }

    public TenantId TenantId { get; set; }
    public OrganizationId OrganizationId { get; set; }
    public AgendaItemId? AgendaItemId { get; set; }
    public ElectionSessionId? ElectionSessionId { get; set; }

    public string Name { get; set; }
    public string? Position { get; set; }
    public string? Description { get; set; }
    public MeetingAttendeeId AttendeeId { get; set; }

    public MeetingAttendeeId? NominatedBy { get; set; }
    public DateTimeOffset? NominatedAt { get; set; }
    public string? Statement { get; set; }

    public DateTimeOffset? WithdrawnAt { get; set; }

    public bool IsPreMeetingNomination { get; set; } // Tracks if the nomination is pre-meeting

    public User? CreatedBy { get; set; } = null!;
    public UserId? CreatedById { get; set; } = null!;
    public DateTimeOffset Created { get; set; }
    public User? LastModifiedBy { get; set; }
    public UserId? LastModifiedById { get; set; }
    public DateTimeOffset? LastModified { get; set; }
}
