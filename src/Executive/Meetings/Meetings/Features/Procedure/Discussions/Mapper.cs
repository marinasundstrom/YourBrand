﻿using YourBrand.Meetings.Features.Users;
using YourBrand.Meetings.Features.Organizations;

namespace YourBrand.Meetings.Features.Procedure.Discussions;

public static partial class Mappings
{
    public static SpeakerSessionDto ToDto(this SpeakerSession speakerSession) => new(speakerSession.Id, speakerSession.SpeakerQueue.Select(x => x.ToDto()));

    public static SpeakerRequestDto ToDto(this SpeakerRequest speakerRequest) => new(speakerRequest.Id, speakerRequest.ParticipantId);
}