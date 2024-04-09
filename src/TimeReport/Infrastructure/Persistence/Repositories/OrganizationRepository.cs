using Microsoft.EntityFrameworkCore;

using YourBrand.TimeReport.Domain.Entities;
using YourBrand.TimeReport.Domain.Repositories;
using YourBrand.TimeReport.Infrastructure.Persistence;

namespace YourBrand.TimeReport.Domain;

public sealed class OrganizationRepository(TimeReportContext context) : IOrganizationRepository
{
    public void AddOrganization(Organization organization)
    {
        context.Organizations.Add(organization);
    }

    public async Task<Organization?> GetOrganizationById(string id, CancellationToken cancellationToken = default)
    {
        return await context.Organizations.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
    }

    public async Task<Organization?> GetOrganizationByName(string name, CancellationToken cancellationToken = default)
    {
        return await context.Organizations.FirstOrDefaultAsync(x => x.Name == name, cancellationToken);
    }

    public IQueryable<Organization> GetOrganizations()
    {
        return context.Organizations
                .AsQueryable();
    }

    public void RemoveOrganization(Organization organization)
    {
        context.Organizations.Remove(organization);
    }
}