namespace YourBrand.Ticketing.Domain.Entities;

public sealed class TicketType : Entity<int>
{
    protected TicketType() : base()
    {

    }

    public TicketType(string name) : base()
    {
        Name = name;
    }

    public string Name { get; set; } = null!;
}
