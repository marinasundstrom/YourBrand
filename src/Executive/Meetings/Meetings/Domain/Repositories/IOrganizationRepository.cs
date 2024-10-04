using YourBrand.Meetings.Domain.Entities;
using YourBrand.Meetings.Domain.ValueObjects;
using YourBrand.Identity;
using YourBrand.Domain;

namespace YourBrand.Meetings.Domain.Repositories;

public interface IOrganizationRepository : IRepository<Organization, OrganizationId>
{

}