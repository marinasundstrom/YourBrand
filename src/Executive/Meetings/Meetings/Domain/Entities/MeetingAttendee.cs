using YourBrand.Identity;
using YourBrand.Tenancy;
using YourBrand.Domain;
using YourBrand.Meetings.Domain.ValueObjects;

namespace YourBrand.Meetings.Domain.Entities;

public enum AttendeeRole
{
    Chairperson,
    Secretary,
    Participant,
    Observer
}

public class MeetingAttendee : Entity<MeetingAttendeeId>, IAuditable, IHasTenant, IHasOrganization
{
    public MeetingAttendee()
        : base(new MeetingAttendeeId())
    {

    }

    public TenantId TenantId { get; set; }
    public OrganizationId OrganizationId { get; set; }

    public MeetingId MeetingId { get; set; }

    public string? Name { get; set; }
    public UserId? UserId { get; set; }

    public int Order { get; set; }

    public MeetingGroupId? MeetingGroupId { get; set; }
    public MeetingGroupMemberId? MeetingGroupMemberId { get; set; }

    public string? Email { get; set; }

    public DateTimeOffset? InvitedAt { get; set; }
    public DateTimeOffset? InviteAcceptedAt { get; set; }

    public bool IsPresent { get; set; }
    public bool HasSpeakingRights { get; set; }
    public bool HasVotingRights { get; set; }
    public AttendeeRole Role { get; set; }

    public User? CreatedBy { get; set; } = null!;
    public UserId? CreatedById { get; set; } = null!;
    public DateTimeOffset Created { get; set; }
    public User? LastModifiedBy { get; set; }
    public UserId? LastModifiedById { get; set; }
    public DateTimeOffset? LastModified { get; set; }
}