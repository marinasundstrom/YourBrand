namespace YourBrand.Meetings.Domain;

public static partial class Errors
{
    public static class Tickets
    {
        public static readonly Error TicketNotFound = new Error(nameof(TicketNotFound), "Ticket not found", string.Empty);

        public static readonly Error TicketCommentNotFound = new Error(nameof(TicketCommentNotFound), "Ticket comment not found", string.Empty);

    }
}