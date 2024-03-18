namespace YourBrand.Ticketing.Domain.Entities;

public class Tag : Entity<int>
{
    public string Name { get; set; } = null!;
}
