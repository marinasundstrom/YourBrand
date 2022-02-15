// Copyright (c) Duende Software. All rights reserved.
// See LICENSE in the project root for license information.


using Microsoft.AspNetCore.Identity;

using Skynet.IdentityService.Domain.Common.Interfaces;

namespace Skynet.IdentityService.Domain.Entities;

// Add profile data for application users by adding properties to the ApplicationUser class
public class ApplicationUser : IdentityUser, IAuditableEntity, ISoftDelete
{
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string? DisplayName { get; set; }
    public string? SSN { get; set; }

    public Department Department { get; set; }
    public List<Team> Teams { get; } = new List<Team>();

    public DateTime Created { get; set; }
    public string CreatedBy { get; set; }
    public DateTime? LastModified { get; set; }
    public string? LastModifiedBy { get; set; }

    public DateTime? Deleted { get; set; }
    public string DeletedBy { get; set; }
}

public class Team 
{
    public string Id { get; set; }

    public string Name { get; set; }

    public Department Department { get; set; }

    public List<ApplicationUser> Persons { get; } = new List<ApplicationUser>();
}

public class Department 
{
    public string Id { get; set; }

    public string Name { get; set; }

    public List<Team> Teams { get; } = new List<Team>();

    public List<ApplicationUser> Persons { get; } = new List<ApplicationUser>();
}