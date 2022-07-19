using System.ComponentModel.DataAnnotations.Schema;

using YourBrand.HumanResources.Domain.Common;
using YourBrand.HumanResources.Domain.Common.Interfaces;
using YourBrand.HumanResources.Domain.Events;

namespace YourBrand.HumanResources.Domain.Entities;

// Add profile data for application persons by adding properties to the ApplicationPerson class
public class Person : AuditableEntity, ISoftDelete
{
    readonly HashSet<Team> _teams = new HashSet<Team>();
    readonly HashSet<TeamMembership> _teamMemberships = new HashSet<TeamMembership>();
    readonly HashSet<PersonDependant> _dependants = new HashSet<PersonDependant>();
    readonly HashSet<Role> _roles = new HashSet<Role>();
    readonly HashSet<PersonRole> _personRoles = new HashSet<PersonRole>();
    readonly HashSet<Contract> _contracts = new HashSet<Contract>();

    internal Person() { }

    public Person(Organization organization, string firstName, string lastName, string? displayName, string title, string? ssn, string email)
    {
        Organization = organization;
        Id = Guid.NewGuid().ToString();
        FirstName = firstName;
        LastName = lastName;
        DisplayName = displayName;
        Title = title;
        SSN = ssn;
        Email = email;

        AddDomainEvent(new UserCreated(Id));
    }

    public string Id { get; set; } = null!;

    public string FirstName { get; set; } = null!;

    public string LastName { get; set; } = null!;

    public string? DisplayName { get; set; }

    public string Title { get; set; }

    public string? SSN { get; set; }

    public string Email { get; set; } = null!;

    public Person? ReportsTo { get; set; }

    public Department? Department { get; /* private */ set; }

    public IReadOnlyCollection<Team> Teams => _teams;

    public IReadOnlyCollection<TeamMembership> TeamMemberships => _teamMemberships;


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

    public Organization Organization { get; private set; }

    public IReadOnlyCollection<PersonDependant> Dependants => _dependants;

    public void AddDependant(PersonDependant dependant) => _dependants.Add(dependant);

    public IReadOnlyCollection<Role> Roles => _roles;

    public void AddToRole(Role role) => _roles.Add(role);

    public IReadOnlyCollection<PersonRole> PersonRoles => _personRoles;

    public IReadOnlyCollection<Contract> Contracts => _contracts;

    public void AddContract(Contract contract) => _contracts.Add(contract);

    public BankAccount? BankAccount { get; set; }

    public DateTime? Deleted { get; set; }

    public string? DeletedBy { get; set; }
}
