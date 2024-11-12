namespace YourBrand.Meetings.Features.Procedure.Discussions;

public sealed record DiscussionDto(string Id, DiscussionState State, IEnumerable<SpeakerRequestDto> SpeakerQueue, SpeakerRequestDto? CurrentSpeaker);

public sealed record SpeakerRequestDto(string Id, string Name, string AttendeeId);