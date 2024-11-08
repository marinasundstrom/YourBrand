using YourBrand.Auditability;
using YourBrand.Domain;
using YourBrand.Identity;
using YourBrand.Meetings.Domain.ValueObjects;
using YourBrand.Tenancy;

namespace YourBrand.Meetings.Domain.Entities;

public class AttendeeRole
{
    public int Id { get; private set; }
    public string Name { get; private set; }
    public string? Description { get; private set; }

    // Permissions associated with each role
    public bool CanVote { get; private set; }
    public bool CanSpeak { get; private set; }
    public bool CanPropose { get; private set; }

    protected AttendeeRole() {}

    public AttendeeRole(int id, string name, bool canVote, bool canSpeak, bool canPropose, string? description = null)
    {
        Id = id;
        Name = name;
        Description = description;
        CanVote = canVote;
        CanSpeak = canSpeak;
        CanPropose = canPropose;
    }

    // Implement equality based on Id
    public override bool Equals(object? obj) =>
        obj is AttendeeRole other && Id == other.Id;

    public override int GetHashCode() => Id.GetHashCode();

    public static bool operator ==(AttendeeRole? left, AttendeeRole? right) =>
        Equals(left, right);

    public static bool operator !=(AttendeeRole? left, AttendeeRole? right) =>
        !Equals(left, right);

    // Static readonly instances for each attendee role
    public static readonly AttendeeRole Chairperson = new(1, "Chairperson", true, true, true, "Leads the meeting");
    public static readonly AttendeeRole Secretary = new(2, "Secretary", false, true, false, "Takes notes and records minutes");
    public static readonly AttendeeRole Attendee = new(3, "Attendee", true, true, true, "Active participant");
    public static readonly AttendeeRole Observer = new(4, "Observer", false, false, false, "Present without active participation");

    // List of all static instances for seeding purposes
    public static readonly AttendeeRole[] AllRoles = {
        Chairperson, Secretary, Attendee, Observer
    };
}


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