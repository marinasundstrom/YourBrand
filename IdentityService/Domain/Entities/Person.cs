// Copyright (c) Duende Software. All rights reserved.
// See LICENSE in the project root for license information.


using System.ComponentModel.DataAnnotations.Schema;

using Microsoft.AspNetCore.Identity;

using YourBrand.IdentityService.Domain.Common.Interfaces;

namespace YourBrand.IdentityService.Domain.Entities;

// Add profile data for application users by adding properties to the ApplicationUser class
public class Person : IdentityUser, IAuditableEntity, ISoftDelete
{
    readonly HashSet<Role> _roles = new HashSet<Role>();
    readonly HashSet<PersonRole> _personRoles = new HashSet<PersonRole>();

    /*
    private User() { }

    public User(string firstName, string lastName, string? displayName, string? ssn)
    {
        FirstName = firstName;
        LastName = lastName;
        DisplayName = displayName;
        SSN = ssn;
    }
    */

    public string FirstName { get; set; }

    public string LastName { get; set; }

    public string? DisplayName { get; set; }

    public string? SSN { get; set; }

    public Person? Manager { get; private set; }


    public void SetDisplayName(string displayName)
    {
        throw new NotImplementedException();
    }

    public void SetSSN(string ssn)
    {
        throw new NotImplementedException();
    }

    public void SetLastName(string lastName)
    {
        throw new NotImplementedException();
    }

    public IReadOnlyCollection<Role> Roles => _roles;

    public IReadOnlyCollection<PersonRole> PersonRoles => _personRoles;

    public DateTime Created { get; set; }

    public string? CreatedBy { get; set; }

    public DateTime? LastModified { get; set; }

    public string? LastModifiedBy { get; set; }

    public DateTime? Deleted { get; set; }

    public string? DeletedBy { get; set; }
}
