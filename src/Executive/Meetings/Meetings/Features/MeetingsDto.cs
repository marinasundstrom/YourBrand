namespace YourBrand.Meetings.Features;

public sealed record MeetingDto(int Id, string Title, string? Description, MeetingState State, DateTimeOffset? ScheduledAt, string Location, 
    MeetingQuorumDto Quorum, IEnumerable<MeetingAttendeeDto> Attendees, int? CurrentAgendaItemIndex, int? CurrentAgendaSubItemIndex,
    IDictionary<string, DtoAction> Actions);

public sealed record MeetingQuorumDto(int RequiredNumber);

public sealed record MeetingAttendeeDto(string Id, string Name, AttendeeRoleDto Role, string? Email, string? UserId, bool? HasSpeakingRights, bool? HasVotingRights, bool IsPresent);

public sealed record AttendeeRoleDto(int Id, string Name, string? Description);

public sealed record DtoAction(string Method, string Href);