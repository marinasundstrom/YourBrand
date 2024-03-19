using YourBrand.IdentityManagement.Domain.Common;
using YourBrand.IdentityManagement.Domain.Events;

namespace YourBrand.IdentityManagement.Domain.Entities;

public class Organization : AuditableEntity
{
    private readonly HashSet<User> _users = new HashSet<User>();

    private Organization() { }

    public Organization(string id, string name, string? friendlyName)
    {
        Id = id;
        Name = name;
        FriendlyName = friendlyName;

        AddDomainEvent(new OrganizationCreated(Id));
    }

    public Organization(string name, string? friendlyName) : this(Guid.NewGuid().ToString(), name, friendlyName)
    {

    }

    public string Id { get; private set; }

    public string Name { get; private set; }

    public string? FriendlyName { get; private set; }

    public string Currency { get; set; } = "SEK";

    public IReadOnlyCollection<User> Users => _users;

    public void ChangeName(string name)
    {
        if(Name != name) 
        {
            Name = name;
        }
    }
}
