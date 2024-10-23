using YourBrand.Domain;
using YourBrand.Identity;
using YourBrand.Ticketing.Domain.Entities;
using YourBrand.Ticketing.Domain.ValueObjects;

namespace YourBrand.Ticketing.Domain.Repositories;

public interface IOrganizationRepository : IRepository<Organization, OrganizationId>
{

}