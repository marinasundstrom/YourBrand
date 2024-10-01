using YourBrand.Domain;
using YourBrand.Identity;
using YourBrand.Showroom.Domain.Common;
using YourBrand.Showroom.Domain.ValueObjects;
using YourBrand.Tenancy;

namespace YourBrand.Showroom.Domain.Entities;

public class Organization : AuditableEntity, IOrganization, IHasTenant, ISoftDeletable
{
    readonly HashSet<User> _users = new HashSet<User>();
    readonly HashSet<Organization> _subOrganizations = new HashSet<Organization>();
    readonly HashSet<OrganizationUser> _organizationUsers = new HashSet<OrganizationUser>();

    public OrganizationId Id { get; set; }

    public TenantId TenantId { get; set; } = null!;

    public string Name { get; set; } = null!;

    public Address? Address { get; set; } = null!;

    public ICollection<PersonProfile> PersonProfiles { get; set; } = null!;

    public IReadOnlyCollection<User> Users => _users;

    public IReadOnlyCollection<Organization> SubOrganizations => _subOrganizations;

    public IReadOnlyCollection<OrganizationUser> OrganizationUsers => _organizationUsers;

    public DateTimeOffset? Deleted { get; set; }
    public UserId? DeletedById { get; set; }
    public User? DeletedBy { get; set; }

    public void AddUser(User user)
    {
        _users.Add(user);
    }

    public bool HasUser(UserId userId)
    {
        return Users.Any(x => x.Id == userId);
    }
}