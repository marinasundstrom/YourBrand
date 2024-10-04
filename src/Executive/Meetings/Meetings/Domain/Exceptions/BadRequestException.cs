namespace YourBrand.Meetings.Domain.Exceptions;

public class BadRequestException(string title) : Exception
{
    public string Title { get; } = title;
}