namespace YourBrand.Catalog.Services;

public interface IDateTime
{
    DateTimeOffset Now { get; }
}