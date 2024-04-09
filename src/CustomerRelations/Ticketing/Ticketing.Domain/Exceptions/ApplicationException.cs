namespace YourBrand.Ticketing.Domain.Exceptions;

public class ApplicationException(string title) : Exception
{
    public string Title { get; } = title;
}