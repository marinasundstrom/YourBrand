using YourBrand.Auditability;
using YourBrand.Domain;
using YourBrand.Identity;
using YourBrand.Meetings.Domain.ValueObjects;
using YourBrand.Tenancy;

namespace YourBrand.Meetings.Domain.Entities;

public class MeetingGroupMember : Entity<MeetingGroupMemberId>, IAuditableEntity<MeetingGroupMemberId>, IHasTenant, IHasOrganization
{
    protected MeetingGroupMember() { }

    public MeetingGroupMember(string name, string email, AttendeeRole role, UserId? userId, bool? hasSpeakingRights, bool? hasVotingRights)
        : base(new MeetingGroupMemberId())
    {
        Name = name;
        Email = email;
        Role = role;
        UserId = userId;
        HasSpeakingRights = hasSpeakingRights;
        HasVotingRights = hasVotingRights;
    }

    public TenantId TenantId { get; set; }
    public OrganizationId OrganizationId { get; set; }
    public MeetingGroupId MeetingGroupId { get; set; }

    public int Order { get; set; }

    public string? Name { get; set; }
    public string? Email { get; set; }

    public UserId? UserId { get; set; }

    public AttendeeRole Role { get; set; } = AttendeeRole.Attendee;
    public int RoleId{ get; set; }

    public bool? HasSpeakingRights { get; set; }
    public bool? HasVotingRights { get; set; }

    public User? CreatedBy { get; set; } = null!;
    public UserId? CreatedById { get; set; } = null!;
    public DateTimeOffset Created { get; set; }
    public User? LastModifiedBy { get; set; }
    public UserId? LastModifiedById { get; set; }
    public DateTimeOffset? LastModified { get; set; }
}