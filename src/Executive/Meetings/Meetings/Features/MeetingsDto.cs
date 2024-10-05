namespace YourBrand.Meetings.Features;

public sealed record MeetingDto(int Id, string Title, MeetingState State, DateTimeOffset? ScheduledAt, string Location, MeetingQuorumDto Quorum, IEnumerable<MeetingParticipantDto> Participants);

public sealed record MeetingQuorumDto(int RequiredNumber);

public sealed record MeetingParticipantDto(string Id, string Name, string? Email, string? UserId, bool HasVotingRights);