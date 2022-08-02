namespace YourBrand.Showroom.Domain.Entities;

public class Company
{
    public string Id { get; set; } = null!;

    public string Name { get; set; } = null!;

    public string? Logo { get; set; }

    public string? Link { get; set; }
}
