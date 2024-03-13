namespace YourBrand.Analytics.Domain.Exceptions;

public class ApplicationException : Exception
{
    public ApplicationException(string title)
    {
        Title = title;
    }

    public string Title { get; }
}
