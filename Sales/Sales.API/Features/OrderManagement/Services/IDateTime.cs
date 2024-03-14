namespace YourBrand.Orders.Application.Services;

public interface IDateTime
{
    DateTimeOffset Now { get; }
}