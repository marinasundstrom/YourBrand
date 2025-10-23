
using YourBrand.Meetings.Features.Organizations;
using YourBrand.Meetings.Features.Users;

namespace YourBrand.Meetings.Features;

public static partial class Mappings
{
    public static MeetingDto ToDto(this Meeting meeting) =>
        new(meeting.Id, meeting.Title!, meeting.Description, meeting.State, meeting.ScheduledAt, meeting.Location,
        meeting.AdjournmentMessage, meeting.AdjournedAt, meeting.Quorum.ToDto(),
        meeting.Attendees.Select(x => x.ToDto()), meeting.CurrentAgendaItemIndex, meeting.CurrentAgendaSubItemIndex,
        meeting.ShowAgendaTimeEstimates,
        PrepareActions(meeting));

    private static IDictionary<string, DtoAction> PrepareActions(Meeting meeting)
    {
        Dictionary<string, DtoAction> actions = new Dictionary<string, DtoAction>();

        if(meeting.CanStart) 
        {
            actions.Add("start", new DtoAction("POST", $"/v1/Meetings/{meeting.Id}/Start"));
        }

        if (meeting.CanMoveNext)
        {
            actions.Add("nextItem", new DtoAction("POST", $"/v1/Meetings/{meeting.Id}/Agenda/NextItem"));
        }

        if (meeting.CanCancel)
        {
            actions.Add("cancel", new DtoAction("POST", $"/v1/Meetings/{meeting.Id}/Cancel"));
        }

        if (meeting.CanEnd)
        {
            actions.Add("end", new DtoAction("POST", $"/v1/Meetings/{meeting.Id}/End"));
        }

        if (meeting.State == MeetingState.InProgress)
        {
            actions.Add("adjourn", new DtoAction("POST", $"/v1/Meetings/{meeting.Id}/Adjourn"));
        }

        if (meeting.State == MeetingState.Adjourned)
        {
            actions.Add("resume", new DtoAction("POST", $"/v1/Meetings/{meeting.Id}/Resume"));
        }

        return actions;
    }

    public static MeetingQuorumDto ToDto(this Quorum quorum) => new(quorum.RequiredNumber);

    public static MeetingAttendeeDto ToDto(this MeetingAttendee attendee) => 
        new(attendee.Id, attendee.Name!, attendee.Role.ToDto(), attendee.Email, attendee.UserId, attendee.HasSpeakingRights, attendee.HasVotingRights, attendee.IsPresent);

    public static AttendeeRoleDto ToDto(this AttendeeRole role) =>
        new(role.Id, role.Name, role.Description);
}