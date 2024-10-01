using Microsoft.AspNetCore.Identity;

using YourBrand.IdentityManagement.Domain.Common.Interfaces;
using YourBrand.Tenancy;

namespace YourBrand.IdentityManagement.Domain.Entities;

// Add profile data for application persons by adding properties to the ApplicationUser class
public class User : IdentityUser, IAuditableEntity, ISoftDeletable, IHasTenant
{
    readonly HashSet<Role> _roles = new HashSet<Role>();
    readonly HashSet<UserRole> _userRoles = new HashSet<UserRole>();

    public User() { }


    public User(string id, string firstName, string lastName, string? displayName, string email)
    {
        Id = id;
        FirstName = firstName;
        LastName = lastName;
        DisplayName = displayName;
        Email = email;

        //AddDomainEvent(new UserCreated(Id));
    }

    public User(string firstName, string lastName, string? displayName, string email)
        : this(Guid.NewGuid().ToString(), firstName, lastName, displayName, email)
    {
    }

    public string FirstName { get; set; } = null!;

    public string LastName { get; set; } = null!;

    public string? DisplayName { get; set; }

    public Tenant Tenant { get; set; }

    public TenantId TenantId { get; set; }

    public Organization? Organization => Organizations.FirstOrDefault();

    public DateTime Created { get; set; }

    public string? CreatedBy { get; set; }

    public DateTime? LastModified { get; set; }

    public string? LastModifiedBy { get; set; }

    public DateTime? Deleted { get; set; }

    public string? DeletedBy { get; set; }

    public IReadOnlyCollection<Role> Roles => _roles;

    public IReadOnlyCollection<UserRole> UserRoles => _userRoles;

    public void AddToRole(Role role) => _roles.Add(role);

    public IEnumerable<Organization> Organizations { get; set; }

    public IEnumerable<OrganizationUser> OrganizationUsers { get; set; }
}