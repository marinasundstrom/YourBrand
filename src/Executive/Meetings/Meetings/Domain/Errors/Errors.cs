namespace YourBrand.Meetings.Domain;

public static partial class Errors
{
    public static class Meetings
    {
        public static readonly Error MeetingNotFound = new Error(nameof(MeetingNotFound), "Meeting not found", string.Empty);

        public static readonly Error AttendeeNotFound = new Error(nameof(AttendeeNotFound), "Attendee not found", string.Empty);

        public static readonly Error AgendaItemNotFound = new Error(nameof(AgendaItemNotFound), "Agenda item not found", string.Empty);

        public static readonly Error NoActiveAgendaItem = new Error(nameof(NoActiveAgendaItem), "No active agenda item", string.Empty);

        public static readonly Error NoOngoingSpeakerSession = new Error(nameof(NoOngoingSpeakerSession), "No on-going speaker session", string.Empty);

        public static readonly Error NoOngoingVotingSession = new Error(nameof(NoOngoingVotingSession), "No on-going voting session", string.Empty);

        public static readonly Error YouAreNotAttendeeOfMeeting = new Error(nameof(YouAreNotAttendeeOfMeeting), "You are not a attendee of this meeting.", string.Empty);

        public static readonly Error YouAreNotChairpersonOfMeeting = new Error(nameof(YouAreNotChairpersonOfMeeting), "You are not the chairperson of this meeting.", string.Empty);

        public static readonly Error YouHaveNoVotingRights = new Error(nameof(YouHaveNoVotingRights), "You have no voting rights.", string.Empty);

        public static readonly Error OnlyChairpersonCanStartTheMeeting = new Error(nameof(OnlyChairpersonCanStartTheMeeting), "Only the chairperson can start the meeting.", string.Empty);

        public static readonly Error OnlyChairpersonCanMoveToNextAgendaItem = new Error(nameof(OnlyChairpersonCanMoveToNextAgendaItem), "Only the chairperson can move to the next agenda item.", string.Empty);

        public static readonly Error OnlyChairpersonCanCompleteAgendaItem = new Error(nameof(OnlyChairpersonCanCompleteAgendaItem), "Only the chairperson can complete agenda items.", string.Empty);

        public static readonly Error OnlyChairpersonCanPostponeAgendaItem = new Error(nameof(OnlyChairpersonCanPostponeAgendaItem), "Only the chairperson can postpone agenda items.", string.Empty);

        public static readonly Error OnlyChairpersonCanCancelAgendaItem = new Error(nameof(OnlyChairpersonCanCancelAgendaItem), "Only the chairperson can cancel agenda items.", string.Empty);

        public static readonly Error OnlyChairpersonCanEndTheMeeting = new Error(nameof(OnlyChairpersonCanEndTheMeeting), "Only the chairperson can end the meeting.", string.Empty);

        public static readonly Error OnlyChairpersonCanResetTheMeetingProcedure = new Error(nameof(OnlyChairpersonCanResetTheMeetingProcedure), "Only the chairperson can reset the meeting procedure.", string.Empty);

        public static readonly Error OnlyChairpersonCanStartDiscussion = new Error(nameof(OnlyChairpersonCanStartDiscussion), "Only the Chairperson can start a discussion.", string.Empty);

        public static readonly Error OnlyChairpersonCanEndDiscussion = new Error(nameof(OnlyChairpersonCanEndDiscussion), "Only the Chairperson can end a discussion.", string.Empty);

        public static readonly Error OnlyChairpersonCanStartVotingSession = new Error(nameof(OnlyChairpersonCanStartVotingSession), "Only the Chairperson can start a voting session.", string.Empty);

        public static readonly Error OnlyChairpersonCanEndVotingSession = new Error(nameof(OnlyChairpersonCanEndVotingSession), "Only the Chairperson can end a voting session.", string.Empty);

        public static readonly Result NotAnAttendantOfMeeting = new Error(nameof(NotAnAttendantOfMeeting), "Not an attendee of this meeting.", string.Empty);

        public static readonly Result CandidateAlreadyProposed = new Error(nameof(CandidateAlreadyProposed), "Attendee has already been proposed as a candidate.", string.Empty);

        public static readonly Result CandidateNotFound = new Error(nameof(CandidateNotFound), "Candidate not found.", string.Empty);
    }

    public static class Agendas
    {
        public static readonly Error AgendaNotFound = new Error(nameof(AgendaNotFound), "Agenda not found", string.Empty);

        public static readonly Error AgendaItemNotFound = new Error(nameof(AgendaItemNotFound), "Agenda item not found", string.Empty);
    }

    public static class Minutes
    {
        public static readonly Error MinuteNotFound = new Error(nameof(MinuteNotFound), "Minutes not found", string.Empty);

        public static readonly Error MinutesItemNotFound = new Error(nameof(MinutesItemNotFound), "Minutes item not found", string.Empty);
    }

    public static class Motions
    {
        public static readonly Error MotionNotFound = new Error(nameof(MotionNotFound), "Motions not found", string.Empty);

        public static readonly Error OperativeClauseNotFound = new Error(nameof(OperativeClauseNotFound), "Motions item not found", string.Empty);
    }

    public static class MeetingGroups
    {
        public static readonly Error MeetingGroupNotFound = new Error(nameof(MeetingGroupNotFound), "Meeting group not found", string.Empty);

        public static readonly Error MeetingGroupMemberNotFound = new Error(nameof(MeetingGroupMemberNotFound), "Meeting group member not found", string.Empty);
    }

    public static class Users
    {
        public static readonly Error UserNotFound = new Error(nameof(UserNotFound), "User not found", string.Empty);
    }

    public static class Organizations
    {
        public static readonly Error OrganizationNotFound = new Error(nameof(OrganizationNotFound), "Organization not found", string.Empty);
    }
}