// Copyright (c) Duende Software. All rights reserved.
// See LICENSE in the project root for license information.

namespace YourBrand.IdentityService.Domain.Entities;

public class UserDependant 
{
    public string Id { get; set; }

    public string FirstName { get; set; }

    public string LastName { get; set; }

    public DependantRelationship Relationship { get; set; }

    public string PhoneNumber { get; set; }
}
