namespace YourBrand.Portal.Services;

public interface IOrganizationProvider
{
    Task<IEnumerable<Organization>> GetAvailableOrganizationsAsync();

    Organization? CurrentOrganization { get; set; }

    Task SetCurrentOrganization(string storeId);

    event EventHandler? CurrentOrganizationChanged;
}

public record class Organization(string Id, string Name, string? FriendlyName);