namespace YourBrand.Meetings.Domain;

public static partial class Errors
{
    public static class Meetings
    {
        public static readonly Error MeetingNotFound = new Error(nameof(MeetingNotFound), "Meeting not found", string.Empty);

        public static readonly Error ParticipantNotFound = new Error(nameof(ParticipantNotFound), "Participant not found", string.Empty);
    }

    public static class Agendas
    {
        public static readonly Error AgendaNotFound = new Error(nameof(AgendaNotFound), "Agenda not found", string.Empty);

        public static readonly Error AgendaItemNotFound = new Error(nameof(AgendaItemNotFound), "Agenda item not found", string.Empty);
    }

    public static class Motions
    {
        public static readonly Error MotionNotFound = new Error(nameof(MotionNotFound), "Motions not found", string.Empty);

        public static readonly Error MotionItemNotFound = new Error(nameof(MotionItemNotFound), "Motions item not found", string.Empty);
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