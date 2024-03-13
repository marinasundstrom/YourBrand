namespace YourBrand.Analytics.Domain.Exceptions;

public class NotFoundException : Exception
{
    public NotFoundException(string title)
    {
        Title = title;
    }

    public string Title { get; }
}
