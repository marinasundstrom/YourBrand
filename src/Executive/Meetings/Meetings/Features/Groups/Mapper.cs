using YourBrand.Meetings.Features.Organizations;
using YourBrand.Meetings.Features.Users;

namespace YourBrand.Meetings.Features.Groups;

public static partial class Mappings
{
    public static MeetingGroupDto ToDto(this MeetingGroup meetingGroup) => new(meetingGroup.Id, meetingGroup.Name!, meetingGroup.Description, meetingGroup.Quorum.ToDto2(), meetingGroup.Members.Select(x => x.ToDto()));
    public static MeetingGroupQuorumDto ToDto2(this Quorum quorum) => new(quorum.RequiredNumber);
    public static MeetingGroupMemberDto ToDto(this MeetingGroupMember member) => new(member.Id, member.Order, member.Name!, member.Role.ToDto(), member.Email, member.UserId, member.HasSpeakingRights, member.HasVotingRights);
    public static MemberRoleDto ToDto(this MemberRole role) =>
        new(role.Id, role.Name, role.Description);
}