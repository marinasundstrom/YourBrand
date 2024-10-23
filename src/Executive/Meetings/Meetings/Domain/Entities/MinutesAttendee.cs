using YourBrand.Domain;
using YourBrand.Identity;
using YourBrand.Meetings.Domain.ValueObjects;
using YourBrand.Tenancy;

namespace YourBrand.Meetings.Domain.Entities;

public class MinutesAttendee : Entity<MinutesAttendeeId>, IAuditable, IHasTenant, IHasOrganization
{
    public MinutesAttendee()
        : base(new MinutesAttendeeId())
    {

    }

    public TenantId TenantId { get; set; }
    public OrganizationId OrganizationId { get; set; }

    public MinutesId MinutesId { get; set; }

    public string? Name { get; set; }
    public UserId? UserId { get; set; }

    public int Order { get; set; }

    public MeetingGroupId? MeetingGroupId { get; set; }
    public MeetingGroupMemberId? MeetingGroupMemberId { get; set; }

    public string? Email { get; set; }

    public bool IsPresent { get; set; }
    public bool? HasSpeakingRights { get; set; }
    public bool? HasVotingRights { get; set; }
    public AttendeeRole Role { get; set; }

    public User? CreatedBy { get; set; } = null!;
    public UserId? CreatedById { get; set; } = null!;
    public DateTimeOffset Created { get; set; }
    public User? LastModifiedBy { get; set; }
    public UserId? LastModifiedById { get; set; }
    public DateTimeOffset? LastModified { get; set; }
}