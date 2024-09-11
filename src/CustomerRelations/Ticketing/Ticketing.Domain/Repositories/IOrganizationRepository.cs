using YourBrand.Ticketing.Domain.Entities;
using YourBrand.Ticketing.Domain.ValueObjects;
using YourBrand.Identity;
using YourBrand.Domain;

namespace YourBrand.Ticketing.Domain.Repositories;

public interface IOrganizationRepository : IRepository<Organization, OrganizationId>
{

}