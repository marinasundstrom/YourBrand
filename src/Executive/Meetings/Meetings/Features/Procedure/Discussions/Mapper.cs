using System;
using System.Linq;

using YourBrand.Meetings.Features.Organizations;
using YourBrand.Meetings.Features.Users;

namespace YourBrand.Meetings.Features.Procedure.Discussions;

public static partial class Mappings
{
    public static DiscussionDto ToDto(this Discussion discussion)
    {
        var now = DateTimeOffset.UtcNow;
        var currentSpeakerDto = discussion.CurrentSpeaker?.ToDto();
        var clockDto = discussion.CurrentSpeaker is null
            ? null
            : discussion.GetCurrentSpeakerClockSnapshot(now).ToDto();

        return new DiscussionDto(
            discussion.Id,
            discussion.State,
            (int?)discussion.SpeakingTimeLimit?.TotalSeconds,
            discussion.GetOrderedSpeakerQueue().Select(x => x.ToDto()),
            currentSpeakerDto,
            clockDto);
    }

    public static SpeakerRequestDto ToDto(this SpeakerRequest speakerRequest) => new(
        speakerRequest.Id,
        speakerRequest.Name,
        speakerRequest.AttendeeId,
        (int?)speakerRequest.AllocatedSpeakingTime?.TotalSeconds,
        speakerRequest.HasExtendedSpeakingTime);

    public static SpeakerClockDto ToDto(this SpeakerClockSnapshot snapshot) => new(
        snapshot.IsRunning,
        (int)Math.Max(0, Math.Round(snapshot.Elapsed.TotalSeconds)),
        snapshot.StartedAtUtc);
}
