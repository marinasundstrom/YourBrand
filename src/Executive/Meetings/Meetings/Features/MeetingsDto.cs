namespace YourBrand.Meetings.Features;

public sealed record MeetingDto(int Id, string Title, string Description, MeetingState State, DateTimeOffset? ScheduledAt, string Location, MeetingQuorumDto Quorum, IEnumerable<MeetingParticipantDto> Participants, int CurrentAgendaItemIndex);

public sealed record MeetingQuorumDto(int RequiredNumber);

public sealed record MeetingParticipantDto(string Id, string Name, ParticipantRole Role, string? Email, string? UserId, bool HasVotingRights, bool IsPresent);