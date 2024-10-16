namespace YourBrand.Meetings.Features;

public sealed record MeetingDto(int Id, string Title, string? Description, MeetingState State, DateTimeOffset? ScheduledAt, string Location, MeetingQuorumDto Quorum, IEnumerable<MeetingAttendeeDto> Attendees, int? CurrentAgendaItemIndex);

public sealed record MeetingQuorumDto(int RequiredNumber);

public sealed record MeetingAttendeeDto(string Id, string Name, AttendeeRole Role, string? Email, string? UserId, bool? HasSpeakingRights, bool? HasVotingRights, bool IsPresent);