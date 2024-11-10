namespace YourBrand.Meetings.Features.Procedure.Discussions;

public sealed record SpeakerSessionDto(string Id, IEnumerable<SpeakerRequestDto> SpeakerQueue);

public sealed record SpeakerRequestDto(string Id, string Name, string AttendeeId);