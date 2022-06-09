// Copyright (c) Duende Software. All rights reserved.
// See LICENSE in the project root for license information.

namespace YourBrand.IdentityService.Domain.Entities;

public class PersonDependant 
{
    private PersonDependant()
    {

    }

    public PersonDependant(string firstName, string lastName, DependantRelationship relationship, string phoneNumber)
    {
        Id = Guid.NewGuid().ToString();
        FirstName = firstName;
        LastName = lastName;
        Relationship = relationship;
        PhoneNumber = phoneNumber;
    }

    public string Id { get; private set; }

    public string FirstName { get; private set; }

    public string LastName { get; private set; }

    public DependantRelationship Relationship { get; private set; }

    public string PhoneNumber { get; private set; }
}
