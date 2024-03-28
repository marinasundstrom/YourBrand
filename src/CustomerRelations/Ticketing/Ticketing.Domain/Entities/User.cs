namespace YourBrand.Ticketing.Domain.Entities;

public class User : AggregateRoot<string>, IAuditable
{
    public User(string id, string name, string email)
        : base(id)
    {
        Id = id;
        Name = name;
        Email = email;
    }

    public string Name { get; private set; }

    public string Email { get; private set; }

    public User? CreatedBy { get; set; }

    public string? CreatedById { get; set; }

    public DateTimeOffset Created { get; set; }

    public User? LastModifiedBy { get; set; }

    public string? LastModifiedById { get; set; }

    public DateTimeOffset? LastModified { get; set; }
}
