using YourBrand.Auditability;
using YourBrand.Domain;
using YourBrand.Identity;
using YourBrand.Meetings.Domain.ValueObjects;
using YourBrand.Tenancy;

namespace YourBrand.Meetings.Domain.Entities;


public class MeetingAttendee : Entity<MeetingAttendeeId>, IAuditableEntity<MeetingAttendeeId>, IHasTenant, IHasOrganization
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

    public DateTimeOffset? AddedAt { get; set; }
    public DateTimeOffset? RemovedAt { get; set; }

    public DateTimeOffset? InvitedAt { get; set; }
    public DateTimeOffset? InviteAcceptedAt { get; set; }

    public DateTimeOffset? JoinedAt { get; set; }

    public bool IsPresent { get; set; }
    public bool? HasSpeakingRights { get; set; }
    public bool? HasVotingRights { get; set; }
    
    public AttendeeRole Role { get; set; }
    public int RoleId { get; set; }

    public User? CreatedBy { get; set; } = null!;
    public UserId? CreatedById { get; set; } = null!;
    public DateTimeOffset Created { get; set; }
    public User? LastModifiedBy { get; set; }
    public UserId? LastModifiedById { get; set; }
    public DateTimeOffset? LastModified { get; set; }
}