namespace YourBrand.Sales.Features.Services;

public interface IDateTime
{
    DateTimeOffset Now { get; }
}