namespace YourBrand.HumanResources.Domain.Entities;

public class PersonRole
{
    public Person User { get; set; }

    public string UserId { get; set; }

    public Role Role { get; set; }

    public string RoleId { get; set; }
}
