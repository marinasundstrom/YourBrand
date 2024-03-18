namespace YourBrand.Ticketing.Domain;

public  static partial class Errors
{
    public static class Tickets
    {
        public static readonly Error TicketNotFound = new Error(nameof(TicketNotFound), "Ticket not found", string.Empty);
    }
}
