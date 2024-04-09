namespace YourBrand.Analytics.Domain.Exceptions;

public class BadRequestException(string title) : Exception
{
    public string Title { get; } = title;
}