using YourBrand.Meetings.Features.Users;
using YourBrand.Meetings.Features.Organizations;

namespace YourBrand.Meetings.Features.Groups;

public static partial class Mappings
{
    public static MeetingGroupDto ToDto(this MeetingGroup meetingGroup) => new(meetingGroup.Id, meetingGroup.Name!, meetingGroup.Description,meetingGroup.Quorum.ToDto2(), meetingGroup.Members.Select(x => x.ToDto()));
    public static MeetingGroupQuorumDto ToDto2(this Quorum quorum) => new(quorum.RequiredNumber);
    public static MeetingGroupMemberDto ToDto(this MeetingGroupMember member) => new(member.Id, member.Name!, member.Role, member.Email, member.UserId, member.HasSpeakingRights, member.HasVotingRights);
}