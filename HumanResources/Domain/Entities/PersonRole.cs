namespace YourBrand.HumanResources.Domain.Entities;

public class PersonRole
{
    public Person Person { get; set; }

    public string PersonId { get; set; }

    public Role Role { get; set; }

    public string RoleId { get; set; }
}
