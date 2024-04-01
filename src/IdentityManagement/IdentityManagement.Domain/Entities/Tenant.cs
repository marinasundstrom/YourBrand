using YourBrand.IdentityManagement.Domain.Common;
using YourBrand.IdentityManagement.Domain.Events;

namespace YourBrand.IdentityManagement.Domain.Entities;

public class Tenant : AuditableEntity
{
    private readonly HashSet<User> _users = new HashSet<User>();
    private readonly HashSet<Organization> _organizations = new HashSet<Organization>();

    private Tenant() { }

    public Tenant(string id, string name, string? friendlyName)
    {
        Id = id;
        Name = name;
        FriendlyName = friendlyName;

        AddDomainEvent(new TenantCreated(Id));
    }

    public Tenant(string name, string? friendlyName) : this(Guid.NewGuid().ToString(), name, friendlyName)
    {

    }

    public string Id { get; private set; }

    public string Name { get; private set; }

    public string? FriendlyName { get; private set; }

    public IReadOnlyCollection<Organization> Organizations => _organizations;

    public IReadOnlyCollection<User> Users => _users;

    public void ChangeName(string name)
    {
        if(Name != name) 
        {
            Name = name;
        }
    }
}
