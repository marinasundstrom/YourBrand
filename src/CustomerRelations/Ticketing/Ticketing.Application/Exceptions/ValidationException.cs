namespace YourBrand.Ticketing.Application.Exceptions;

public class ValidationException(Dictionary<string, string[]> errors) : Exception
{
    public IReadOnlyDictionary<string, string[]> Errors { get; } = errors;
}