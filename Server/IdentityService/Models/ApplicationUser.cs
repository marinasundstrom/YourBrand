// Copyright (c) Duende Software. All rights reserved.
// See LICENSE in the project root for license information.


using Microsoft.AspNetCore.Identity;

namespace IdentityServerHost.Models;

// Add profile data for application users by adding properties to the ApplicationUser class
public class ApplicationUser : IdentityUser
{
    public Department Department { get; set; }

    public List<Team> Teams { get; } = new List<Team>();
    public string FirstName { get; internal set; }
    public string LastName { get; internal set; }
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