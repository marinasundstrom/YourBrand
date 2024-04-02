namespace YourBrand.Portal;

public interface IOrganizationSearchProvider
{
    Task<IEnumerable<Organization>> QueryOrganizationsAsync(string? searchTerm, CancellationToken cancellationToken = default);
}

public record Organization(string Id, string Name);