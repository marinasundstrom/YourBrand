using YourBrand.IdentityManagement.Domain.Common;
using YourBrand.IdentityManagement.Domain.Events;
using YourBrand.Tenancy;

namespace YourBrand.IdentityManagement.Domain.Entities;

public class Tenant : AuditableEntity
{
    private readonly HashSet<User> _users = new HashSet<User>();
    private readonly HashSet<Organization> _organizations = new HashSet<Organization>();

    private Tenant() { }

    public Tenant(TenantId id, string name, string? friendlyName)
    {
        Id = id;
        Name = name;
        FriendlyName = friendlyName;

        AddDomainEvent(new TenantCreated(Id));
    }

    public Tenant(string name, string? friendlyName) : this(Guid.NewGuid().ToString(), name, friendlyName)
    {

    }

    public TenantId Id { get; private set; }

    public string Name { get; private set; }

    public void ChangeName(string name)
    {
        if (Name != name)
        {
            Name = name;
        }
    }

    public string? FriendlyName { get; private set; }

    public IReadOnlyCollection<User> Users => _users;

    public void AddUser(User user)
    {
        user.TenantId = Id;
        user.Tenant = this;

        _users.Add(user);
    }

    public IReadOnlyCollection<Organization> Organizations => _organizations;

    public void AddOrganization(Organization organization)
    {
        organization.TenantId = Id;
        organization.Tenant = this;

        _organizations.Add(organization);
    }
}