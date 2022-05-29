// Copyright (c) Duende Software. All rights reserved.
// See LICENSE in the project root for license information.

namespace YourBrand.IdentityService.Domain.Entities;

public class Team 
{
    public string Id { get; set; }

    public string Name { get; set; }

    public Department Department { get; set; }

    public List<Employee> Persons { get; } = new List<Employee>();
}
