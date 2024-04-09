namespace YourBrand.Analytics.Domain.Exceptions;

public class NotFoundException(string title) : Exception
{
    public string Title { get; } = title;
}