namespace YourBrand.ApiKeys;

public interface IApiApplicationContext
{
    string? AppId { get; }

    string? AppName { get; }
}
