namespace YourBrand.Showroom.Domain.Entities;

public class Language
{
    public string Id { get; set; }

    public string Name { get; set; }

    public string NativeName { get; set; }

    public string? ISO639 { get; set; }
}