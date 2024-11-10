using YourBrand.Meetings.Features.Organizations;
using YourBrand.Meetings.Features.Users;

namespace YourBrand.Meetings.Features.Procedure.Discussions;

public static partial class Mappings
{
    public static SpeakerSessionDto ToDto(this SpeakerSession speakerSession) => new(speakerSession.Id, speakerSession.State, 
        speakerSession.GetOrderedSpeakerQueue().Select(x => x.ToDto()), speakerSession.CurrentSpeaker?.ToDto());

    public static SpeakerRequestDto ToDto(this SpeakerRequest speakerRequest) => new(speakerRequest.Id, speakerRequest.Name, speakerRequest.AttendeeId);
}