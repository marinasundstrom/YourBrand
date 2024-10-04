namespace YourBrand.Meetings.Services;

public interface IDateTime
{
    DateTimeOffset Now { get; }
}