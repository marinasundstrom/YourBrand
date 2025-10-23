using System;

namespace YourBrand.Meetings.Features.Procedure.Discussions;

public sealed record DiscussionDto(string Id, DiscussionState State, int? SpeakingTimeLimitSeconds, IEnumerable<SpeakerRequestDto> SpeakerQueue, SpeakerRequestDto? CurrentSpeaker, SpeakerClockDto? CurrentSpeakerClock);

public sealed record SpeakerRequestDto(string Id, string Name, string AttendeeId, int? AllocatedSpeakingTimeSeconds, bool HasExtendedSpeakingTime);

public sealed record SpeakerClockDto(bool IsRunning, int ElapsedSeconds, DateTimeOffset? StartedAtUtc);