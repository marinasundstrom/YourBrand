using YourBrand.Identity;
using YourBrand.Tenancy;
using YourBrand.Domain;
using YourBrand.Meetings.Domain.ValueObjects;

namespace YourBrand.Meetings.Domain.Entities;

public class MeetingGroupMember : Entity<MeetingGroupMemberId>, IAuditable, IHasTenant, IHasOrganization
{
    public MeetingGroupMember(string name, string email, ParticipantRole role, UserId? userId, bool hasVotingRights)
        : base(new MeetingGroupMemberId())
    {
        Name = name;
        Email = email;
        Role = role;
        UserId = userId;
        HasVotingRights = hasVotingRights;
    }

    public TenantId TenantId { get; set; }
    public OrganizationId OrganizationId { get; set; }
    public MeetingGroupId MeetingGroupId { get; set; }

    public int Order { get; set; }

    public string? Name { get; set; }
    public string? Email { get; set; }

    public UserId? UserId { get; set; }

    public ParticipantRole Role { get; set; } = ParticipantRole.Participant;

    public bool HasVotingRights { get; set; }

    public User? CreatedBy { get; set; } = null!;
    public UserId? CreatedById { get; set; } = null!;
    public DateTimeOffset Created { get; set; }
    public User? LastModifiedBy { get; set; }
    public UserId? LastModifiedById { get; set; }
    public DateTimeOffset? LastModified { get; set; }
}