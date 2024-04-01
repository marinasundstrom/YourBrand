namespace YourBrand.Sales.Services;

public interface IDateTime
{
    DateTimeOffset Now { get; }
}