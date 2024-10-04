namespace YourBrand.Meetings.Features;

public sealed record MeetingDto(int Id, string Title, IEnumerable<MeetingParticipantDto> Participants);

public sealed record MeetingParticipantDto(string Id, string UserId);