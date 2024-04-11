namespace YourBrand.Portal.Services;

public interface IOrganizationProvider
{
    Task<IEnumerable<Organization>> GetAvailableOrganizationsAsync(CancellationToken cancellationToken = default);

    Task<Organization?> GetCurrentOrganizationAsync(CancellationToken cancellationToken = default);

    Task SetCurrentOrganization(string storeId, CancellationToken cancellationToken = default);

    event EventHandler? CurrentOrganizationChanged;
}

public record class Organization(string Id, string Name, string? FriendlyName);