namespace YourBrand.Meetings.Features.Procedure.Discussions;

public sealed record SpeakerSessionDto(string Id, SpeakerSessionState State, IEnumerable<SpeakerRequestDto> SpeakerQueue, SpeakerRequestDto? CurrentSpeaker);

public sealed record SpeakerRequestDto(string Id, string Name, string AttendeeId);