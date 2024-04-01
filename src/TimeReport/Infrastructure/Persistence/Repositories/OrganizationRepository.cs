using Microsoft.EntityFrameworkCore;

using YourBrand.TimeReport.Domain.Entities;
using YourBrand.TimeReport.Domain.Repositories;
using YourBrand.TimeReport.Infrastructure.Persistence;

namespace YourBrand.TimeReport.Domain;

public sealed class OrganizationRepository : IOrganizationRepository
{
    private readonly TimeReportContext _context;

    public OrganizationRepository(TimeReportContext context)
    {
        _context = context;
    }

    public void AddOrganization(Organization organization)
    {
        _context.Organizations.Add(organization);
    }

    public async Task<Organization?> GetOrganizationById(string id, CancellationToken cancellationToken = default)
    {
        return await _context.Organizations.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
    }

    public async Task<Organization?> GetOrganizationByName(string name, CancellationToken cancellationToken = default)
    {
        return await _context.Organizations.FirstOrDefaultAsync(x => x.Name == name, cancellationToken);
    }

    public IQueryable<Organization> GetOrganizations()
    {
        return _context.Organizations
                .AsQueryable();
    }

    public void RemoveOrganization(Organization organization)
    {
        _context.Organizations.Remove(organization);
    }
}