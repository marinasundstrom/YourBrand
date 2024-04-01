using YourBrand.TimeReport.Domain.Entities;

namespace YourBrand.TimeReport.Domain.Repositories;

public interface IOrganizationRepository
{
    void AddOrganization(Organization organization);

    void RemoveOrganization(Organization organization);

    IQueryable<Organization> GetOrganizations();

    Task<Organization?> GetOrganizationById(string id, CancellationToken cancellationToken = default);

    Task<Organization?> GetOrganizationByName(string name, CancellationToken cancellationToken);
}