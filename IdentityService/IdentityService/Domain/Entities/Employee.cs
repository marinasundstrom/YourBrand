// Copyright (c) Duende Software. All rights reserved.
// See LICENSE in the project root for license information.


using System.ComponentModel.DataAnnotations.Schema;

using Microsoft.AspNetCore.Identity;

using YourBrand.IdentityService.Domain.Common.Interfaces;

namespace YourBrand.IdentityService.Domain.Entities;

// Add profile data for application users by adding properties to the ApplicationUser class
public class User : IdentityUser, IAuditableEntity, ISoftDelete
{
    readonly HashSet<Team> _teams = new HashSet<Team>();
    readonly HashSet<TeamMembership> _teamMemberships = new HashSet<TeamMembership>();
    readonly HashSet<UserDependant> _dependants = new HashSet<UserDependant>();
    readonly HashSet<Role> _roles = new HashSet<Role>();
    readonly HashSet<UserRole> _useRoles = new HashSet<UserRole>();

    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string? DisplayName { get; set; }
    public string? SSN { get; set; }

    public User ReportsTo { get; set; }

    public Department Department { get; set; }

    public IReadOnlyCollection<Team> Teams => _teams;

    public IReadOnlyCollection<TeamMembership> TeamMemberships => _teamMemberships;

    public IReadOnlyCollection<UserDependant> Dependants => _dependants;

    public IReadOnlyCollection<Role> Roles => _roles;

    public IReadOnlyCollection<UserRole> UserRoles => _useRoles;

    public BankAccount? BankAccount { get; set; }

    public DateTime Created { get; set; }

    public string? CreatedBy { get; set; }

    public DateTime? LastModified { get; set; }

    public string? LastModifiedBy { get; set; }

    public DateTime? Deleted { get; set; }

    public string DeletedBy { get; set; }
}
