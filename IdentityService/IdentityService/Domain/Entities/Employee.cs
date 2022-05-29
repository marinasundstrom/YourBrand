// Copyright (c) Duende Software. All rights reserved.
// See LICENSE in the project root for license information.


using System.ComponentModel.DataAnnotations.Schema;

using Microsoft.AspNetCore.Identity;

using YourBrand.IdentityService.Domain.Common.Interfaces;

namespace YourBrand.IdentityService.Domain.Entities;

// Add profile data for application users by adding properties to the ApplicationUser class
public class Employee : IdentityUser, IAuditableEntity, ISoftDelete
{
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string? DisplayName { get; set; }
    public string? SSN { get; set; }

    public Employee ReportsTo { get; set; }

    public Department Department { get; set; }
    public List<Team> Teams { get; } = new List<Team>();

    public List<EmployeeDependant> Dependants { get; } = new List<EmployeeDependant>();

    public List<Role> Roles { get; } = new List<Role>();
    public List<UserRole> UserRoles { get; } = new List<UserRole>();

    public BankAccount? BankAccount { get; set; }

    public DateTime Created { get; set; }
    public string? CreatedBy { get; set; }
    public DateTime? LastModified { get; set; }
    public string? LastModifiedBy { get; set; }

    public DateTime? Deleted { get; set; }
    public string DeletedBy { get; set; }
}
