namespace YourBrand.Ticketing.Domain.Entities;

public class TicketStatus : Entity<int>
{
    public string Name { get; set; } = null!;
}