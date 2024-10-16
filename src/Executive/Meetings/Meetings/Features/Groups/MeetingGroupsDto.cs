namespace YourBrand.Meetings.Features.Groups;

public sealed record MeetingGroupDto(int Id, string Title, string Description, MeetingGroupQuorumDto Quorum, IEnumerable<MeetingGroupMemberDto> Members);

public sealed record MeetingGroupQuorumDto(int RequiredNumber);

public sealed record MeetingGroupMemberDto(string Id, string Name, AttendeeRole Role, string? Email, string? UserId, bool? HasSpeakingRights, bool? HasVotingRights);