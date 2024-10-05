using YourBrand.Meetings.Features.Users;
using YourBrand.Meetings.Features.Organizations;

namespace YourBrand.Meetings.Features;

public static partial class Mappings
{
    public static MeetingDto ToDto(this Meeting ticket) => new(ticket.Id, ticket.Title!, ticket.State, ticket.ScheduledAt, ticket.Location, ticket.Quorum.ToDto(), ticket.Participants.Select(x => x.ToDto()));
    public static MeetingQuorumDto ToDto(this Quorum quorum) => new(quorum.RequiredNumber);
    public static MeetingParticipantDto ToDto(this MeetingParticipant participant) => new(participant.Id, participant.Name!, participant.Email, participant.UserId, participant.HasVotingRights);
}