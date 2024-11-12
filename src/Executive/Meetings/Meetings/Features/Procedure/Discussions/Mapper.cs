using YourBrand.Meetings.Features.Organizations;
using YourBrand.Meetings.Features.Users;

namespace YourBrand.Meetings.Features.Procedure.Discussions;

public static partial class Mappings
{
    public static DiscussionDto ToDto(this Discussion discussion) => new(discussion.Id, discussion.State, 
        discussion.GetOrderedSpeakerQueue().Select(x => x.ToDto()), discussion.CurrentSpeaker?.ToDto());

    public static SpeakerRequestDto ToDto(this SpeakerRequest speakerRequest) => new(speakerRequest.Id, speakerRequest.Name, speakerRequest.AttendeeId);
}