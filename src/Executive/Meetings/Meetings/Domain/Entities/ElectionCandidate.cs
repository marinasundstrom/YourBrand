using YourBrand.Auditability;
using YourBrand.Domain;
using YourBrand.Identity;
using YourBrand.Meetings.Domain.ValueObjects;
using YourBrand.Tenancy;

namespace YourBrand.Meetings.Domain.Entities;

public sealed class ElectionCandidate : Entity<ElectionCandidateId>, IAuditableEntity<ElectionCandidateId>, IHasTenant, IHasOrganization
{
    private ElectionCandidate()
        : base(new ElectionCandidateId())
    {
    }

    public ElectionCandidate(string name, string? statement,
        UserId? userId = null,
        MeetingGroupMemberId? groupMemberId = null,
        MeetingAttendeeId? attendeeId = null)
        : base(new ElectionCandidateId())
    {
        UserId = userId;
        GroupMemberId = groupMemberId;
        AttendeeId = attendeeId;
        Name = name;
        Statement = statement;
    }

    public TenantId TenantId { get; set; }
    public OrganizationId OrganizationId { get; set; }
    public ElectionId? ElectionId { get; set; }

    public string Name { get; set; }
    public string? Description { get; set; }

    public UserId? UserId { get; set; }
    public MeetingGroupMemberId? GroupMemberId { get; set; }
    public MeetingAttendeeId? AttendeeId { get; set; }

    public IdGroup? NominatedBy { get; set; }
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

public class IdGroup 
{
    public string? Name { get; set; }
    public UserId? UserId { get; set; }
    public MeetingGroupMemberId? GroupMemberId { get; set; }
    public MeetingAttendeeId? AttendeeId { get; set; }
}