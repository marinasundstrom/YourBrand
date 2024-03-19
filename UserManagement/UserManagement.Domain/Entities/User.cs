using System.ComponentModel.DataAnnotations.Schema;

using Microsoft.AspNetCore.Identity;

using YourBrand.UserManagement.Domain.Common;
using YourBrand.UserManagement.Domain.Common.Interfaces;
using YourBrand.UserManagement.Domain.Events;
using YourBrand.UserManagement.Domain.ValueObjects;

namespace YourBrand.UserManagement.Domain.Entities;

// Add profile data for application persons by adding properties to the ApplicationUser class
public class User : IdentityUser, IAuditableEntity, ISoftDelete
{
    readonly HashSet<Role> _roles = new HashSet<Role>();
    readonly HashSet<UserRole> _userRoles = new HashSet<UserRole>();

    public User() { }

    public User(string firstName, string lastName, string? displayName, string email)
    {
        Id = Guid.NewGuid().ToString();
        FirstName = firstName;
        LastName = lastName;
        DisplayName = displayName;
        Email = email;

        //AddDomainEvent(new UserCreated(Id));
    }

    public string FirstName { get; set; } = null!;

    public string LastName { get; set; } = null!;

    public string? DisplayName { get; set; }

    public Organization? Organization { get; set; }

    public DateTime Created { get; set; }

    public string? CreatedBy { get; set; }

    public DateTime? LastModified { get; set; }

    public string? LastModifiedBy { get; set; }

    public DateTime? Deleted { get; set; }

    public string? DeletedBy { get; set; }

    public IReadOnlyCollection<Role> Roles => _roles;

    public IReadOnlyCollection<UserRole> UserRoles => _userRoles;

    public void AddToRole(Role role) => _roles.Add(role);
}
