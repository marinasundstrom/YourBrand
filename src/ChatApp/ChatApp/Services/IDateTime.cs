namespace YourBrand.ChatApp.Services;

public interface IDateTime
{
    DateTimeOffset Now { get; }
}