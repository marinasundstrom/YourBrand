using YourBrand.Domain;
using YourBrand.Identity;
using YourBrand.Meetings.Domain.Entities;
using YourBrand.Meetings.Domain.ValueObjects;

namespace YourBrand.Meetings.Domain.Repositories;

public interface IOrganizationRepository : IRepository<Organization, OrganizationId>
{

}