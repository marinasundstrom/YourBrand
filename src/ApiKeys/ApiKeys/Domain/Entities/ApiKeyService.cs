namespace YourBrand.ApiKeys.Domain.Entities;

public class ApiKeyService
{
    public string Id { get; set; } = Guid.NewGuid().ToString();

    public ApiKey ApiKey { get; set; } = null!;
    public Service Service { get; set; } = null!;
}