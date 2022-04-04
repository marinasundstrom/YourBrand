// Copyright (c) Duende Software. All rights reserved.
// See LICENSE in the project root for license information.


using System.ComponentModel.DataAnnotations.Schema;

using Microsoft.AspNetCore.Identity;

using YourBrand.IdentityService.Domain.Common.Interfaces;

namespace YourBrand.IdentityService.Domain.Entities;

// Add profile data for application users by adding properties to the ApplicationUser class
public class User : IdentityUser, IAuditableEntity, ISoftDelete
{
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string? DisplayName { get; set; }
    public string? SSN { get; set; }

    public Department Department { get; set; }
    public List<Team> Teams { get; } = new List<Team>();

    public List<UserDependant> Dependants { get; } = new List<UserDependant>();

    public List<Role> Roles { get; } = new List<Role>();

    public List<UserRole> UserRoles { get; } = new List<UserRole>();

    public DateTime Created { get; set; }
    public string? CreatedBy { get; set; }
    public DateTime? LastModified { get; set; }
    public string? LastModifiedBy { get; set; }

    public DateTime? Deleted { get; set; }
    public string DeletedBy { get; set; }
}

public class Role : IdentityRole<string>
{
    public Role()
    {
        Id = Guid.NewGuid().ToString();
    }

    public List<User> Users { get; } = new List<User>();

    public List<UserRole> UserRoles { get; } = new List<UserRole>();
}

public class UserRole : IdentityUserRole<string>
{
    public User User { get; set; }

    public Role Role { get; set; }
}

public class Team 
{
    public string Id { get; set; }

    public string Name { get; set; }

    public Department Department { get; set; }

    public List<User> Persons { get; } = new List<User>();
}

public class Department 
{
    public string Id { get; set; }

    public string Name { get; set; }

    public List<Team> Teams { get; } = new List<Team>();

    public List<User> Persons { get; } = new List<User>();
}

public class UserDependant 
{
    public string Id { get; set; }

    public string FirstName { get; set; }

    public string LastName { get; set; }

    public DependantRelationship Relationship { get; set; }

    public string PhoneNumber { get; set; }
}

public enum DependantRelationship
{
    Spouse,
    Child,
    Mother,
    Father,
}