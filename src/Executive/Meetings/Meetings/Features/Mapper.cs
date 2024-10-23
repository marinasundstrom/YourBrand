using YourBrand.Meetings.Features.Organizations;
using YourBrand.Meetings.Features.Users;

namespace YourBrand.Meetings.Features;

public static partial class Mappings
{
    public static MeetingDto ToDto(this Meeting meeting) => new(meeting.Id, meeting.Title!, meeting.Description, meeting.State, meeting.ScheduledAt, meeting.Location, meeting.Quorum.ToDto(), meeting.Attendees.Select(x => x.ToDto()), meeting.CurrentAgendaItemIndex);
    public static MeetingQuorumDto ToDto(this Quorum quorum) => new(quorum.RequiredNumber);
    public static MeetingAttendeeDto ToDto(this MeetingAttendee attendee) => new(attendee.Id, attendee.Name!, attendee.Role, attendee.Email, attendee.UserId, attendee.HasSpeakingRights, attendee.HasVotingRights, attendee.IsPresent);
}