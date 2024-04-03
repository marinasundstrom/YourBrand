namespace YourBrand.Carts.Services;

public interface IDateTime
{
    DateTimeOffset Now { get; }
}