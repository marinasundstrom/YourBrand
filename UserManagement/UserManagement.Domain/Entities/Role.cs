namespace YourBrand.UserManagement.Domain.Entities;

public class Role
{
    readonly HashSet<User> _users = new HashSet<User>();
    readonly HashSet<UserRole> _userRoles = new HashSet<UserRole>();

    internal Role()
    {

    }

    public Role(string name)
    {
        Id = Guid.NewGuid().ToString();
        Name = name;
    }

    public string Id { get; private set; } = null!;

    public string Name { get; private set; } = null!;

    public IReadOnlyCollection<User> Users => _users;

    public IReadOnlyCollection<UserRole> UserRoles => _userRoles;
}
