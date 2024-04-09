namespace YourBrand.Sales.Exceptions;

public class ValidationException(Dictionary<string, string[]> errors) : Exception
{
    public IReadOnlyDictionary<string, string[]> Errors { get; } = errors;
}