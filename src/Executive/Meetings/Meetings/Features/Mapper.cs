using YourBrand.Meetings.Features.Users;
using YourBrand.Meetings.Features.Organizations;

namespace YourBrand.Meetings.Features;

public static partial class Mappings
{
    public static MeetingDto ToDto(this Meeting meeting) => new(meeting.Id, meeting.Title!, meeting.Description, meeting.State, meeting.ScheduledAt, meeting.Location, meeting.Quorum.ToDto(), meeting.Participants.Select(x => x.ToDto()), meeting.CurrentAgendaItemIndex);
    public static MeetingQuorumDto ToDto(this Quorum quorum) => new(quorum.RequiredNumber);
    public static MeetingParticipantDto ToDto(this MeetingParticipant participant) => new(participant.Id, participant.Name!, participant.Role, participant.Email, participant.UserId, participant.HasVotingRights, participant.IsPresent);
}